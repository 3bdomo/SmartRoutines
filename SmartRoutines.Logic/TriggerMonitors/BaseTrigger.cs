using System;
using System.Text.Json;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.TriggerMonitors
{
    /// <summary>
    /// Provides common logic for all triggers, such as firing state management and simplified configuration.
    /// </summary>
    public abstract class BaseTrigger : ITrigger
    {
        protected bool HasFired { get; set; } = false;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public virtual void Configure(string json) { }

        /// <inheritdoc />
        public abstract Task<bool> ShouldFireAsync();

        /// <inheritdoc />
        public virtual string GetDiagnosticInfo() => IsEnabled ? "Running" : "Disabled";

        /// <inheritdoc />
        public virtual void OnFired()
        {
            HasFired = true;
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            HasFired = false;
        }

        /// <inheritdoc />
        public virtual void Dispose() { }
    }

    /// <summary>
    /// Base class for triggers that require a specific JSON configuration.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration object.</typeparam>
    public abstract class BaseTrigger<TConfig> : BaseTrigger where TConfig : class
    {
        protected TConfig Config { get; private set; }

        /// <inheritdoc />
        public override void Configure(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            try
            {
                Config = JsonSerializer.Deserialize<TConfig>(json);
            }
            catch (JsonException)
            {
                // In a production app, we would log this error.
                Config = null;
            }
        }
    }
}
