using System;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Core.Events;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Adapter that forwards live logs to the ActivityLog repository and raises engine events if required.
/// This implementation is lightweight and intended for DI into the engine in UI scenarios.
/// </summary>
public sealed class LiveLogger : ILiveLogger
{
    private readonly IActivityLogRepository _repo;

    public LiveLogger(IActivityLogRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public void LogInfo(string message) => EnqueueLog(LogStatus.Unknown, message);
    public void LogSuccess(string message) => EnqueueLog(LogStatus.Success, message);
    public void LogWarning(string message) => EnqueueLog(LogStatus.Warning, message);
    public void LogError(string message, Exception? ex = null) => EnqueueLog(LogStatus.Error, message + (ex != null ? " - " + ex.Message : string.Empty));

    private void EnqueueLog(LogStatus status, string message)
    {
        var log = new ActivityLog(Guid.Empty, "System", status, message ?? string.Empty);
        // Fire-and-forget persistence for UI live logging; repository mock is synchronous-friendly.
        _ = _repo.AddAsync(log);
    }
}
