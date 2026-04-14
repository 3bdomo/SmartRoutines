using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Domain.Enums;

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
    /// Clears all logs from the system.
    /// </summary>
    Task ClearAllLogsAsync();
}
