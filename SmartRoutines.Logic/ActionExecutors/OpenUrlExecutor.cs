using System.Diagnostics;
using System.Text.Json;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes URL open actions using the shell.
/// </summary>
public sealed class OpenUrlExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.OpenUrl];

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

            var url = ParseUrl(entry);
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory
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
            throw new ActionFailedException(actionType, $"Failed to open URL '{targetLabel}': {ex.Message}", ex);
        }
    }

    private static string ParseUrl(ActionEntry entry)
    {
        if (entry.Type != ActionType.OpenUrl)
        {
            throw new ArgumentException("Action entry type must be OpenUrl.", nameof(entry));
        }

        if (string.IsNullOrWhiteSpace(entry.Arguments))
        {
            throw new ArgumentException("OpenUrl requires a non-empty argument payload.", nameof(entry));
        }

        var payload = entry.Arguments.Trim();
        if (!payload.StartsWith("{", StringComparison.Ordinal))
        {
            return ValidateUrl(payload);
        }

        var parsed = JsonSerializer.Deserialize<OpenUrlPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Url))
        {
            throw new ArgumentException("OpenUrl JSON payload must contain 'Url'.", nameof(entry));
        }

        return ValidateUrl(parsed.Url);
    }

    private static string ValidateUrl(string candidate)
    {
        var cleaned = candidate.Trim().Trim('"');
        if (!Uri.TryCreate(cleaned, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("OpenUrl supports only absolute HTTP/HTTPS URLs.", nameof(candidate));
        }

        return cleaned;
    }

    private sealed record OpenUrlPayload(string Url);
}

