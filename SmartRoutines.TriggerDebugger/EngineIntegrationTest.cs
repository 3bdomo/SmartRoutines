using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using System.Text.Json;

namespace SmartRoutines.TriggerDebugger;

public class EngineIntegrationTest
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EngineIntegrationTest> _logger;

    public EngineIntegrationTest(IServiceProvider serviceProvider, ILogger<EngineIntegrationTest> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task RunTestAsync()
    {
        _logger.LogInformation("--- STARTING END-TO-END AUTOMATION ENGINE INTEGRATION TEST ---");

        // 1. Database Seeding
        using (var scope = _serviceProvider.CreateScope())
        {
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await SeedDatabaseAsync(uow);
        }

        // 2. Engine Lifecycle
        var engine = _serviceProvider.GetRequiredService<IAutomationEngine>();

        engine.RoutineStarted += (sender, args) => _logger.LogInformation($"[Engine Event] Routine '{args.RoutineId}' Started.");
        engine.RoutineCompleted += (sender, args) =>
        {
            if (args.Succeeded)
                _logger.LogInformation($"[Engine Event] Routine '{args.RoutineId}' Completed Successfully.");
            else
                _logger.LogError($"[Engine Event] Routine '{args.RoutineId}' Failed! Error: {args.Error?.Message}");
        };

        engine.LogEmitted += (sender, args) => _logger.LogInformation($"[Engine LOG Activity] {args.Log.RoutineName} -> {args.Log.Message}");

        // Subscribe to LiveLogger securely
        var liveLogger = _serviceProvider.GetRequiredService<ILiveLogger>();
        if (liveLogger is Logic.Services.LiveLogger ll)
        {
            ll.OnLogReceived += (level, message) =>
            {
                var lvlStr = level.ToString();
                if (lvlStr == LogStatus.Error.ToString()) _logger.LogError($"[LiveLogger ERROR] {message}");
                else if (lvlStr == LogStatus.Warning.ToString()) _logger.LogWarning($"[LiveLogger WARN] {message}");
                else _logger.LogInformation($"[LiveLogger INFO] {message}");
            };
        }

        await engine.StartAsync();

        _logger.LogInformation("Engine started. Waiting for 2 minutes to observe heartbeat and executions...");

        // Wait 2 minutes for tests to run
        await Task.Delay(TimeSpan.FromMinutes(2));

        _logger.LogInformation("Stopping engine gracefully...");
        await engine.StopAsync();

        _logger.LogInformation("--- INTEGRATION TEST COMPLETE ---");
    }

    private async Task SeedDatabaseAsync(IUnitOfWork uow)
    {
        _logger.LogInformation("Seeding database with test routines...");

        // Routine A (WiFi Connection)
        var routineA = new Routine(
            "WiFi Connected Routine A",
            "Test description",
            "wifi.png",
            TriggerType.WiFi,
            JsonSerializer.Serialize(new { SsidName = "A_7" })
        );

        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.Notification, "WiFi A_7 Connected!", 1));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.RunCommand, "echo Hello from WiFi Routine A", 2));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.Mute, "true", 3));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.RunCommand, "notepad.exe", 4));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.RunCommand, "calc.exe", 5));

        // Routine B (Time + 10s)
        var triggerTime = DateTime.Now.AddSeconds(5);
        var routineB = new Routine(
            "Time Delay Routine B",
            "Test description",
            "time.png",
            TriggerType.Time,
            JsonSerializer.Serialize(new { ScheduledTime = triggerTime, RepeatDays = 127 })
        );

        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.Notification, "Time Trigger fired natively!", 1));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.RunCommand, "ping 127.0.0.1 -n 2", 2));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.LaunchApp, "mspaint.exe", 5));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.LaunchApp, "notepad.exe", 3));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.KillProcess, "calc.exe", 4));

        await uow.Routines.AddAsync(routineA);
        await uow.Routines.AddAsync(routineB);
        await uow.SaveChangesAsync();

        _logger.LogInformation($"Database seeded successfully.");
        _logger.LogInformation($"Routine A: WiFi 'A_7'");
        _logger.LogInformation($"Routine B: Scheduled to fire at {triggerTime:HH:mm:ss}");
    }
}
