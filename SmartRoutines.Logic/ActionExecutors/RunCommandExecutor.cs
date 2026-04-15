using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;
using System.Diagnostics;
using System.Text.Json;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes PowerShell commands for routine actions.
/// </summary>
/// <remarks>
/// This executor is genuinely asynchronous: it uses <see cref="Process.WaitForExitAsync"/>
/// to await process completion without blocking a thread. No <c>Task.Run</c> wrapper is
/// needed here; the <c>ActionRunner</c>'s universal <c>Task.Run</c> dispatch is harmless
/// since the async continuation unblocks the thread immediately.
/// </remarks>
public sealed class RunCommandExecutor : IAction
{
    private const int DefaultTimeoutSeconds = 120;

    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.RunCommand];

    /// <inheritdoc />
    public async Task ExecuteAsync(RuntimeAction action, ActionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        if (action.Type != ActionType.RunCommand)
            throw new ArgumentException("Action type must be RunCommand.", nameof(action));

        try
        {
            var request = ParseRequest(action.Arguments);
            await ExecutePowerShellAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw; // Surface cancellation cleanly to ActionRunner
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(
                action.Type,
                $"Failed to run command '{action.Arguments}': {ex.Message}",
                ex);
        }
    }

    // ── Argument parser ──────────────────────────────────────────────────────

    private static CommandRequest ParseRequest(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
            throw new ArgumentException("RunCommand requires a non-empty command payload.", nameof(arguments));

        var payload = arguments.Trim();

        // Plain command — no JSON wrapper
        if (!payload.StartsWith("{", StringComparison.Ordinal))
            return new CommandRequest(payload, null, DefaultTimeoutSeconds);

        var parsed = JsonSerializer.Deserialize<CommandPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Command))
            throw new ArgumentException("RunCommand JSON payload must contain 'Command'.", nameof(arguments));

        var timeout = parsed.TimeoutSeconds.GetValueOrDefault(DefaultTimeoutSeconds);
        if (timeout is < 1 or > 3600)
            throw new ArgumentOutOfRangeException(nameof(arguments), "TimeoutSeconds must be between 1 and 3600.");

        if (!string.IsNullOrWhiteSpace(parsed.WorkingDirectory) && !Directory.Exists(parsed.WorkingDirectory))
            throw new DirectoryNotFoundException($"Working directory does not exist: {parsed.WorkingDirectory}");

        return new CommandRequest(parsed.Command, parsed.WorkingDirectory, timeout);
    }

    // ── PowerShell runner ────────────────────────────────────────────────────

    private static async Task ExecutePowerShellAsync(CommandRequest request, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = string.IsNullOrWhiteSpace(request.WorkingDirectory)
                                         ? Environment.CurrentDirectory
                                         : request.WorkingDirectory
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(request.Command);

        using var process = new Process { StartInfo = startInfo };

        if (!process.Start())
            throw new InvalidOperationException("Process.Start returned false.");

        // Read streams concurrently to prevent pipe-buffer deadlock
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        // Link a per-command timeout with the upstream cancellation token
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(request.TimeoutSeconds));

        try
        {
            await process.WaitForExitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Timeout expired — kill the child process and surface a clean exception
            TryKillProcess(process);
            throw new TimeoutException($"Command exceeded timeout of {request.TimeoutSeconds} seconds.");
        }
        catch (OperationCanceledException)
        {
            // Upstream cancellation — kill the child process and re-throw
            TryKillProcess(process);
            throw;
        }

        await outputTask.ConfigureAwait(false);
        var standardError = await errorTask.ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            var errorText = string.IsNullOrWhiteSpace(standardError) ? "No error output." : standardError.Trim();
            throw new InvalidOperationException($"PowerShell exited with code {process.ExitCode}. {errorText}");
        }
    }

    private static void TryKillProcess(Process process)
    {
        if (!process.HasExited)
            process.Kill(entireProcessTree: true);
    }

    // ── Private records ──────────────────────────────────────────────────────

    private sealed record CommandRequest(string Command, string? WorkingDirectory, int TimeoutSeconds);
    private sealed record CommandPayload(string Command, string? WorkingDirectory, int? TimeoutSeconds);
}
