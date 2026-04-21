using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.DTOs;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Historical activity log access service with real-time event notifications.
/// </summary>
public interface IActivityLogService
{
    /// <summary>
    /// Raised when a new log is added to the system.
    /// </summary>
    event EventHandler<ActivityLogEventArgs>? OnLogAdded;

    /// <summary>
    /// Raised when all logs are cleared from the system.
    /// </summary>
    event EventHandler? OnLogsCleared;

    /// <summary>
    /// Returns a page of logs along with the total count.
    /// </summary>
    Task<(IReadOnlyList<ActivityLogDto> Logs, int TotalCount)> GetPagedLogsAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null);

    /// <summary>
    /// Adds a new activity log entry to the system.
    /// </summary>
    /// <param name="logDto">The log data transfer object containing execution details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLogAsync(ActivityLogDto logDto);

    /// <summary>
    /// Clears all logs from the system.
    /// </summary>
    Task ClearAllLogsAsync();

    /// <summary>
    /// Adds a new log to the system.
    /// </summary>
   // Task AddLogAsync(ActivityLogDto logDto);
}

/// <summary>
/// Event arguments for when a log is added.
/// </summary>
public class ActivityLogEventArgs : EventArgs
{
    public ActivityLogEventArgs(ActivityLogDto log)
    {
        Log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public ActivityLogDto Log { get; }
}
