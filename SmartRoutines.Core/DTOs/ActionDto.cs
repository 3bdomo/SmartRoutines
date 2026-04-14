using SmartRoutines.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartRoutines.Core.DTOs;

/// <summary>
/// Action representation included in UpsertRoutineDto.
/// </summary>
public sealed class ActionDto
{
    [Required]
    public ActionType Type { get; set; }

    [Required]
    [StringLength(1000)]
    public string Arguments { get; set; } = string.Empty; // JSON or plain string depending on action

    [Range(1, int.MaxValue)]
    public int ExecutionOrder { get; set; }
}
