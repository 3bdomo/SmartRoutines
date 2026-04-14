using System.Threading.Tasks;
using SmartRoutines.Core.DTOs;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Provides aggregated information for the dashboard.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets summary statistics for display on the dashboard.
    /// </summary>
    Task<DashboardStatsDto> GetStatsAsync();
}
