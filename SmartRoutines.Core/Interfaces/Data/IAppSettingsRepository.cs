using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Core.Interfaces.Data
{
    public interface IAppSettingsRepository
    {
        Task<AppSettings?> GetAsync();
        Task<AppSettings> GetOrCreateDefaultAsync();
        void Update(AppSettings settings);
    }
}
