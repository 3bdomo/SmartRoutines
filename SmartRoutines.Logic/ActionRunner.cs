using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Collections.ObjectModel;

namespace SmartRoutines.Logic;

/// <summary>
/// Orchestrates the execution of a routine's action pipeline by selecting the
/// correct strategy for each <see cref="RuntimeAction"/> and reporting progress
/// in real-time through <see cref="ILiveLogger"/>.
/// </summary>
/// <remarks>
/// <strong>Threading contract:</strong> Every executor call is wrapped in
/// <c>Task.Run</c> so that CPU-bound executors (e.g. audio, process kill) never
/// block the calling thread (typically the WinForms UI thread or an engine
/// background thread). Genuinely async executors (e.g. <c>RunCommandExecutor</c>)
/// unblock the thread immediately after their first <c>await</c>, so the
/// <c>Task.Run</c> wrapping adds negligible overhead for them.
///
/// <strong>Persistence:</strong> <see cref="ActionRunner"/> does not persist logs
/// to the database. Database logging is decoupled and handled by a separate
/// subscriber of <see cref="ILiveLogger"/>.
/// </remarks>
public sealed class ActionRunner
{
    private readonly IReadOnlyDictionary<ActionType, IAction> _actionMap;
    private readonly ILiveLogger _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ActionRunner"/>.
    /// </summary>
    /// <param name="actions">The available action executor strategies.</param>
    /// <param name="logger">Real-time logger for streaming execution events to the UI.</param>
    public ActionRunner(IEnumerable<IAction> actions, ILiveLogger logger)
    {
        ArgumentNullException.ThrowIfNull(actions);
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _actionMap = BuildActionMap(actions);
    }

    /// <summary>
    /// Executes all provided actions in <see cref="RuntimeAction.ExecutionOrder"/> sequence.
    /// Actions with no registered executor are skipped with a warning.
    /// Failures in one action do not abort subsequent actions unless the
    /// <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="actions">The runtime action snapshots to execute.</param>
    /// <param name="context">The runtime context for this routine run.</param>
    /// <param name="cancellationToken">A token that cancels the entire pipeline.</param>
    public async Task RunAsync(
        IEnumerable<RuntimeAction> actions,
        ActionContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actions);
        ArgumentNullException.ThrowIfNull(context);

        // Materialise + sort once — IEnumerable could be deferred
        var pipeline = actions
            .OrderBy(a => a.ExecutionOrder)
            .ToList();

        _logger.LogInfo(
            $"[{context.RoutineName}] Pipeline started — {pipeline.Count} action(s) queued.");

        for (int i = 0; i < pipeline.Count; i++)
        {
            var runtimeAction = pipeline[i];

            // Honour cancellation before each step
            cancellationToken.ThrowIfCancellationRequested();

            if (!_actionMap.TryGetValue(runtimeAction.Type, out var executor))
            {
                _logger.LogWarning(
                    $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}: " +
                    $"No executor registered for '{runtimeAction.Type}'. Skipping.");
                continue;
            }

            _logger.LogInfo(
                $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}/{pipeline.Count}: " +
                $"Executing '{runtimeAction.Type}'…");

            try
            {
                // Runner owns thread dispatch
                // Task.Run ensures CPU-bound executors never block the caller.
                // For I/O-async executors (RunCommandExecutor) the overhead is
                // negligible — the thread is released at the first real await.
                await Task
                    .Run(
                        async () => await executor
                            .ExecuteAsync(runtimeAction, context, cancellationToken)
                            .ConfigureAwait(false),
                        cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogSuccess(
                    $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}: " +
                    $"'{runtimeAction.Type}' completed successfully.");
            }
            catch (ActionFailedException ex)
            {
                // Executor-level failure: log and continue to the next action
                _logger.LogError(
                    $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}: " +
                    $"'{runtimeAction.Type}' failed — {ex.Message}", ex);
            }
            catch (OperationCanceledException)
            {
                // Propagate immediately — do not swallow pipeline cancellation
                _logger.LogWarning(
                    $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}: " +
                    $"'{runtimeAction.Type}' was cancelled. Aborting pipeline.");
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected executor crash: log and continue so one broken action
                // cannot silently kill the rest of the routine
                _logger.LogError(
                    $"[{context.RoutineName}] Step {runtimeAction.ExecutionOrder}: " +
                    $"Unexpected error in '{runtimeAction.Type}' — {ex.Message}", ex);
            }
        }

        _logger.LogSuccess(
            $"[{context.RoutineName}] Pipeline completed — all steps processed.");
    }

    // Action map builder 

    private static IReadOnlyDictionary<ActionType, IAction> BuildActionMap(IEnumerable<IAction> actions)
    {
        var map = new Dictionary<ActionType, IAction>();

        foreach (var action in actions)
        {
            foreach (var type in action.SupportedActionTypes)
            {
                if (!map.TryAdd(type, action))
                {
                    throw new InvalidOperationException(
                        $"Duplicate executor registration detected for action type '{type}'. " +
                        $"Each ActionType must have exactly one registered executor.");
                }
            }
        }

        return new ReadOnlyDictionary<ActionType, IAction>(map);
    }
}
