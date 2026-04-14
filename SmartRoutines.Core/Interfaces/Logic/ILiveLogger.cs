using System;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Lightweight real-time logger used by the engine to stream events to the UI.
/// </summary>
public interface ILiveLogger
{
    void LogInfo(string message);
    void LogSuccess(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? ex = null);
}
