using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.Domain.Models;

/// <summary>
/// Execution-time model representing a routine. This is a frozen, in-memory snapshot
/// of the routine and its actions used by the automation engine.
/// </summary>
public sealed class RuntimeRoutine
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public TriggerType Type { get; init; }
    public string TriggerConfig { get; init; } = string.Empty;

    private readonly List<RuntimeAction> _actions = new();
    public IReadOnlyList<RuntimeAction> Actions => _actions.AsReadOnly();

    public RuntimeRoutine(Guid id, string name, TriggerType type, string triggerConfig, IEnumerable<RuntimeAction>? actions = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (triggerConfig is null) throw new ArgumentNullException(nameof(triggerConfig));

        Id = id;
        Name = name;
        Type = type;
        TriggerConfig = triggerConfig;

        if (actions != null)
            _actions.AddRange(actions);
    }
}
