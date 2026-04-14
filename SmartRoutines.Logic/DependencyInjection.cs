using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.ActionExecutors;
using SmartRoutines.Logic.Services;

namespace SmartRoutines.Logic;

/// <summary>
/// Registers logic-layer services into the application's dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds SmartRoutines logic services and action executors.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<ILiveLogger, LiveLogger>();

        services.AddScoped<IAction, LaunchAppExecutor>();
        services.AddScoped<IAction, OpenUrlExecutor>();
        services.AddScoped<IAction, AudioExecutor>();
        services.AddScoped<IAction, ProcessKillerExecutor>();
        services.AddScoped<IAction, RunCommandExecutor>();

        services.AddScoped<ActionRunner>();
        return services;
    }
}


