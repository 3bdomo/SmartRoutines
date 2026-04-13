using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Interfaces.Data;

namespace SmartRoutines.Data.Repositories
{


    public class MockLogRepository : IActivityLogRepository
    {

        private readonly List<ActivityLog> _logs = new();
        /// <inheritdoc/>
        public Task AddAsync(ActivityLog log)
        {
            _logs.Add(log);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var log = _logs.FirstOrDefault(l => l.Id == id);
            if (log != null)
            {
                log.SoftDelete();
            }
            return Task.CompletedTask;
        }

        public Task<List<ActivityLog>> GetAllAsync()
        {
            return Task.FromResult(_logs.Where(l => !l.IsDeleted).OrderByDescending(l => l.CreatedAt).ToList());
        }

        public Task<List<ActivityLog>> GetByRoutineIdAsync(Guid routineId)
        {
            return Task.FromResult(_logs.Where(l => l.RoutineId == routineId && !l.IsDeleted).OrderByDescending(l => l.CreatedAt).ToList());
        }
    }
}
