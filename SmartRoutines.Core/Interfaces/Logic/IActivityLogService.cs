using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Domain.Enums;

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
    /// Clears all logs from the system.
    /// </summary>
    Task ClearAllLogsAsync();

    /// <summary>
    /// Adds a new log to the system.
    /// </summary>
    Task AddLogAsync(ActivityLogDto logDto);
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
