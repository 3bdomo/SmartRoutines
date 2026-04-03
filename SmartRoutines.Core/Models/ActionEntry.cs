using SmartRoutines.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutines.Core.Models
{

    /// <summary>
    /// Represents a discrete, executable operation within a routine's automation sequence.
    /// </summary>
    /// <remarks>
    /// This entity defines the "what" and "how" of an automation step. 
    /// The <see cref="ActionRunner"/> evaluates a collection of these entries, sorting them by 
    /// <see cref="ExecutionOrder"/> before invoking the appropriate executor based on the <see cref="Type"/>.
    /// </remarks>
    /// <example>
    /// Here is how to instantiate different types of actions:
    /// <code>
    /// // Example 1: Launching an application
    /// var openChrome = new ActionEntry(ActionType.LaunchApp, @"C:\Program Files\chrome.exe", 1);
    /// 
    /// // Example 2: Setting volume with a JSON payload
    /// var setVolume = new ActionEntry(ActionType.SetVolume, "{\"Level\": 60}", 2);
    /// </code>
    /// </example>
    public class ActionEntry : BaseEntity
    {
        /// <summary>
        /// Gets the specific category of action to be executed.
        /// </summary>
        /// <value>An <see cref="ActionType"/> enumeration indicating the execution strategy.</value>
        public ActionType Type { get; private set; }

        /// <summary>
        /// Gets the flexible execution arguments required by the action type.
        /// </summary>
        /// <value>A string up to 1000 characters. Can be a raw file path, a URL, or a serialized JSON object.</value>
        /// <remarks>
        /// Using a single string column for polymorphic data prevents database schema bloat.
        /// The specific Executor is responsible for parsing this string correctly.
        /// </remarks>
        //[MaxLength(1000)]
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets the sequential order in which this action should be processed relative to others in the same routine.
        /// </summary>
        /// <value>An integer representing the 1-based execution index.</value>
        public int ExecutionOrder { get; private set; }

        /// <summary>
        /// Gets the Foreign Key identifying the parent routine.
        /// </summary>
        /// <value>A <see cref="Guid"/> matching a <see cref="Routine.Id"/>.</value>
        public Guid RoutineId { get; private set; }

        /// <summary>
        /// Gets the navigation property for the parent routine, allowing Entity Framework Core to perform JOINs automatically.
        /// </summary>
        /// <value>The parent <see cref="Routine"/> object, or <c>null</c> if not explicitly loaded (Lazy/Explicit Loading).</value>
        //[ForeignKey(nameof(RoutineId))]
        public Routine? Routine { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEntry"/> class with strict validation.
        /// </summary>
        /// <param name="type">The type of action to execute.</param>
        /// <param name="arguments">The parameters needed for the action (e.g., file path, URL).</param>
        /// <param name="executionOrder">The order in the execution pipeline (must be greater than 0).</param>
        /// <exception cref="ArgumentNullException">Thrown if the arguments parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the execution order is less than 1.</exception>
        public ActionEntry(ActionType type, string arguments, int executionOrder)
        {
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments), "Action arguments cannot be null. Use an empty string if no arguments are required.");

            if (executionOrder < 1)
                throw new ArgumentOutOfRangeException(nameof(executionOrder), "Execution order must be 1 or greater.");

            Type = type;
            Arguments = arguments;
            ExecutionOrder = executionOrder;
        }
    }
}
