using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Persists action execution results for audit and troubleshooting.
/// Implementations must use the provided UnitOfWork only for data access.
/// </summary>
public sealed class LoggerService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public event EventHandler<ActivityLogEventArgs>? OnLogAdded;
    public event EventHandler? OnLogsCleared;

    /// <summary>
    /// Initializes a new instance of <see cref="LoggerService"/>.
    /// </summary>
    public LoggerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task ClearAllLogsAsync()
    {
        // Use repository set-based clear and persist via unit of work
        await _unitOfWork.ActivityLogs.ClearAllAsync();
        await _unitOfWork.SaveChangesAsync();

        // Raise event to notify subscribers
        OnLogsCleared?.Invoke(this, EventArgs.Empty);
    }

    public async Task<(IReadOnlyList<ActivityLogDto> Logs, int TotalCount)> GetPagedLogsAsync(int page, int pageSize, LogStatus? statusFilter = null, Guid? routineId = null)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        // Get total count using repository (respects optional status filter)
        var totalCount = await _unitOfWork.ActivityLogs.GetTotalCountAsync(statusFilter);

        // Retrieve page from repository
        var entities = (await _unitOfWork.ActivityLogs.GetPagedAsync(page, pageSize, statusFilter, routineId)).ToList();

        var dtos = entities
            .Select(e => new ActivityLogDto
            {
                Id = e.Id,
                RoutineName = e.RoutineName,
                Status = e.Status,
                Message = e.Message,
                ExecutedAt = e.CreatedAt,
                DurationSeconds = 0,
                RelativeTime = GetRelativeTime(e.CreatedAt.ToLocalTime())
            })
            .ToList();

        return (dtos, totalCount);
    }

    public async Task AddLogAsync(ActivityLogDto logDto)
    {
        if (logDto == null) throw new ArgumentNullException(nameof(logDto));

        // DTO does not include RoutineId; persist a log with an empty Guid when not available.
        var routineId = Guid.Empty;
        var routineName = string.IsNullOrWhiteSpace(logDto.RoutineName) ? "Unnamed Routine" : logDto.RoutineName;
        var message = logDto.Message ?? string.Empty;

        var entity = new ActivityLog(routineId, routineName, logDto.Status, message);

        await _unitOfWork.ActivityLogs.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // Raise event to notify subscribers about the new log
        OnLogAdded?.Invoke(this, new ActivityLogEventArgs(logDto));
    }

    private string GetRelativeTime(DateTime dateTime)
    {
        var diff = DateTime.Now - dateTime;

        if (diff.TotalSeconds < 60)
            return "Just now";

        if (diff.TotalMinutes < 60)
            return $"{(int)diff.TotalMinutes} min ago";

        if (diff.TotalHours < 24)
            return $"{(int)diff.TotalHours} hr ago";

        if (diff.TotalDays < 7)
            return $"{(int)diff.TotalDays} day ago";

        return dateTime.ToString("yyyy-MM-dd");
    }

    // ── Internal legacy helper (kept for any callers not yet migrated) ───────

    /// <summary>
    /// Legacy helper used elsewhere for recording action results (keeps existing behaviour).
    /// It uses the UnitOfWork already so no changes required here.
    /// </summary>
    public async Task LogActionResultAsync(ActionEntry action, Core.Domain.Models.ActionContext context, LogStatus status, string message)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        if (context is null) throw new ArgumentNullException(nameof(context));

        var routineName = string.IsNullOrWhiteSpace(context.RoutineName)
            ? "Unnamed Routine"
            : context.RoutineName;

        var details = $"[{action.Type}] {message}";

        // ActivityLog still uses a Guid for the routine reference; default used here
        // because RuntimeAction intentionally carries no DB identity.
        var log = new ActivityLog(Guid.Empty, routineName, status, details);

        await _unitOfWork.ActivityLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        // Raise event for new log added via legacy method
        var dto = new ActivityLogDto
        {
            Id = log.Id,
            RoutineName = log.RoutineName,
            Status = log.Status,
            Message = log.Message,
            ExecutedAt = log.CreatedAt,
            DurationSeconds = 0,
            RelativeTime = GetRelativeTime(log.CreatedAt.ToLocalTime())
        };
        OnLogAdded?.Invoke(this, new ActivityLogEventArgs(dto));
    }
}
