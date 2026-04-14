using System.Threading.Tasks;
using SmartRoutines.Core.DTOs;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Application settings management service.
/// </summary>
public interface IAppSettingsService
{
    Task<AppSettingsDto> GetSettingsAsync();
    Task UpdateSettingsAsync(AppSettingsDto dto);
}
