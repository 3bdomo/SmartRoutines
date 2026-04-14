using SmartRoutines.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs;

/// <summary>
/// DTO for create/update operations for a Routine.
/// </summary>
public sealed class UpsertRoutineDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    [StringLength(255)]
    public string? IconPath { get; set; }

    [Required]
    public TriggerType TriggerType { get; set; }

    [Required]
    [StringLength(2000)]
    public string TriggerConfig { get; set; } = string.Empty; // JSON payload matching a TriggerConfig DTO

    [Required]
    public List<ActionDto> Actions { get; set; } = new();
}
