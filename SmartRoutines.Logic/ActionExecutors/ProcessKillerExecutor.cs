using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Diagnostics;
using System.Text.Json;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes process termination actions by process name.
/// </summary>
/// <remarks>
/// <see cref="Process.Kill"/> and <see cref="Process.WaitForExit(int)"/> are
/// CPU-bound synchronous operations. This executor returns <see cref="Task.CompletedTask"/>
/// so the <c>ActionRunner</c> can dispatch it on the thread pool via <c>Task.Run</c>,
/// preventing it from blocking the calling thread.
/// </remarks>
public sealed class ProcessKillerExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.KillProcess];

    /// <inheritdoc />
    public Task ExecuteAsync(RuntimeAction action, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var processName = ParseProcessName(action.Arguments);
            KillProcesses(processName, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw; // Let ActionRunner handle cancellation logging
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(
                action.Type,
                $"Failed to kill process '{action.Arguments}': {ex.Message}",
                ex);
        }

        return Task.CompletedTask;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string ParseProcessName(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
            throw new ArgumentException("KillProcess requires a non-empty process name.", nameof(arguments));

        var payload = arguments.Trim();

        if (!payload.StartsWith("{", StringComparison.Ordinal))
            return NormalizeProcessName(payload);

        var parsed = JsonSerializer.Deserialize<ProcessPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.ProcessName))
            throw new ArgumentException("KillProcess JSON payload must contain 'ProcessName'.", nameof(arguments));

        return NormalizeProcessName(parsed.ProcessName);
    }

    private static string NormalizeProcessName(string processName)
    {
        var cleaned = processName.Trim().Trim('"');
        return cleaned.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            ? cleaned[..^4]
            : cleaned;
    }

    private static void KillProcesses(string processName, CancellationToken cancellationToken)
    {
        var processes = Process.GetProcessesByName(processName);
        foreach (var process in processes)
        {
            // Honour cancellation between each kill — avoids hanging on a long process list
            cancellationToken.ThrowIfCancellationRequested();

            using (process)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }
        }
    }

    // ── Private records ──────────────────────────────────────────────────────

    private sealed record ProcessPayload(string ProcessName);
}
