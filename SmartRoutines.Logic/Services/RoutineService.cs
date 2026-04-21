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
    private static readonly SemaphoreSlim _dbLock = new(1, 1);

    public RoutineService(IUnitOfWork uow)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoutineCardDto>> GetAllCardsAsync()
    {
        await _dbLock.WaitAsync();
        try
        {
            var routines = await _uow.Routines.GetAllAsync();
            return routines.Select(r => new RoutineCardDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IconPath = r.IconPath,
                TriggerSummary = r.TriggerType.ToString(),
                ActionCount = r.Actions.Count,
                IsActive = r.IsActive,
                IsRunningNow = false,
                LastRunRelativeTime = string.Empty
            }).ToList();
        }
        finally
        {
            _dbLock.Release();
        }
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
        await _dbLock.WaitAsync();
        try
        {
            // Validation
            if (!await _uow.Routines.IsNameUniqueAsync(dto.Name, dto.Id))
                throw new DuplicateRoutineNameException(dto.Name);

            Routine? entity = null;
            if (dto.Id.HasValue)
            {
                entity = await _uow.Routines.GetByIdWithActionsAsync(dto.Id.Value);
                if (entity == null)
                    throw new RoutineNotFoundException(dto.Id.Value);
                // update fields
                entity.UpdateDetails(dto.Name, dto.Description, dto.IconPath ?? string.Empty, dto.TriggerType, dto.TriggerConfig);
            }
            else
            {
                entity = new Routine(dto.Name, dto.Description ?? string.Empty, dto.IconPath ?? string.Empty, dto.TriggerType, dto.TriggerConfig);
                await _uow.Routines.AddAsync(entity);
            }

            // Sync actions: existing vs dto
            var existingActions = entity.Actions.ToDictionary(a => a.ExecutionOrder);
            var dtoByOrder = dto.Actions.OrderBy(a => a.ExecutionOrder).ToList();

            // 1. Remove actions not in DTO (by order)
            var dtoOrders = dtoByOrder.Select(d => d.ExecutionOrder).ToHashSet();
            foreach (var existing in entity.Actions.ToList())
            {
                if (!dtoOrders.Contains(existing.ExecutionOrder))
                {
                    entity.RemoveAction(existing);
                }
            }

            // 2. Add or Update
            foreach (var adto in dtoByOrder)
            {
                if (existingActions.TryGetValue(adto.ExecutionOrder, out var existing))
                {
                    existing.UpdateDetails(adto.Type, adto.Arguments, adto.ExecutionOrder);
                }
                else
                {
                    entity.AddAction(new ActionEntry(entity.Id, adto.Type, adto.Arguments, adto.ExecutionOrder));
                }
            }

            // Persist
            await _uow.SaveChangesAsync();
            return entity.Id;
        }
        finally
        {
            _dbLock.Release();
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
