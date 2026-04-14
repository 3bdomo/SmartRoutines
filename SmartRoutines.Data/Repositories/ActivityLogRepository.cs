using Microsoft.EntityFrameworkCore;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;

namespace SmartRoutines.Data.Repositories
{
    internal class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        public ActivityLogRepository(SmartRoutinesDbContext ctx) : base(ctx)
        {
        }
        /// <summary>
        /// Deletes all logs using a set-based operation for performance.
        /// </summary>
        public async Task ClearAllAsync()
        {
            // Use ExecuteDeleteAsync for high performance set-based delete
            await _dbSet.ExecuteDeleteAsync();
        }

        /// <summary>
        /// Retrieves the most recent log for a specific routine.
        /// </summary>
        public async Task<ActivityLog?> GetLastForRoutineAsync(Guid routineId)
        {
            return await _dbSet
                .Where(l => l.RoutineId == routineId)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns a page of logs. Uses AsNoTracking for read-only performance.
        /// </summary>
        public async Task<IEnumerable<ActivityLog>> GetPagedAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (statusFilter != null) query = query.Where(l => l.Status == statusFilter.Value);
            if (routineId != null) query = query.Where(l => l.RoutineId == routineId.Value);

            return await query.OrderByDescending(l => l.CreatedAt)
                               .Skip((Math.Max(1, page) - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }

        /// <summary>
        /// Returns the total count of logs optionally filtered by status.
        /// </summary>
        public async Task<int> GetTotalCountAsync(LogStatus? statusFilter = null)
        {
            var query = _dbSet.AsQueryable();
            if (statusFilter != null) query = query.Where(l => l.Status == statusFilter.Value);
            return await query.CountAsync();
        }
    }
}
