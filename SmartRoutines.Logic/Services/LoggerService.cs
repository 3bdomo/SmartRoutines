using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Persists action execution results and provides historical log access.
/// This service is intentionally decoupled from <see cref="ActionRunner"/>;
/// callers that need DB persistence subscribe to <see cref="LiveLogger.OnLogReceived"/>
/// and call <see cref="AddLogAsync"/> independently.
/// </summary>
public sealed class LoggerService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="LoggerService"/>.
    /// </summary>
    /// <param name="unitOfWork">The unit-of-work used to persist log entries.</param>
    public LoggerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    // ── IActivityLogService ─────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task AddLogAsync(ActivityLogDto logDto)
    {
        ArgumentNullException.ThrowIfNull(logDto);

        var routineName = string.IsNullOrWhiteSpace(logDto.RoutineName)
            ? "Unnamed Routine"
            : logDto.RoutineName;

        var log = new ActivityLog(
            routineId:   logDto.Id,
            routineName: routineName,
            status:      logDto.Status,
            message:     logDto.Message);

        await _unitOfWork.ActivityLogs.AddAsync(log).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<(IReadOnlyList<ActivityLogDto> Logs, int TotalCount)> GetPagedLogsAsync(
        int page,
        int pageSize,
        LogStatus? statusFilter = null,
        Guid? routineId = null)
    {
        // TODO: implement paged query via _unitOfWork.ActivityLogs
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task ClearAllLogsAsync()
    {
        // TODO: implement bulk delete via _unitOfWork.ActivityLogs
        throw new NotImplementedException();
    }

    // ── Internal legacy helper (kept for any callers not yet migrated) ───────

    /// <summary>
    /// Writes one action execution result directly from a runtime action snapshot.
    /// Prefer <see cref="AddLogAsync"/> for new callers.
    /// </summary>
    internal async Task LogActionResultAsync(
        RuntimeAction action,
        ActionContext  context,
        LogStatus      status,
        string         message)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        var routineName = string.IsNullOrWhiteSpace(context.RoutineName)
            ? "Unnamed Routine"
            : context.RoutineName;

        var details = $"[{action.Type}] {message}";

        // ActivityLog still uses a Guid for the routine reference; default used here
        // because RuntimeAction intentionally carries no DB identity.
        var log = new ActivityLog(Guid.Empty, routineName, status, details);

        await _unitOfWork.ActivityLogs.AddAsync(log).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
    }
}
