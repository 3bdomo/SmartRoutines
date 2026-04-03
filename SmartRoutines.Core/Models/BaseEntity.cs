using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.Core.Models
{
    /// <summary>
    /// Serves as the base class for all domain entities, providing essential tracking and 
    /// lifecycle management properties.
    /// </summary>
    /// <remarks>
    /// This class implements the "Soft Delete" pattern and "Audit Logging" fields 
    /// required for enterprise-grade data persistence. 
    /// Using <see cref="Guid"/> for IDs ensures global uniqueness across distributed systems.
    /// </remarks>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets the globally unique identifier for the entity.
        /// </summary>
        /// <value>A <see cref="Guid"/> that serves as the Primary Key in the database.</value>
        //[Key]
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets the date and time when the entity was first persisted.
        /// </summary>
        /// <value>The <see cref="DateTime"/> in UTC format.</value>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Gets the date and time of the most recent modification.
        /// </summary>
        /// <value>A nullable <see cref="DateTime"/> in UTC; returns <c>null</c> if never modified.</value>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the entity has been logically deleted.
        /// </summary>
        /// <remarks>
        /// Setting this to <c>true</c> hides the record from the UI without removing it from the database.
        /// This is a critical safety feature for data recovery and history tracking.
        /// </remarks>
        public bool IsDeleted { get; protected set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Transitions the entity to a deleted state logically.
        /// </summary>
        /// <returns>Void.</returns>
        /// <remarks>
        /// Updates the <see cref="IsDeleted"/> flag and sets the <see cref="UpdatedAt"/> timestamp.
        /// </remarks>
        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
