using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Adapter that forwards live logs to the ActivityLog repository and raises engine events if required.
/// This implementation is lightweight and intended for DI into the engine in UI scenarios.
/// </summary>
public class LiveLogger : ILiveLogger
{

    public event Action<string, string>? OnLogReceived;

    public void LogInfo(string message) => Publish("Info", message);
    public void LogSuccess(string message) => Publish("Success", message);
    public void LogWarning(string message) => Publish("Warning", message);
    public void LogError(string message, Exception? ex = null)
        => Publish("Error", $"{message} {ex?.Message}");

    private void Publish(string level, string message)
    {
        OnLogReceived?.Invoke(level, $"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
