using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs.Configurations;

/// <summary>
/// Arguments for launching an application.
/// </summary>
public sealed class LaunchAppActionArgs
{
    [Required]
    [StringLength(1000)]
    public string Path { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Args { get; set; }
}
