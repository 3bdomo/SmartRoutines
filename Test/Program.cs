using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Data;
using SmartRoutines.Logic;

namespace Test
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Build a host with the application DI configuration but using InMemory DB for tests
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Read the connection string from configuration
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    // Register services and dependencies here
                    services.AddDataServices(connectionString);

                    // Logic layer registrations (action executors, engine, loggers)
                    services.AddLogicServices();

                }).Build();



            // Seed two routines with actions and call AutomationEngine to run them.
            using (var scope = host.Services.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<SmartRoutines.Core.Interfaces.Data.IUnitOfWork>();

                // Routine 1: Time trigger scheduled to now (will fire immediately when engine ticks)
                var timeCfg = new SmartRoutines.Core.Domain.Models.TriggerConfiguration
                {
                    ScheduledTime = DateTime.Now,
                    RepeatDays = SmartRoutines.Core.Domain.Enums.DayOfWeek.Mon | SmartRoutines.Core.Domain.Enums.DayOfWeek.Tue | SmartRoutines.Core.Domain.Enums.DayOfWeek.Wed | SmartRoutines.Core.Domain.Enums.DayOfWeek.Thu | SmartRoutines.Core.Domain.Enums.DayOfWeek.Fri | SmartRoutines.Core.Domain.Enums.DayOfWeek.Sat | SmartRoutines.Core.Domain.Enums.DayOfWeek.Sun
                };

                var r1 = new Routine("Test Time Routine", "Executes immediately", "", TriggerType.Time, System.Text.Json.JsonSerializer.Serialize(timeCfg));
                r1.AddAction(new ActionEntry(r1.Id, SmartRoutines.Core.Domain.Enums.ActionType.RunCommand, "echo TimeRoutine", 1));

                // Routine 2: Startup trigger - Should fire once as HasFired is false
                var r2 = new Routine("Test Startup Routine", "Executes on startup", "", TriggerType.Startup, "{}");
                r2.AddAction(new ActionEntry(r2.Id, SmartRoutines.Core.Domain.Enums.ActionType.RunCommand, "echo StartupRoutine", 1));

                // Routine 3: AppOpen trigger - configured with empty JSON so it will fire once for tests
                var r3 = new Routine("Test AppOpen Routine", "Executes when an app opens (test)", "", TriggerType.AppOpen, "{}");
                // Add three actions to the routine - Launch Notepad, Run a command, Open a URL
                r3.AddAction(new ActionEntry(r3.Id, SmartRoutines.Core.Domain.Enums.ActionType.LaunchApp, "notepad.exe", 1));
                r3.AddAction(new ActionEntry(r3.Id, SmartRoutines.Core.Domain.Enums.ActionType.RunCommand, "echo AppOpenRoutine Action 2", 2));
                r3.AddAction(new ActionEntry(r3.Id, SmartRoutines.Core.Domain.Enums.ActionType.OpenUrl, "https://example.com", 3));

                await uow.Routines.AddAsync(r1);
                await uow.Routines.AddAsync(r2);
                await uow.SaveChangesAsync();
            }

            try
            {


                // Resolve engine and start it
                using (var scope = host.Services.CreateScope())
                {
                    var engine = scope.ServiceProvider.GetRequiredService<SmartRoutines.Core.Interfaces.Logic.IAutomationEngine>();

                    // Subscribe to events for console output
                    engine.RoutineStarted += (s, e) => Console.WriteLine($"Routine started: {e.RoutineId}");
                    engine.RoutineCompleted += (s, e) => Console.WriteLine($"Routine completed: {e.RoutineId} Success={e.Succeeded} Error={e.Error?.Message}");
                    engine.LogEmitted += (s, e) => Console.WriteLine($"Log: {e.Log.RoutineName} - {e.Log.Message}");

                    await engine.StartAsync();

                    Console.WriteLine("Engine started. Waiting 5 seconds for triggers to fire...");
                    // Wait a few seconds for the engine loop to evaluate triggers
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    Console.WriteLine("Triggering routines manually to ensure execution (RunRoutineNowAsync)");

                    // Manually trigger both routines to guarantee run in the test
                    using var manualScope = host.Services.CreateScope();
                    var uow2 = manualScope.ServiceProvider.GetRequiredService<SmartRoutines.Core.Interfaces.Data.IUnitOfWork>();
                    var routines = (await uow2.Routines.GetAllAsync()).ToList();
                    var engineService = manualScope.ServiceProvider.GetRequiredService<SmartRoutines.Core.Interfaces.Logic.IAutomationEngine>();
                    foreach (var r in routines)
                    {
                        Console.WriteLine($"Manually running: {r.Name} ({r.Id})");
                        // IAutomationEngine exposes ExecuteManualAsync in the interface
                        await engineService.ExecuteManualAsync(r.Id);
                    }

                    // Wait for manual runs to finish
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    Console.WriteLine("Stopping engine...");
                    await engine.StopAsync();
                }

                await host.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message__________{ex.Message}");
            }
            return 0;
        }
    }
}
