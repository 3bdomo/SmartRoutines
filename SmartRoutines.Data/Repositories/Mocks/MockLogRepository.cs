using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
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

        public Task ClearAllAsync()
        {
            _logs.Clear();
            return Task.CompletedTask;
        }

        public void Delete(ActivityLog log)
        {
            if (log != null)
            {
                log.SoftDelete();
            }
        }

        public void Update(ActivityLog log)
        {
            if (log == null) return;
            var idx = _logs.FindIndex(l => l.Id == log.Id);
            if (idx >= 0)
            {
                // Replace the existing entry with the provided one. ActivityLog is effectively immutable
                // so swapping the reference preserves historical semantics for the mock repository.
                _logs[idx] = log;
            }
        }

        public Task<IReadOnlyList<ActivityLog>> GetAllAsync()
        {
            return Task.FromResult((IReadOnlyList<ActivityLog>)_logs.Where(l => !l.IsDeleted).OrderByDescending(l => l.CreatedAt).ToList());
        }

        public Task<ActivityLog?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_logs.FirstOrDefault(l => l.Id == id && !l.IsDeleted));
        }

        public Task<List<ActivityLog>> GetByRoutineIdAsync(Guid routineId)
        {
            return Task.FromResult(_logs.Where(l => l.RoutineId == routineId && !l.IsDeleted).OrderByDescending(l => l.CreatedAt).ToList());
        }

        public Task<ActivityLog?> GetLastForRoutineAsync(Guid routineId)
        {
            var last = _logs.Where(l => l.RoutineId == routineId && !l.IsDeleted).OrderByDescending(l => l.CreatedAt).FirstOrDefault();
            return Task.FromResult(last);
        }

        public Task<IEnumerable<ActivityLog>> GetPagedAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null)
        {
            var query = _logs.Where(l => !l.IsDeleted);
            if (statusFilter != null) query = query.Where(l => l.Status == statusFilter.Value);
            if (routineId != null) query = query.Where(l => l.RoutineId == routineId.Value);
            var result = query.OrderByDescending(l => l.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<int> GetTotalCountAsync(LogStatus? statusFilter = null)
        {
            var query = _logs.Where(l => !l.IsDeleted);
            if (statusFilter != null) query = query.Where(l => l.Status == statusFilter.Value);
            return Task.FromResult(query.Count());
        }
    }
}
