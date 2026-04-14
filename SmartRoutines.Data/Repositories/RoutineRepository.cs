using Microsoft.EntityFrameworkCore;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;

namespace SmartRoutines.Data.Repositories
{
    internal class RoutineRepository : GenericRepository<Routine>, IRoutineRepository
    {
        public RoutineRepository(SmartRoutinesDbContext ctx) : base(ctx)
        {
        }

        public async Task<IEnumerable<Routine>> GetActiveNotDeletedWithActionsAsync()
        {
            return await _dbSet
                .Where(r => r.IsActive)
                .Include(r => r.Actions.OrderBy(a => a.ExecutionOrder))
                .ToListAsync();
        }

        public async Task<Routine?> GetByIdWithActionsAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Actions.OrderBy(a => a.ExecutionOrder))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            var exists = _dbSet.Where(r => r.Name.ToLower() == name.ToLower());

            if (excludeId != null)
            {
                exists = exists.Where(r => r.Id != excludeId.Value);
            }

            return !await exists.AnyAsync();
        }
    }
}
