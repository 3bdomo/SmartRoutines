using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.DTOs;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Historical activity log access service.
/// </summary>
public interface IActivityLogService
{
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
}
