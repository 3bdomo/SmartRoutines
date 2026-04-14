using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs.Configurations;

/// <summary>
/// Trigger configuration for application launch triggers.
/// </summary>
public sealed class AppLaunchedTriggerConfig
{
    [Required]
    [StringLength(200)]
    public string ProcessName { get; set; } = string.Empty;
}
