using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs.Configurations;

/// <summary>
/// Trigger configuration for time-based triggers.
/// </summary>
public sealed class TimeTriggerConfig
{
    [Required]
    public TimeSpan Time { get; set; }

    /// <summary>
    /// Flags enum indicating which days the trigger should run on.
    /// </summary>
 
    public List<Domain.Enums.DayOfWeek> DaysOfWeek { get; set; } = new();

}
