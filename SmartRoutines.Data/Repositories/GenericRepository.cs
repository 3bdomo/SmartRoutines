using Microsoft.EntityFrameworkCore;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;

namespace SmartRoutines.Data.Repositories
{
    /// <summary>
    /// Generic EF Core repository providing basic CRUD operations.
    /// Methods are virtual so specialized repositories can override behavior.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly SmartRoutinesDbContext _ctx;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(SmartRoutinesDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = _ctx.Set<T>();
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            var list = await _dbSet.ToListAsync();
            return list;
        }

        /// <inheritdoc />
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <inheritdoc />
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc />
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
