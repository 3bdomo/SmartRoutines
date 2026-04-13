using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Data.Context;

namespace SmartRoutines.Data
{
    /// <summary>
    /// Registers data layer services (DbContext, UnitOfWork) into the application's DI container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds SmartRoutines data services to the service collection.
        /// - Registers <see cref="SmartRoutinesDbContext"/> using SQL Server.
        /// - Registers <see cref="IUnitOfWork"/> as scoped.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">Application configuration to read the connection string from.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Please add it to your configuration (e.g., appsettings.json).");
            }

            services.AddDbContext<SmartRoutinesDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            // Register the UnitOfWork as scoped so the same DbContext is shared across repositories within a unit of work.
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
