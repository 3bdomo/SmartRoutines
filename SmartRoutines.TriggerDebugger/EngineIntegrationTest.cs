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
        _logger.LogInformation("Seeding database with ALL triggers and ALL actions...");

        // Routine A (WiFi Connection - MEGA ROUTINE 1)
        var routineA = new Routine(
            "Mega WiFi Routine A",
            "Tests all action types",
            "wifi.png",
            TriggerType.WiFi,
            JsonSerializer.Serialize(new { SsidName = "A_7" })
        );

        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.Notification, "WiFi A_7 Connected!", 1));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.RunCommand, "echo Hello from WiFi Routine A", 2));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.Mute, "true", 3));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.KillProcess, "notepad", 4));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.LaunchApp, "calculator", 5));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.OpenUrl, "https://github.com", 6));
        routineA.AddAction(new ActionEntry(routineA.Id, ActionType.SetVolume, "50", 7));

        // Routine B (Time + 5s - MEGA ROUTINE 2)
        var triggerTime = DateTime.Now.AddSeconds(5);
        var routineB = new Routine(
            "Mega Time Routine B",
            "Tests time trigger with multiple actions",
            "time.png",
            TriggerType.Time,
            JsonSerializer.Serialize(new { ScheduledTime = triggerTime, RepeatDays = 127 })
        );

        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.Notification, "Time Trigger fired natively!", 1));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.RunCommand, "ping 127.0.0.1 -n 2", 2));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.LaunchApp, "notepad.exe", 3));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.KillProcess, "calc.exe", 4));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.LaunchApp, "mspaint.exe", 5));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.SetVolume, "100", 6));
        routineB.AddAction(new ActionEntry(routineB.Id, ActionType.OpenUrl, "https://microsoft.com", 7));

        // Routine C (Battery)
        var routineC = new Routine("Battery Routine C", "Battery check", "battery.png", TriggerType.Battery, JsonSerializer.Serialize(new { BatteryThreshold = 20 }));
        routineC.AddAction(new ActionEntry(routineC.Id, ActionType.Notification, "Battery below 20%!", 1));

        // Routine D (Startup)
        var routineD = new Routine("Startup Routine D", "Startup check", "startup.png", TriggerType.Startup, "{}");
        routineD.AddAction(new ActionEntry(routineD.Id, ActionType.Notification, "System Started!", 1));

        // Routine E (Shutdown)
        var routineE = new Routine("Shutdown Routine E", "Shutdown check", "shutdown.png", TriggerType.Shutdown, "{}");
        routineE.AddAction(new ActionEntry(routineE.Id, ActionType.Notification, "System Shutting Down!", 1));

        // Routine F (Idle)
        var routineF = new Routine("Idle Routine F", "Idle check", "idle.png", TriggerType.Idle, JsonSerializer.Serialize(new { IdleMinutes = 5 }));
        routineF.AddAction(new ActionEntry(routineF.Id, ActionType.Notification, "User is idle!", 1));

        // Routine G (AppOpen)
        var routineG = new Routine("AppOpen Routine G", "AppOpen check", "app.png", TriggerType.AppOpen, JsonSerializer.Serialize(new { SsidName = "chrome" }));
        routineG.AddAction(new ActionEntry(routineG.Id, ActionType.Notification, "Chrome Opened!", 1));

        // Routine H (ProcessClose)
        var routineH = new Routine("ProcessClose Routine H", "ProcessClose check", "process.png", TriggerType.ProcessClose, JsonSerializer.Serialize(new { SsidName = "notepad" }));
        routineH.AddAction(new ActionEntry(routineH.Id, ActionType.Notification, "Notepad Closed!", 1));

        // Routine I (AppLaunched)
        var routineI = new Routine("AppLaunched Routine I", "AppLaunched check", "app2.png", TriggerType.AppLaunched, JsonSerializer.Serialize(new { SsidName = "calculator" }));
        routineI.AddAction(new ActionEntry(routineI.Id, ActionType.Notification, "Calculator Launched!", 1));

        // Routine J (FileChanged)
        var routineJ = new Routine("FileChanged Routine J", "FileChanged check", "file.png", TriggerType.FileChanged, JsonSerializer.Serialize(new { SsidName = "C:\\test.txt" }));
        routineJ.AddAction(new ActionEntry(routineJ.Id, ActionType.Notification, "Test file changed!", 1));

        await uow.Routines.AddAsync(routineA);
        await uow.Routines.AddAsync(routineB);
        await uow.Routines.AddAsync(routineC);
        await uow.Routines.AddAsync(routineD);
        await uow.Routines.AddAsync(routineE);
        await uow.Routines.AddAsync(routineF);
        await uow.Routines.AddAsync(routineG);
        await uow.Routines.AddAsync(routineH);
        await uow.Routines.AddAsync(routineI);
        await uow.Routines.AddAsync(routineJ);
        
        await uow.SaveChangesAsync();

        _logger.LogInformation($"Database seeded successfully with ALL 10 Triggers and ALL 7 Actions.");
    }
}
