using System;

namespace SmartRoutines.Core.Interfaces.Logic
{
    /// <summary>
    /// Defines the contract for an event trigger within the context of a routine.
    /// </summary>
    public interface ITrigger : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the trigger is currently active.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the trigger has already fired for its current cycle.
        /// </summary>
        bool HasFired { get; }

        /// <summary>
        /// Human-readable name for UI display (e.g., 'Every day at 07:30' or 'Battery < 20%')
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Initializes the trigger and deserializes the configuration payload.
        /// (Equivalent to your Initialize method)
        /// </summary>
        /// <param name="json">Serialization payload defining the trigger's settings.</param>
        void Configure(string json);

        /// <summary>
        /// Called by the Engine every 1s-30s. Returns true when the condition is met.
        /// </summary>
        /// <returns>True if the routine should execute, otherwise false.</returns>
        Task<bool> ShouldFireAsync();

        /// <summary>
        /// Returns a human-readable status of why the trigger is or isn't firing.
        /// Used for real-time UI diagnostics.
        /// </summary>
        string GetDiagnosticInfo();

        /// <summary>
        /// Post-execution hook. Called by the engine immediately after execution 
        /// to update internal state (e.g., set a flag to prevent infinite loops).
        /// </summary>
        void OnFired();

        /// <summary>
        /// Resets the trigger's fired state.
        /// </summary>
        void Reset();
    }
}