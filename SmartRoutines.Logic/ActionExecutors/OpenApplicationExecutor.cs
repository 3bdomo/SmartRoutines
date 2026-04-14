using SmartRoutines.Core.Domain.Entities;
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
public class LaunchAppExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } =
    [
        ActionType.LaunchApp
    ];

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

            var launchRequest = ParseLaunchRequest(entry);
            var normalizedTarget = NormalizePath(launchRequest.ApplicationPath);

            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = normalizedTarget,
                    Arguments = launchRequest.Arguments,
                    UseShellExecute = true,
                    WorkingDirectory = ResolveWorkingDirectory(normalizedTarget)
                };

                var process = Process.Start(psi);
                if (process is null)
                {
                    throw new InvalidOperationException("Process start returned null.");
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            var actionType = entry?.Type ?? ActionType.Unknown;
            var targetLabel = entry?.Arguments ?? "<null>";
            throw new ActionFailedException(actionType, $"Failed to start target '{targetLabel}': {ex.Message}", ex);
        }
    }

    private static string NormalizePath(string pathOrUrl)
    {
        return pathOrUrl.Trim().Trim('"');
    }

    private static LaunchRequest ParseLaunchRequest(ActionEntry entry)
    {
        if (entry is null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (entry.Type != ActionType.LaunchApp)
        {
            throw new ArgumentException("Action entry type must be LaunchApp.", nameof(entry));
        }

        if (string.IsNullOrWhiteSpace(entry.Arguments))
        {
            throw new ArgumentException("LaunchApp requires a non-empty argument payload.", nameof(entry));
        }

        var payload = entry.Arguments.Trim();

        if (!payload.StartsWith("{", StringComparison.Ordinal))
        {
            return new LaunchRequest(payload, string.Empty);
        }

        var parsed = JsonSerializer.Deserialize<LaunchRequestPayload>(payload);
        if (parsed is null)
        {
            throw new ArgumentException("LaunchApp JSON payload is invalid.", nameof(entry));
        }

        if (string.IsNullOrWhiteSpace(parsed.ApplicationPath))
        {
            throw new ArgumentException("LaunchApp JSON payload must contain 'ApplicationPath'.", nameof(entry));
        }

        return new LaunchRequest(parsed.ApplicationPath, parsed.Arguments ?? string.Empty);
    }

    private static string ResolveWorkingDirectory(string target)
    {
        var directory = Path.GetDirectoryName(target);
        return string.IsNullOrWhiteSpace(directory) ? Environment.CurrentDirectory : directory;
    }

    private sealed record LaunchRequest(string ApplicationPath, string Arguments);

    private sealed class LaunchRequestPayload
    {
        public string ApplicationPath { get; init; } = string.Empty;
        public string? Arguments { get; init; }
    }
}

/// <summary>
/// Backward-compatible alias for older registrations that still reference OpenApplicationAction.
/// </summary>
public sealed class OpenApplicationExecutor : LaunchAppExecutor
{
}

