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
    /// Adds SmartRoutines logic services, action executors, and the action runner.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection for chaining.</returns>
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // ── Live logger: Singleton so the UI can subscribe to its events once
        //    and receive notifications across the entire application lifetime.
        services.AddSingleton<LiveLogger>();
        services.AddSingleton<ILiveLogger>(sp => sp.GetRequiredService<LiveLogger>());

        // ── Activity log persistence service (used by the Logs page)
        services.AddScoped<IActivityLogService, LoggerService>();

        // ── Action executors: Transient — each pipeline run gets fresh instances
        services.AddTransient<IAction, LaunchAppExecutor>();
        services.AddTransient<IAction, OpenUrlExecutor>();
        services.AddTransient<IAction, AudioExecutor>();
        services.AddTransient<IAction, ProcessKillerExecutor>();
        services.AddTransient<IAction, RunCommandExecutor>();

        // ── Action runner: Transient — constructs a new map per pipeline run
        services.AddTransient<ActionRunner>();

        return services;
    }
}
