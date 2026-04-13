using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.Interfaces.Data;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Persists action execution results for audit and troubleshooting.
/// </summary>
public sealed class LoggerService
{
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerService"/> class.
    /// </summary>
    /// <param name="logRepository">The log repository used to persist execution logs.</param>
    public LoggerService(ILogRepository logRepository)
    {
        _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
    }

    /// <summary>
    /// Writes one action execution result to the activity log store.
    /// </summary>
    /// <param name="action">The action that was executed.</param>
    /// <param name="context">The active routine execution context.</param>
    /// <param name="status">The final execution status.</param>
    /// <param name="message">Diagnostic details for this execution step.</param>
    /// <returns>A task that completes when the log is persisted.</returns>
    public async Task LogActionResultAsync(ActionEntry action, ActionContext context, LogStatus status, string message)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var routineName = string.IsNullOrWhiteSpace(context.RoutineName) ? "Unnamed Routine" : context.RoutineName;
        var details = $"[{action.Type}] {message}";
        var log = new ActivityLog(action.RoutineId, routineName, status, details);

        await _logRepository.AddAsync(log);
    }
}

