using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartRoutines.Core.Domain.Entities;

namespace SmartRoutines.Data.Context
{
    public class SmartRoutinesDbContext : DbContext
    {

        public DbSet<Routine> Routines => Set<Routine>();
        public DbSet<ActionEntry> ActionEntries => Set<ActionEntry>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<AppSettings> AppSettings => Set<AppSettings>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
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
