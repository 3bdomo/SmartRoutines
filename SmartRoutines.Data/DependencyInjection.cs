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
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Please add it to your appsettings.json.");
            }

            services.AddDbContext<SmartRoutinesDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            });

            // Register the UnitOfWork as scoped so the same DbContext is shared across repositories within a unit of work.
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
