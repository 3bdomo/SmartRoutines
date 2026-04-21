using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Exceptions;

namespace SmartRoutines.Core.Domain.Entities
{
    /// <summary>
    /// Represents a high-level automation workflow consisting of a trigger and a sequence of actions.
    /// </summary>
    /// <example>
    /// <code>
    /// var routine = new Routine("Morning Work", "work_icon.png", TriggerType.Time, "{\"Time\":\"08:00\"}");
    /// routine.AddAction(new ActionEntry(ActionType.LaunchApp, "chrome.exe", 1));
    /// </code>
    /// </example>
    public class Routine : BaseEntity
    {
        /// <summary>
        /// The display name of the routine.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description associated with the current instance.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// File path or identifier for the routine's visual icon.
        /// </summary>
        public string IconPath { get; private set; }

        /// <summary>
        /// Indicates if the routine is currently enabled for monitoring.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Defines the event type that initiates this routine.
        /// </summary>
        public TriggerType TriggerType { get; private set; }

        /// <summary>
        /// Serialized configuration details for the specific trigger.
        /// </summary>
        public string TriggerConfig { get; private set; }


        private readonly List<ActionEntry> _actions = new();

        /// <summary>
        /// A read-only collection of actions belonging to this routine.
        /// </summary>
        /// <remarks>
        /// Direct modification is prohibited; use <see cref="AddAction"/> or <see cref="RemoveAction"/>.
        /// </remarks>
        public IReadOnlyCollection<ActionEntry> Actions => _actions.AsReadOnly();

        // private parameterless constructor for EF Core
        private Routine() { }

        /// <summary>
        /// Initializes a new Routine instance.
        /// </summary>
        /// <param name="name">The name of the routine.</param>
        /// <param name="iconPath">Path to the routine icon.</param>
        /// <param name="triggerType">The type of trigger to monitor.</param>
        /// <param name="triggerConfig">JSON configuration for the trigger.</param>
        /// <exception cref="ArgumentException">Thrown if name is null or whitespace.</exception>
        public Routine(string name, string description, string iconPath, TriggerType triggerType, string triggerConfig)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Routine name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(triggerConfig))
                throw new BusinessRuleException("Trigger configuration cannot be empty.");

            Name = name;
            Description = description;
            IconPath = iconPath;
            TriggerType = triggerType;
            TriggerConfig = triggerConfig;
            IsActive = true;
        }

        /// <summary>
        /// Flips the <see cref="IsActive"/> state.
        /// </summary>
        public void ToggleStatus()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Appends a new action to the routine's execution sequence.
        /// </summary>
        /// <param name="action">The action entry to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if action is null.</exception>
        public void AddAction(ActionEntry action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _actions.Add(action);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes an action from the routine.
        /// </summary>
        /// <param name="action">The action entry to remove.</param>
        public void RemoveAction(ActionEntry action)
        {
            if (_actions.Remove(action))
                UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the routine's identification details.
        /// </summary>
        /// <param name="name">New name for the routine.</param>
        /// <param name="iconPath">New icon path.</param>
        public void UpdateDetails(string name, string description, string iconPath, TriggerType triggerType, string triggerConfig)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException("Routine name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(triggerConfig))
                throw new BusinessRuleException("Trigger configuration cannot be empty.");

            Name = name;
            Description = description;
            IconPath = iconPath;
            TriggerType = triggerType;
            TriggerConfig = triggerConfig;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
