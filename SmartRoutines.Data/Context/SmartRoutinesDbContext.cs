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

        public SmartRoutinesDbContext(DbContextOptions<SmartRoutinesDbContext> options)
            : base(options)
        {
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
