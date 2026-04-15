using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Diagnostics;
using System.Text.Json;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes URL open actions using the OS default browser via the shell.
/// </summary>
/// <remarks>
/// <see cref="Process.Start"/> with <c>UseShellExecute = true</c> delegates immediately
/// to the OS; it does not block waiting for the browser to load the page.
/// This executor is therefore synchronous and returns <see cref="Task.CompletedTask"/>.
/// The <c>ActionRunner</c> is responsible for dispatching it on a thread-pool thread.
/// </remarks>
public sealed class OpenUrlExecutor : IAction
{
    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.OpenUrl];

    /// <inheritdoc />
    public Task ExecuteAsync(RuntimeAction action, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var url = ParseUrl(action);

            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            var process = Process.Start(psi);
            if (process is null)
                throw new InvalidOperationException("Process.Start returned null — the OS could not open the URL.");
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(
                action.Type,
                $"Failed to open URL '{action.Arguments}': {ex.Message}",
                ex);
        }

        return Task.CompletedTask;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string ParseUrl(RuntimeAction action)
    {
        if (action.Type != ActionType.OpenUrl)
            throw new ArgumentException("Action type must be OpenUrl.", nameof(action));

        if (string.IsNullOrWhiteSpace(action.Arguments))
            throw new ArgumentException("OpenUrl requires a non-empty argument payload.", nameof(action));

        var payload = action.Arguments.Trim();

        // Plain URL — no JSON wrapper
        if (!payload.StartsWith("{", StringComparison.Ordinal))
            return ValidateUrl(payload);

        var parsed = JsonSerializer.Deserialize<OpenUrlPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Url))
            throw new ArgumentException("OpenUrl JSON payload must contain a non-empty 'Url'.", nameof(action));

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

    // ── Private records ──────────────────────────────────────────────────────

    private sealed record OpenUrlPayload(string Url);
}
