using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using System.Linq;

namespace SmartRoutines.Logic.Services;

/// <summary>
/// Aggregates lightweight statistics for the UI dashboard.
/// </summary>
public sealed class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly IAutomationEngine? _engine;

    public DashboardService(IUnitOfWork uow, IAutomationEngine? engine = null)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _engine = engine;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var all = await _uow.Routines.GetAllAsync();
        var active = await _uow.Routines.GetActiveNotDeletedWithActionsAsync();

        int totalRoutines = all?.Count ?? 0;
        int activeRoutines = active?.Count() ?? 0;
        int totalActions = all?.Sum(r => r.Actions.Count) ?? 0;
        int runningNow = _engine?.RunningRoutineIds?.Count ?? 0;
        bool isEngineActive = _engine?.IsRunning ?? false;

        return new DashboardStatsDto
        {
            TotalRoutines = totalRoutines,
            ActiveRoutines = activeRoutines,
            RunningNowCount = runningNow,
            TotalActions = totalActions,
            IsEngineActive = isEngineActive
        };
    }
}
