using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using System.Text.Json;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Implementation of <see cref="IRoutineService"/> responsible for routine CRUD and domain operations.
/// </summary>
public class RoutineService : IRoutineService
{
    private readonly IUnitOfWork _uow;

    public RoutineService(IUnitOfWork uow)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoutineCardDto>> GetAllCardsAsync()
    {
        var routines = await _uow.Routines.GetAllAsync();
        return routines.Select(r => new RoutineCardDto
        {
            Id = r.Id,
            Name = r.Name,
            IconPath = r.IconPath,
            TriggerSummary = r.TriggerType.ToString(),
            ActionCount = r.Actions.Count,
            IsActive = r.IsActive,
            IsRunningNow = false,
            LastRunRelativeTime = string.Empty
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<UpsertRoutineDto?> GetForEditAsync(Guid id)
    {
        var r = await _uow.Routines.GetByIdWithActionsAsync(id);
        if (r == null) return null;

        return new UpsertRoutineDto
        {
            Id = r.Id,
            Name = r.Name,
            IconPath = r.IconPath,
            TriggerType = r.TriggerType,
            TriggerConfig = r.TriggerConfig,
            Actions = r.Actions.Select(a => new ActionDto
            {
                Type = a.Type,
                Arguments = a.Arguments,
                ExecutionOrder = a.ExecutionOrder
            }).ToList()
        };
    }

    /// <inheritdoc />
    public async Task<Guid> SaveAsync(UpsertRoutineDto dto)
    {
        // Validation
        if (!await _uow.Routines.IsNameUniqueAsync(dto.Name, dto.Id))
            throw new DuplicateRoutineNameException(dto.Name);

        // Start transaction
        await _uow.BeginTransactionAsync();
        try
        {
            Routine? entity = null;
            if (dto.Id.HasValue)
            {
                entity = await _uow.Routines.GetByIdAsync(dto.Id.Value);
                if (entity == null)
                    throw new RoutineNotFoundException(dto.Id.Value);
                // update fields
                entity.UpdateDetails(dto.Name, dto.Description, dto.IconPath ?? string.Empty);
            }
            else
            {
                entity = new Routine(dto.Name, dto.Description ?? string.Empty, dto.IconPath ?? string.Empty, dto.TriggerType, dto.TriggerConfig);
                await _uow.Routines.AddAsync(entity);
            }

            // Sync actions: existing vs dto
            // Build maps
            var existingActions = entity.Actions.ToDictionary(a => a.Id);
            var dtoByOrder = dto.Actions.OrderBy(a => a.ExecutionOrder).ToList();

            // Remove actions not present
            foreach (var existing in existingActions.Values)
            {
                if (!dtoByOrder.Any(d => d.ExecutionOrder == existing.ExecutionOrder))
                {
                    entity.RemoveAction(existing);
                }
            }

            // Add or update
            foreach (var adto in dtoByOrder)
            {
                var match = entity.Actions.FirstOrDefault(a => a.ExecutionOrder == adto.ExecutionOrder);
                if (match != null)
                {
                    // update via replace: here domain probably has methods; using UpdateDetails on routine to persist changes
                    match = new ActionEntry(entity.Id, adto.Type, adto.Arguments, adto.ExecutionOrder);
                    // can't directly replace in EF tracked collection so remove and add
                    entity.RemoveAction(entity.Actions.First(a => a.ExecutionOrder == adto.ExecutionOrder));
                    entity.AddAction(match);
                }
                else
                {
                    entity.AddAction(new ActionEntry(entity.Id, adto.Type, adto.Arguments, adto.ExecutionOrder));
                }
            }

            // Persist
            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            return entity.Id;
        }
        catch (JsonException jex)
        {
            await _uow.RollbackTransactionAsync();
            throw new TriggerInitializationException("Trigger configuration JSON invalid.", jex);
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _uow.Routines.GetByIdAsync(id);
        if (entity == null) throw new RoutineNotFoundException(id);

        entity.SoftDelete();
        await _uow.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task ToggleStatusAsync(Guid id)
    {
        var entity = await _uow.Routines.GetByIdAsync(id);
        if (entity == null) throw new RoutineNotFoundException(id);
        entity.ToggleStatus();
        await _uow.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
    {
        return await _uow.Routines.IsNameUniqueAsync(name, excludeId);
    }

    /// <summary>
    /// Retrieves active routines and maps them to RuntimeRoutine for engine consumption.
    /// </summary>
    public async Task<IReadOnlyList<RuntimeRoutine>> GetActiveRoutinesForRuntimeAsync()
    {
        var list = await _uow.Routines.GetActiveNotDeletedWithActionsAsync();
        // Map to runtime models
        var mapped = list
            .Select(r => new RuntimeRoutine
            (
                r.Id,
                r.Name,
                r.TriggerType,
                r.TriggerConfig,
                r.Actions.OrderBy(a => a.ExecutionOrder)
                .Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder))
            ))
            .ToList();

        return mapped;
    }
}
