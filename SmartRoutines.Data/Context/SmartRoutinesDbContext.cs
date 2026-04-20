using Microsoft.EntityFrameworkCore;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Context
{
    public class SmartRoutinesDbContext : DbContext
    {

        public DbSet<Routine> Routines => Set<Routine>();
        public DbSet<ActionEntry> ActionEntries => Set<ActionEntry>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<AppSettings> AppSettings => Set<AppSettings>();

        // public SmartRoutinesDbContext()
        // {
        // }

        public SmartRoutinesDbContext(DbContextOptions<SmartRoutinesDbContext> options)
            : base(options)
        {
        }

        /*
        /// <summary>
        /// Called only at design-time (e.g. Add-Migration) when no options are configured via DI.
        /// Finds the appsettings.json in the UI project to read the connection string.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var basePath = ResolveConfigurationBasePath();
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
            }
        }

        private static string ResolveConfigurationBasePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var candidates = new[]
            {
                currentDirectory,
                Path.Combine(currentDirectory, "SmartRoutines.UI"),
                Path.GetFullPath(Path.Combine(currentDirectory, "..", "SmartRoutines.UI")),
                AppContext.BaseDirectory
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                    return candidate;
            }

            return currentDirectory;
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartRoutinesDbContext).Assembly);

            modelBuilder.Entity<Routine>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<ActionEntry>().HasQueryFilter(a => !a.IsDeleted);

        }

    }
}
