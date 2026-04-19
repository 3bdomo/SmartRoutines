using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartRoutines.Data.Context
{
    /// <summary>
    /// Explicit design-time factory for EF tooling (migrations/update database).
    /// This avoids relying on startup DI construction, which can fail for unrelated services.
    /// </summary>
    public sealed class SmartRoutinesDbContextFactory : IDesignTimeDbContextFactory<SmartRoutinesDbContext>
    {
        public SmartRoutinesDbContext CreateDbContext(string[] args)
        {
            var basePath = ResolveConfigurationBasePath();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found in appsettings.json.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<SmartRoutinesDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            });

            return new SmartRoutinesDbContext(optionsBuilder.Options);
        }

        private static string ResolveConfigurationBasePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var candidates = new[]
            {
                currentDirectory,
                Path.Combine(currentDirectory, "SmartRoutines.UI"),
                Path.GetFullPath(Path.Combine(currentDirectory, "..", "SmartRoutines.UI")),
                AppContext.BaseDirectory,
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SmartRoutines.UI"))
            };

            foreach (var candidate in candidates)
            {
                var appSettingsPath = Path.Combine(candidate, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException(
                $"Could not find appsettings.json for design-time DbContext creation. Current directory: '{currentDirectory}'.");
        }
    }
}

