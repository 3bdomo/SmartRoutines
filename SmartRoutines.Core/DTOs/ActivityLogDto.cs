using System;
using SmartRoutines.Core.Domain.Enums;

namespace SmartRoutines.Core.DTOs;

/// <summary>
/// DTO representing an activity/log entry for display in the UI.
/// </summary>
public sealed class ActivityLogDto
{
    public Guid Id { get; init; }
    public string RoutineName { get; init; } = string.Empty;
    public LogStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime ExecutedAt { get; init; }
    public string RelativeTime { get; init; } = string.Empty;
    public int DurationSeconds { get; init; }
}
