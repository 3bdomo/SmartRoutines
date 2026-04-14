using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs.Configurations;

/// <summary>
/// Arguments for a volume action.
/// </summary>
public sealed class VolumeActionArgs
{
    [Range(0, 100)]
    public int Level { get; set; }

    public bool Mute { get; set; }
}
