namespace SmartRoutines.Core.DTOs;

/// <summary>
/// Lightweight DTO used to render a routine card in the UI.
/// </summary>
public sealed class RoutineCardDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? IconPath { get; init; }
    public string TriggerSummary { get; init; } = string.Empty;
    public int ActionCount { get; init; }
    public bool IsActive { get; init; }
    public bool IsRunningNow { get; init; }
    public string LastRunRelativeTime { get; init; } = string.Empty;
}
