using SmartRoutines.Core.Domain.Entities;
using System.Threading.Tasks;

namespace SmartRoutines.Core.Interfaces.Data
{
    /// <summary>
    /// Repository for application-wide settings. This repository manages a single row and
    /// intentionally does NOT expose the generic repository contract (no Add/Delete operations).
    /// </summary>
    public interface IAppSettingsRepository
    {
        Task<AppSettings?> GetAsync();
        Task<AppSettings> GetOrCreateDefaultAsync();
        void Update(AppSettings settings);
    }
}
