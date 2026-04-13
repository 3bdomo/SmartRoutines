using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Core.Interfaces.Data
{
    /// <summary>
    /// Generic repository contract exposing common CRUD operations for an entity type.
    /// Implementations should provide lightweight, persistence-agnostic behavior.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>The entity if found; otherwise null.</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns all entities as a read-only list.
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity (in-memory change; persistence occurs when unit of work is saved).
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Marks an entity for deletion.
        /// </summary>
        void Delete(T entity);
    }
}
