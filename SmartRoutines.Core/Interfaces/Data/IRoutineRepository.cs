using SmartRoutines.Core.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartRoutines.Core.Interfaces.Data
{
    /// <summary>
    /// Repository for <see cref="Routine"/> entities with domain-specific operations.
    /// Inherits generic CRUD from <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IRoutineRepository : IGenericRepository<Routine>
    {
        /// <summary>
        /// Retrieve a routine and include related actions.
        /// </summary>
        Task<Routine?> GetByIdWithActionsAsync(Guid id);

        /// <summary>
        /// Returns active, not-deleted routines with their actions eagerly loaded.
        /// </summary>
        Task<IEnumerable<Routine>> GetActiveNotDeletedWithActionsAsync();

        /// <summary>
        /// Checks whether a routine name is unique within the store.
        /// </summary>
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    }
}
