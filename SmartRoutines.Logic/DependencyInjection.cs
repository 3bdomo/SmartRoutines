using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Domain.Enums;
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

        // Core Services
        // 
        services.AddScoped<IRoutineService, RoutineService>();
        // Live logger: Singleton so the UI can subscribe once and receive events
        services.AddSingleton<LiveLogger>();
        services.AddSingleton<ILiveLogger>(sp => sp.GetRequiredService<LiveLogger>());

        // Activity log service: scoped so it can work with UnitOfWork per request/operation
        services.AddScoped<IActivityLogService, LoggerService>();

        // Executors
        // Register IAction implementations as transient - a fresh instance per pipeline
        services.AddTransient<IAction, LaunchAppExecutor>();
        services.AddTransient<IAction, OpenUrlExecutor>();
        services.AddTransient<IAction, AudioExecutor>();
        services.AddTransient<IAction, ProcessKillerExecutor>();
        services.AddTransient<IAction, RunCommandExecutor>();

        // Action runner: transient so each execution gets a new runner
        services.AddTransient<ActionRunner>();

        // Engine Components
        // TriggerFactory is implemented as a stateless creator (static helper).
        // Register a delegate pointing to the factory method so consumers may obtain triggers via DI.
        services.AddSingleton<Func<TriggerType, ITrigger?>>(sp => SmartRoutines.Logic.AutomationEngine.TriggerFactory.Create);

        // Automation engine: singleton to maintain background loop and trigger cache for app lifetime
        services.AddSingleton<IAutomationEngine, SmartRoutines.Logic.AutomationEngine.AutomationEngine>();

        return services;
    }
}
