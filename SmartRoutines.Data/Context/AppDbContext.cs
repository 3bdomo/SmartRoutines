using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartRoutines.Data.Configurations;

namespace SmartRoutines.Data.Context
{
    internal class SmartRoutinesDbContext : DbContext
    {
        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetSection("ConnectionStrings").Value;

            optionsBuilder.UseSqlServer(connectionString);
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the assembly containing the DbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActionEntryConfiguration).Assembly);
        }
    }

}
