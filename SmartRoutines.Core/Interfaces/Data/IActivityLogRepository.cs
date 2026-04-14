using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRoutines.Core.Interfaces.Data
{
    /// <summary>
    /// Repository with logging-specific retrieval semantics.
    /// Inherits basic CRUD from <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        Task<IEnumerable<ActivityLog>> GetPagedAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null);
        Task<int> GetTotalCountAsync(LogStatus? statusFilter = null);
        Task<ActivityLog?> GetLastForRoutineAsync(Guid routineId);
        Task ClearAllAsync();
    }
}
