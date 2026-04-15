using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using SmartRoutines.Core.Domain.Entities;
using System.IO;

namespace SmartRoutines.Data.Context
{
    public class SmartRoutinesDbContext : DbContext
    {

        public DbSet<Routine> Routines => Set<Routine>();
        public DbSet<ActionEntry> ActionEntries => Set<ActionEntry>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<AppSettings> AppSettings => Set<AppSettings>();

        public SmartRoutinesDbContext(DbContextOptions<SmartRoutinesDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Called only at design-time (Add-Migration) when no options are configured via DI.
        /// Reads the connection string from appsettings.json in the startup project root.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartRoutinesDbContext).Assembly);

            modelBuilder.Entity<Routine>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<ActionEntry>().HasQueryFilter(a => !a.IsDeleted);

        }

    }
}
