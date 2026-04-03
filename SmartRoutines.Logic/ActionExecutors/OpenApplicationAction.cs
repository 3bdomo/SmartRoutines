using System.Diagnostics;
using SmartRoutines.Core.Enums;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces;
using SmartRoutines.Core.Models;

namespace SmartRoutines.Logic.ActionExecutors;

public class OpenApplicationAction : IAction
{
    private string ApplicationPath { get; set; }
    private string Arguments { get; set; }
    /// <summary>
    ///  If user want to make the app running even the smartroutines is closed, set this to true.
    /// It will start the app in a detached way using cmd.exe, so that it won't be killed when the main process exits. .
    /// </summary>
    private bool ForceDetached { get; set; }
    public LogStatus Status { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public ActionType ActionType { get; }
   

    public OpenApplicationAction(string applicationPath, string arguments = "", bool forceDetached = false)
    {
        ApplicationPath = applicationPath;
        Arguments = arguments;
        ForceDetached = forceDetached;
        ActionType = ActionType.LaunchApp;
    }

    public OpenApplicationAction(string applicationPath)
    {
        ApplicationPath = applicationPath;
        Arguments = string.Empty;
        ForceDetached = false;
        ActionType = ActionType.LaunchApp;
    }

    public void Execute(ActionContext context)
    {
        try
        {
            string normalizedPath = NormalizePath(ApplicationPath);

            if (ForceDetached)
            {
                StartDetached(normalizedPath, Arguments ?? string.Empty);
            }
            else
            {
                var psi = new ProcessStartInfo
                {
                    FileName = normalizedPath,
                    Arguments = Arguments ?? string.Empty,
                    UseShellExecute = true,
                    WorkingDirectory = ResolveWorkingDirectory(normalizedPath)
                };

                var process = Process.Start(psi);
                if (process is null)
                {
                    throw new InvalidOperationException("Process start returned null.");
                }
            }

            Status = LogStatus.Success;
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            Status = LogStatus.Error;
            ErrorMessage = $"Failed to open application: {ex.Message}";
            throw new ActionFailedException(ActionType,ErrorMessage, ex);
        }
    }
/// <summary>
/// Starts an application in a detached process using cmd.exe, allowing it to continue running
/// independently of the parent process.
/// </summary>
/// <param name="executablePath">The full path to the executable to run.</param>
/// <param name="arguments">Optional command\-line arguments to pass to the executable.</param>
/// <remarks>
/// This method uses cmd.exe with the "start" command to detach the process. The application
/// will run in a hidden window and continue executing even after the parent process terminates.
/// </remarks>
/// <exception cref="InvalidOperationException">Thrown when the detached process fails to start.</exception>
    private static void StartDetached(string executablePath, string arguments)
    {
        var detachedArgs = $"/c start \"\" \"{executablePath}\"";
        if (!string.IsNullOrWhiteSpace(arguments))
        {
            detachedArgs = $"{detachedArgs} {arguments}";
        }

        var cmdPsi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = detachedArgs,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = ResolveWorkingDirectory(executablePath)
        };

        var shellProcess = Process.Start(cmdPsi);
        if (shellProcess is null)
        {
            throw new InvalidOperationException("Detached process start returned null.");
        }
    }

    private static string NormalizePath(string path)
    {
        return path.Trim().Trim('"');
    }

    private static string ResolveWorkingDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        return string.IsNullOrWhiteSpace(directory) ? Environment.CurrentDirectory : directory;
    }
}