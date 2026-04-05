namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Defines the contract for an event trigger within the context of a routine.
/// </summary>
public interface ITrigger:IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the trigger conditions are currently satisfied.
    /// </summary>
    bool IsSatisfied { get; }

    /// <summary>
    /// Initializes the trigger securely substituting a configuration payload.
    /// </summary>
    /// <param name="configJson">Serialization payload defining the trigger's configuration options.</param>
    void Initialize(string configJson);
}
