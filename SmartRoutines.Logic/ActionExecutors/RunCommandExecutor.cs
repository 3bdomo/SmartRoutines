using System.Diagnostics;
using System.Text.Json;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.ActionExecutors;

/// <summary>
/// Executes PowerShell commands for routine actions.
/// </summary>
public sealed class RunCommandExecutor : IAction
{
    private const int DefaultTimeoutSeconds = 120;

    /// <inheritdoc />
    public IReadOnlyCollection<ActionType> SupportedActionTypes { get; } = [ActionType.RunCommand];

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

            if (entry.Type != ActionType.RunCommand)
            {
                throw new ArgumentException("Action entry type must be RunCommand.", nameof(entry));
            }

            var request = ParseRequest(entry.Arguments);
            await ExecutePowerShellAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var actionType = entry?.Type ?? ActionType.Unknown;
            var commandLabel = entry?.Arguments ?? "<null>";
            throw new ActionFailedException(actionType, $"Failed to run command '{commandLabel}': {ex.Message}", ex);
        }
    }

    private static CommandRequest ParseRequest(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            throw new ArgumentException("RunCommand requires a non-empty command payload.", nameof(arguments));
        }

        var payload = arguments.Trim();
        if (!payload.StartsWith("{", StringComparison.Ordinal))
        {
            return new CommandRequest(payload, null, DefaultTimeoutSeconds);
        }

        var parsed = JsonSerializer.Deserialize<CommandPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.Command))
        {
            throw new ArgumentException("RunCommand JSON payload must contain 'Command'.", nameof(arguments));
        }

        var timeout = parsed.TimeoutSeconds.GetValueOrDefault(DefaultTimeoutSeconds);
        if (timeout < 1 || timeout > 3600)
        {
            throw new ArgumentOutOfRangeException(nameof(arguments), "TimeoutSeconds must be between 1 and 3600.");
        }

        if (!string.IsNullOrWhiteSpace(parsed.WorkingDirectory) && !Directory.Exists(parsed.WorkingDirectory))
        {
            throw new DirectoryNotFoundException($"Working directory does not exist: {parsed.WorkingDirectory}");
        }

        return new CommandRequest(parsed.Command, parsed.WorkingDirectory, timeout);
    }

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

        var process = new Process();
        process.StartInfo = startInfo;
        using (process)
        {
            if (!process.Start())
            {
                throw new InvalidOperationException("Process start returned false.");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(request.TimeoutSeconds));

            try
            {
                await process.WaitForExitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                TryKillProcess(process);
                throw new TimeoutException($"Command exceeded timeout of {request.TimeoutSeconds} seconds.");
            }
            catch (OperationCanceledException)
            {
                TryKillProcess(process);
                throw;
            }

            await outputTask;
            var standardError = await errorTask;

            if (process.ExitCode != 0)
            {
                var errorText = string.IsNullOrWhiteSpace(standardError) ? "No error output." : standardError.Trim();
                throw new InvalidOperationException($"PowerShell exited with code {process.ExitCode}. {errorText}");
            }
        }
    }

    private static void TryKillProcess(Process process)
    {
        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
        }
    }

    private sealed record CommandRequest(string Command, string? WorkingDirectory, int TimeoutSeconds);

    private sealed record CommandPayload(string Command, string? WorkingDirectory, int? TimeoutSeconds);
}

