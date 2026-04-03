using SmartRoutines.Core.Enums;

namespace SmartRoutines.Core.Models
{
    /// <summary>
    /// Represents an immutable historical record of a routine's execution attempt or system event.
    /// </summary>
    /// <remarks>
    /// This entity utilizes strategic denormalization by explicitly storing the <see cref="RoutineName"/>. 
    /// This ensures that if a parent <see cref="Routine"/> is permanently deleted from the database, 
    /// the historical audit logs remain legible and intact without throwing foreign key violations 
    /// or displaying empty names.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Logging a successful execution
    /// var successLog = new ActivityLog(routineId, "Morning Work", LogStatus.Success, "Chrome and VS Code launched successfully.");
    /// 
    /// // Logging a failure
    /// var errorLog = new ActivityLog(routineId, "Gaming Mode", LogStatus.Error, "Failed to launch Steam.exe: File not found.");
    /// </code>
    /// </example>
    public class ActivityLog : BaseEntity
    {
        /// <summary>
        /// Gets the unique identifier of the routine that triggered this log.
        /// </summary>
        /// <value>A <see cref="Guid"/> representing the associated routine.</value>
        /// <remarks>
        /// This is treated as a loose reference rather than a strict Foreign Key constraint 
        /// to prevent cascading deletes from destroying historical logs.
        /// </remarks>
        public Guid RoutineId { get; private set; }

        /// <summary>
        /// Gets the display name of the routine captured exactly at the moment of execution.
        /// </summary>
        /// <value>A string up to 100 characters.</value>
        //[Required]
        //[MaxLength(100)]
        public string RoutineName { get; private set; }

        /// <summary>
        /// Gets the final outcome of the routine's execution.
        /// </summary>
        /// <value>A <see cref="LogStatus"/> enumeration (e.g., Success, Warning, Error).</value>
        public LogStatus Status { get; private set; }

        /// <summary>
        /// Gets detailed diagnostic information, success messages, or exception stack traces.
        /// </summary>
        /// <value>A string up to 500 characters.</value>
        //[MaxLength(500)]
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new, immutable instance of the <see cref="ActivityLog"/> class.
        /// </summary>
        /// <param name="routineId">The ID of the routine being executed.</param>
        /// <param name="routineName">The name of the routine at the time of execution.</param>
        /// <param name="status">The success or failure state.</param>
        /// <param name="message">Contextual details about the execution.</param>
        /// <exception cref="ArgumentException">Thrown if the routine name is null or whitespace.</exception>
        public ActivityLog(Guid routineId, string routineName, LogStatus status, string message)
        {
            if (string.IsNullOrWhiteSpace(routineName))
                throw new ArgumentException("Routine name must be provided for historical logging.", nameof(routineName));

            RoutineId = routineId;
            RoutineName = routineName;
            Status = status;
            Message = message ?? string.Empty; // Prevents null messages in the database
        }
    }
}
