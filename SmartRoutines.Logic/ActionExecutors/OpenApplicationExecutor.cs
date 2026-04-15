using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Diagnostics;
using System.Text.Json;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes application launch actions.
/// </summary>
/// <remarks>
/// <see cref="Process.Start"/> returns immediately after handing the launch to the OS shell;
/// it does not block for the process to exit. This executor is therefore effectively
/// synchronous and returns <see cref="Task.CompletedTask"/>. The <c>ActionRunner</c>
/// is responsible for dispatching all executors on a thread-pool thread.
/// </remarks>
public class LaunchAppExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } =
    [
        ActionType.LaunchApp
    ];

    /// <inheritdoc />
    public Task ExecuteAsync(RuntimeAction action, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var launchRequest = ParseLaunchRequest(action);
            var normalizedTarget = NormalizePath(launchRequest.ApplicationPath);

            var psi = new ProcessStartInfo
            {
                FileName = normalizedTarget,
                Arguments = launchRequest.Arguments,
                UseShellExecute = true,
                WorkingDirectory = ResolveWorkingDirectory(normalizedTarget)
            };

            var process = Process.Start(psi);
            if (process is null)
                throw new InvalidOperationException("Process.Start returned null — the OS could not launch the target.");
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(
                action.Type,
                $"Failed to start target '{action.Arguments}': {ex.Message}",
                ex);
        }

        return Task.CompletedTask;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string NormalizePath(string pathOrUrl) => pathOrUrl.Trim().Trim('"');

    private static LaunchRequest ParseLaunchRequest(RuntimeAction action)
    {
        if (action.Type != ActionType.LaunchApp)
            throw new ArgumentException("Action type must be LaunchApp.", nameof(action));

        if (string.IsNullOrWhiteSpace(action.Arguments))
            throw new ArgumentException("LaunchApp requires a non-empty argument payload.", nameof(action));

        var payload = action.Arguments.Trim();

        // Plain path — no JSON wrapper
        if (!payload.StartsWith("{", StringComparison.Ordinal))
            return new LaunchRequest(payload, string.Empty);

        var parsed = JsonSerializer.Deserialize<LaunchRequestPayload>(payload);

        if (parsed is null || string.IsNullOrWhiteSpace(parsed.ApplicationPath))
            throw new ArgumentException(
                "LaunchApp JSON payload must contain a non-empty 'ApplicationPath'.",
                nameof(action));

        return new LaunchRequest(parsed.ApplicationPath, parsed.Arguments ?? string.Empty);
    }

    private static string ResolveWorkingDirectory(string target)
    {
        var directory = Path.GetDirectoryName(target);
        return string.IsNullOrWhiteSpace(directory) ? Environment.CurrentDirectory : directory;
    }

    // ── Private records ──────────────────────────────────────────────────────

    private sealed record LaunchRequest(string ApplicationPath, string Arguments);

    private sealed class LaunchRequestPayload
    {
        public string ApplicationPath { get; init; } = string.Empty;
        public string? Arguments { get; init; }
    }
}

/// <summary>
/// Backward-compatible alias for older registrations that still reference OpenApplicationExecutor.
/// </summary>
public sealed class OpenApplicationExecutor : LaunchAppExecutor { }
