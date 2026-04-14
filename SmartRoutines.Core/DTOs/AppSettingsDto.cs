using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs;

/// <summary>
/// DTO representing application-level settings.
/// </summary>
public sealed class AppSettingsDto
{
    [Required]
    [StringLength(50)]
    public string Theme { get; set; } = "Dark";

    public bool AutoStart { get; set; } = true;

    public bool EngineEnabled { get; set; } = true;
}
