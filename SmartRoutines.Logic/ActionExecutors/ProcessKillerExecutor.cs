using System.Diagnostics;
using System.Text.Json;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes process termination actions by process name.
/// </summary>
public sealed class ProcessKillerExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.KillProcess];

    /// <inheritdoc />
    public async Task ExecuteAsync(ActionEntry entry, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            _ = context ?? throw new ArgumentNullException(nameof(context));

            var processName = ParseProcessName(entry.Arguments);
            await Task.Run(() => KillProcesses(processName, cancellationToken), cancellationToken);
        }
        catch (Exception ex)
        {
            var actionType = entry?.Type ?? ActionType.Unknown;
            var processLabel = entry?.Arguments ?? "<null>";
            throw new ActionFailedException(actionType, $"Failed to kill process '{processLabel}': {ex.Message}", ex);
        }
    }

    private static string ParseProcessName(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            throw new ArgumentException("KillProcess requires a process name.", nameof(arguments));
        }

        var payload = arguments.Trim();
        if (!payload.StartsWith("{", StringComparison.Ordinal))
        {
            return NormalizeProcessName(payload);
        }

        var parsed = JsonSerializer.Deserialize<ProcessPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.ProcessName))
        {
            throw new ArgumentException("KillProcess JSON payload must contain 'ProcessName'.", nameof(arguments));
        }

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
            cancellationToken.ThrowIfCancellationRequested();

            using (process)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }
        }
    }

    private sealed record ProcessPayload(string ProcessName);
}


