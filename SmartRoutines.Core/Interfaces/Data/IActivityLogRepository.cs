using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.Interfaces.Data
{
    public interface IActivityLogRepository
    {
        Task<ActivityLog?> GetByIdAsync(Guid id);
        Task<List<ActivityLog>> GetAllAsync();
        Task<IEnumerable<ActivityLog>> GetPagedAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null);
        Task<int> GetTotalCountAsync(LogStatus? statusFilter = null);
        Task<ActivityLog?> GetLastForRoutineAsync(Guid routineId);
        Task AddAsync(ActivityLog log);
        Task ClearAllAsync();
    }
}
