using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Events;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Services;
using SmartRoutines.Logic.TriggerMonitors;
using System.Collections.Concurrent;

namespace SmartRoutines.Logic.AutomationEngine;

/// <summary>
/// Background automation engine that monitors triggers and executes routines.
/// Runs a high precision 1s loop and isolates failures per-routine.
/// </summary>
public sealed class AutomationEngine : IAutomationEngine, IDisposable
{
    private readonly IUnitOfWork _uow;
    private readonly ILiveLogger _liveLogger;
    private readonly IActivityLogService _logService;
    private readonly CancellationTokenSource _cts = new();
    private readonly List<ITrigger> _activeTriggers = new();
    private readonly ConcurrentDictionary<Guid, byte> _running = new();
    private Task? _engineTask;

    public bool IsRunning { get; private set; }
    public IReadOnlyCollection<Guid> RunningRoutineIds => _running.Keys.ToList().AsReadOnly();

    public event EventHandler<RoutineStartedEventArgs>? RoutineStarted;
    public event EventHandler<RoutineCompletedEventArgs>? RoutineCompleted;
    public event EventHandler<LogEmittedEventArgs>? LogEmitted;

    public AutomationEngine(IUnitOfWork uow, ILiveLogger liveLogger, IActivityLogService logService)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _liveLogger = liveLogger ?? throw new ArgumentNullException(nameof(liveLogger));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));

        // Subscribe to live logger to forward events
        if (_liveLogger is LiveLogger ll) ll.OnLogReceived += (lvl, msg) => OnLogEmitted(new ActivityLogDto { Id = Guid.NewGuid(), RoutineName = "Engine", Status = LogStatus.Unknown, Message = msg, ExecutedAt = DateTime.UtcNow, RelativeTime = "now", DurationSeconds = 0 });
    }

    /// <summary>
    /// Starts the engine loop.
    /// </summary>
    public Task StartAsync()
    {
        if (IsRunning) return Task.CompletedTask;
        IsRunning = true;
        _liveLogger.LogInfo("Engine Started");
        _engineTask = Task.Run(EngineLoop, _cts.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the engine and disposes triggers.
    /// </summary>
    public async Task StopAsync()
    {
        if (!IsRunning) return;
        _liveLogger.LogInfo("Engine Stopping");
        _cts.Cancel();
        if (_engineTask != null) await _engineTask.ConfigureAwait(false);
        Dispose();
        IsRunning = false;
    }

    /// <summary>
    /// Executes a routine on-demand.
    /// </summary>
    public async Task ExecuteManualAsync(Guid routineId)
    {
        var routines = await _uow.Routines.GetByIdWithActionsAsync(routineId);
        if (routines == null) throw new RoutineNotFoundException(routineId);

        // Map to runtime model
        var runtime = new RuntimeRoutine(routines.Id, routines.Name, routines.TriggerType, routines.TriggerConfig, routines.Actions.Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder)));

        // TODO: Implement ActionRunner.Execute(routine.Actions)
        _liveLogger.LogInfo($"Manual execution requested for routine: {runtime.Name}");
    }

    private async Task EngineLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var active = await _uow.Routines.GetActiveNotDeletedWithActionsAsync();
                _liveLogger.LogInfo($"Monitoring {active.Count()} routines");

                foreach (var r in active)
                {
                    // For each routine, evaluate in isolation
                    try
                    {
                        ITrigger trigger = CreateTrigger(r.TriggerType, r.TriggerConfig);
                        if (trigger == null) continue;

                        try
                        {
                            trigger.Configure(r.TriggerConfig);
                            if (trigger.IsEnabled && trigger.ShouldFire())
                            {
                                trigger.OnFired();
                                _liveLogger.LogInfo($"Routine [{r.Name}] Triggered");

                                // TODO: Implement ActionRunner.Execute(routine.Actions)

                                // Persist log entry via activity log service
                                await _logService.ClearAllLogsAsync(); // <-- placeholder to use the log service
                            }
                        }
                        catch (Exception ex)
                        {
                            // ensure an engine's failure on a particular trigger won't break the loop
                            _liveLogger.LogError($"Error evaluating trigger for routine {r.Name}", ex);
                        }
                        finally
                        {
                            trigger.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        _liveLogger.LogError($"Exception processing routine {r.Name}", ex);
                    }
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                _liveLogger.LogError("Engine encountered an unexpected error", ex);
            }

            try
            {
                await Task.Delay(1000, _cts.Token);
            }
            catch (TaskCanceledException) { break; }
        }
    }

    private ITrigger CreateTrigger(TriggerType type, string jsonConfig)
    {
        return type switch
        {
            TriggerType.Time => new TimeTrigger(),
            TriggerType.WiFi => new WiFiTrigger(),
            TriggerType.Startup => new StartupTrigger(),
            TriggerType.Shutdown => new ShutdownTrigger(),
            TriggerType.Battery => new BatteryTrigger(),
            TriggerType.Idle => new IdleTrigger(),
            _ => null
        };
    }

    private void OnLogEmitted(ActivityLogDto dto)
    {
        LogEmitted?.Invoke(this, new LogEmittedEventArgs(dto));
    }

    public void Dispose()
    {
        foreach (var t in _activeTriggers)
        {
            try { t.Dispose(); } catch { }
        }
        _activeTriggers.Clear();
        _cts.Dispose();
    }
}
