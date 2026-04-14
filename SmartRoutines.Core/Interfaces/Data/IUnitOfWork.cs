namespace SmartRoutines.Core.Interfaces.Data
{
    /// <summary>
    /// Defines the contract for the Unit of Work pattern, acting as a central gateway 
    /// to manage all repositories and ensure atomic database transactions.
    /// </summary>
    /// <remarks>
    /// The Unit of Work ensures that multiple operations (e.g., adding a Routine and 
    /// logging its creation) are treated as a single unit. If one fails, the entire 
    /// transaction can be rolled back to maintain data integrity.
    /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for managing <see cref="Routine"/> entities.
        /// </summary>
        IRoutineRepository Routines { get; }

        /// <summary>
        /// Gets the repository for managing <see cref="ActivityLog"/> entities.
        /// </summary>
        IActivityLogRepository ActivityLogs { get; }

        /// <summary>
        /// Gets the repository for managing global <see cref="AppSettings"/>.
        /// </summary>
        IAppSettingsRepository AppSettings { get; }

        /// <summary>
        /// Persists all tracked changes within the current context to the database asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// the number of state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Manually initiates a new database transaction.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction, making all changes permanent.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Reverts all changes made within the current transaction if an error occurs.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RollbackTransactionAsync();
    }
}
