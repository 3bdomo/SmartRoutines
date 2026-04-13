using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;

namespace SmartRoutines.Data.Repositories
{
    /// <summary>
    /// Repository for the single-row AppSettings entity. This repository immediately persists
    /// default settings if none exist to ensure a single source of truth.
    /// </summary>
    internal class AppSettingsRepository : IAppSettingsRepository
    {
        private readonly SmartRoutinesDbContext _ctx;

        public AppSettingsRepository(SmartRoutinesDbContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        /// <inheritdoc />
        public async Task<AppSettings?> GetAsync()
        {
            return await _ctx.AppSettings.FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<AppSettings> GetOrCreateDefaultAsync()
        {
            var existing = await GetAsync();
            if (existing != null) return existing;

            var defaults = new AppSettings();
            await _ctx.AppSettings.AddAsync(defaults);
            await _ctx.SaveChangesAsync();
            return defaults;
        }

        /// <inheritdoc />
        public void Update(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _ctx.AppSettings.Update(settings);
        }
    }
}
