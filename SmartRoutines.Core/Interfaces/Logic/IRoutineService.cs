using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRoutines.Core.DTOs;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Service responsible for handling routines (CRUD + domain operations) and bridging UI to data layer.
/// </summary>
public interface IRoutineService
{
    /// <summary>
    /// Returns a lightweight set of routine cards used by the UI.
    /// </summary>
    Task<IReadOnlyList<RoutineCardDto>> GetAllCardsAsync();

    /// <summary>
    /// Retrieves a routine's data prepared for editing. Returns null if not found.
    /// </summary>
    Task<UpsertRoutineDto?> GetForEditAsync(Guid id);

    /// <summary>
    /// Creates or updates a routine. Returns the entity Id of the saved routine.
    /// </summary>
    Task<Guid> SaveAsync(UpsertRoutineDto dto);

    /// <summary>
    /// Deletes a routine by id.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Toggles the routine's enabled state.
    /// </summary>
    Task ToggleStatusAsync(Guid id);

    /// <summary>
    /// Checks whether the provided name is unique for routines.
    /// </summary>
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
}
