using System.Collections.ObjectModel;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Services;

namespace SmartRoutines.Logic
{
    /// <summary>
    /// Executes a routine action list by selecting the proper strategy for each action type.
    /// </summary>
    public class ActionRunner
    {
        private readonly IReadOnlyDictionary<ActionType, IAction> _actionMap;
        private readonly LoggerService _loggerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRunner"/> class.
        /// </summary>
        /// <param name="actions">The available action executor strategies.</param>
        /// <param name="loggerService">Service used to persist execution results.</param>
        public ActionRunner(IEnumerable<IAction> actions, LoggerService loggerService)
        {
            if (actions is null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
            _actionMap = BuildActionMap(actions);
        }

        /// <summary>
        /// Executes all provided actions in execution-order sequence.
        /// </summary>
        /// <param name="actionEntries">The actions to execute.</param>
        /// <param name="actionContext">The runtime context for this routine run.</param>
        /// <param name="cancellationToken">A token that can cancel execution.</param>
        /// <returns>A task that completes after all actions are attempted.</returns>
        public async Task RunAsync(IEnumerable<ActionEntry> actionEntries, ActionContext actionContext, CancellationToken cancellationToken = default)
        {
            if (actionEntries is null)
            {
                throw new ArgumentNullException(nameof(actionEntries));
            }

            if (actionContext is null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            foreach (var entry in actionEntries.OrderBy(x => x.ExecutionOrder))
            {
                if (!_actionMap.TryGetValue(entry.Type, out var action))
                {
                    await _loggerService.LogActionResultAsync(entry, actionContext, LogStatus.Warning, "No executor is registered for this action type.");
                    continue;
                }

                try
                {
                    await action.ExecuteAsync(entry, actionContext, cancellationToken);
                    await _loggerService.LogActionResultAsync(entry, actionContext, LogStatus.Success, "Action executed successfully.");
                }
                catch (ActionFailedException ex)
                {
                    await _loggerService.LogActionResultAsync(entry, actionContext, LogStatus.Error, ex.Message);
                }
                catch (OperationCanceledException)
                {
                    await _loggerService.LogActionResultAsync(entry, actionContext, LogStatus.Warning, "Action execution canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    await _loggerService.LogActionResultAsync(entry, actionContext, LogStatus.Error, $"Unexpected error: {ex.Message}");
                }
            }
        }

        private static IReadOnlyDictionary<ActionType, IAction> BuildActionMap(IEnumerable<IAction> actions)
        {
            var map = new Dictionary<ActionType, IAction>();

            foreach (var action in actions)
            {
                foreach (var type in action.SupportedActionTypes)
                {
                    if (!map.TryAdd(type, action))
                    {
                        throw new InvalidOperationException($"Duplicate executor registration for action type: {type}.");
                    }
                }
            }

            return new ReadOnlyDictionary<ActionType, IAction>(map);
        }
    }
}
