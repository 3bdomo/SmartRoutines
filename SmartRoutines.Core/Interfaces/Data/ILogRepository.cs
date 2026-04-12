using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Core.Interfaces.Data
{
    public interface ILogRepository
    {
        Task<List<ActivityLog>> GetAllAsync();
        Task<List<ActivityLog>> GetByRoutineIdAsync(Guid routineId);
        Task AddAsync(ActivityLog log);
        Task DeleteAsync(Guid id);
    }
}
