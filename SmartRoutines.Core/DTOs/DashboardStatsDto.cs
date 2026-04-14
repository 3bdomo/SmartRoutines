namespace SmartRoutines.Core.DTOs;

/// <summary>
/// Aggregated statistics for the dashboard view.
/// </summary>
public sealed class DashboardStatsDto
{
    public int TotalRoutines { get; init; }
    public int ActiveRoutines { get; init; }
    public int RunningNowCount { get; init; }
    public int TotalActions { get; init; }
    public bool IsEngineActive { get; init; }
}
