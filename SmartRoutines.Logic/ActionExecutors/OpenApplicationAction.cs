using System.Diagnostics;
using System.Text.Json;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces;

namespace SmartRoutines.Logic.ActionExecutors;

public class OpenApplicationAction : IAction
{
    public ActionType ActionType => ActionType.LaunchApp;

    public LogStatus Status { get; }
    public string ErrorMessage { get; }

    public void Execute(ActionEntry entry, ActionContext context)
    {
        try
        {
            var launchRequest = ParseLaunchRequest(entry);
            var normalizedPath = NormalizePath(launchRequest.ApplicationPath);

            var psi = new ProcessStartInfo
            {
                FileName = normalizedPath,
                Arguments = launchRequest.Arguments,
                UseShellExecute = true,
                WorkingDirectory = ResolveWorkingDirectory(normalizedPath)
            };

            var process = Process.Start(psi);
            if (process is null)
            {
                throw new InvalidOperationException("Process start returned null.");
            }
        }
        catch (Exception ex)
        {
            throw new ActionFailedException(ActionType, $"Failed to open application: {ex.Message}", ex);
        }
    }

    private static string NormalizePath(string path)
    {
        return path.Trim().Trim('"');
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
            throw new ArgumentException("LaunchApp action requires a non-empty argument payload.", nameof(entry));
        }
    
        var payload = entry.Arguments.Trim();
    
        if (!payload.StartsWith("{", StringComparison.Ordinal))
        {
            return new LaunchRequest(payload, string.Empty);
        }
    
        var parsed = JsonSerializer.Deserialize<LaunchRequestPayload>(payload);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.ApplicationPath))
        {
            throw new ArgumentException("LaunchApp JSON payload must contain 'ApplicationPath'.", nameof(entry));
        }
    
        return new LaunchRequest(parsed.ApplicationPath, parsed.Arguments ?? string.Empty);
    }

    private static string ResolveWorkingDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        return string.IsNullOrWhiteSpace(directory) ? Environment.CurrentDirectory : directory;
    }
    
    private sealed record LaunchRequest(string ApplicationPath, string Arguments);
    
    private sealed class LaunchRequestPayload
    {
        public string ApplicationPath { get; init; } = string.Empty;
        public string? Arguments { get; init; }
        // // Legacy field kept for payload compatibility; ignored by design.
        // public bool ForceDetached { get; init; }
    }
}