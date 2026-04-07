using SmartRoutines.Core.Models;

namespace SmartRoutines.Core.Interfaces.Data;

/// <summary>
/// Defines the contract for an asynchronous data repository managing Routine persistence.
/// </summary>
public interface IRoutineRepository
{
    /// <summary>
    /// Retrieves all routines asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all routines.</returns>
    Task<List<Routine>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific routine by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the routine to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the routine if found; otherwise, null.</returns>
    Task<Routine?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new routine to the repository asynchronously.
    /// </summary>
    /// <param name="routine">The routine to add.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(Routine routine);

    /// <summary>
    /// Updates an existing routine in the repository asynchronously.
    /// </summary>
    /// <param name="routine">The routine to update.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(Routine routine);

    /// <summary>
    /// Deletes a routine from the repository by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the routine to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid id);
}
