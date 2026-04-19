using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Events;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SmartRoutines.Logic.AutomationEngine;

/// <summary>
/// High-performance background automation engine.
/// - Runs a 1s heartbeat loop
/// - Caches trigger instances to avoid recreation
/// - Executes routines in isolated "fire-and-forget" tasks
/// - Persists execution summaries via <see cref="IActivityLogService"/>
/// </summary>
public sealed class AutomationEngine : IAutomationEngine, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILiveLogger _liveLogger;
    private readonly CancellationTokenSource _cts = new();

    // Trigger cache: keep long-lived trigger instances in memory keyed by Routine.Id
    private readonly ConcurrentDictionary<Guid, ITrigger> _triggerCache = new();
    // Keep last-known config to detect updates
    private readonly ConcurrentDictionary<Guid, string> _triggerConfig = new();

    // Running guard: stores per-routine CancellationTokenSource to allow cancelling individual runs
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _runningCts = new();

    private Task? _engineTask;

    public bool IsRunning { get; private set; }
    public IReadOnlyCollection<Guid> RunningRoutineIds => _runningCts.Keys.ToList().AsReadOnly();

    public event EventHandler<RoutineStartedEventArgs>? RoutineStarted;
    public event EventHandler<RoutineCompletedEventArgs>? RoutineCompleted;
    public event EventHandler<LogEmittedEventArgs>? LogEmitted;

    public AutomationEngine(IServiceScopeFactory scopeFactory, ILiveLogger liveLogger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _liveLogger = liveLogger ?? throw new ArgumentNullException(nameof(liveLogger));

        // Forward live logger emissions into engine events for UI subscription
        if (_liveLogger is SmartRoutines.Logic.Services.LiveLogger ll)
            ll.OnLogReceived += (lvl, msg) => OnLogEmitted(new ActivityLogDto { Id = Guid.NewGuid(), RoutineName = "Engine", Status = LogStatus.Unknown, Message = msg, ExecutedAt = DateTime.UtcNow, RelativeTime = "now", DurationSeconds = 0 });
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        if (IsRunning) return Task.CompletedTask;
        IsRunning = true;
        _liveLogger.LogInfo("Automation engine started");
        _engineTask = Task.Run(() => EngineLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync()
    {
        if (!IsRunning) return;
        _liveLogger.LogInfo("Automation engine stopping");
        _cts.Cancel();
        if (_engineTask != null) await _engineTask.ConfigureAwait(false);

        // Dispose cached triggers
        foreach (var kv in _triggerCache)
        {
            try { kv.Value.Dispose(); } catch { }
        }
        _triggerCache.Clear();
        _triggerConfig.Clear();

        IsRunning = false;
    }

    /// <inheritdoc />
    public async Task ExecuteManualAsync(Guid routineId)
    {
        await RunRoutineNowAsync(routineId).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs a routine immediately, creating a per-run CancellationTokenSource to allow cancellation.
    /// </summary>
    public async Task RunRoutineNowAsync(Guid routineId)
    {
        string routineName;
        using (var scope = _scopeFactory.CreateScope())
        {
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var entity = await uow.Routines.GetByIdWithActionsAsync(routineId);
            if (entity == null) throw new RoutineNotFoundException(routineId);
            routineName = entity.Name;
        }

        // Prevent double-run
        if (_runningCts.ContainsKey(routineId))
        {
            _liveLogger.LogWarning($"Routine {routineName} is already running.");
            return;
        }

        var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
        if (!_runningCts.TryAdd(routineId, perRunCts))
        {
            perRunCts.Dispose();
            _liveLogger.LogWarning($"Failed to start manual run for {routineName} (already running).");
            return;
        }

        // Fire started event
        RoutineStarted?.Invoke(this, new RoutineStartedEventArgs(routineId));

        _ = Task.Run(async () =>
        {
            var sw = Stopwatch.StartNew();
            LogStatus status = LogStatus.Success;
            string message = string.Empty;
            Exception? error = null;
            try
            {
                _liveLogger.LogInfo($"Manual execution started: {routineName}");

                RuntimeRoutine? runtime = null;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var entity = await uow.Routines.GetByIdWithActionsAsync(routineId);
                    if (entity != null)
                    {
                        runtime = new RuntimeRoutine(entity.Id, entity.Name, entity.TriggerType, entity.TriggerConfig,
                            entity.Actions.OrderBy(a => a.ExecutionOrder).Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder)));
                    }
                }

                if (runtime == null) throw new RoutineNotFoundException(routineId);

                using var runScope = _scopeFactory.CreateScope();
                var runner = runScope.ServiceProvider.GetRequiredService<ActionRunner>();
                var ctx = new ActionContext { RoutineName = runtime.Name, TriggerTime = DateTime.UtcNow, TriggerData = new Dictionary<string, object?>(), IsManualTrigger = true };
                await runner.RunAsync(runtime.Actions, ctx, perRunCts.Token).ConfigureAwait(false);
                message = $"Manual executed {runtime.Actions.Count} actions.";
            }
            catch (OperationCanceledException)
            {
                status = LogStatus.Warning;
                message = "Manual execution cancelled.";
                _liveLogger.LogWarning($"Manual run cancelled for {routineName}");
            }
            catch (Exception ex)
            {
                status = LogStatus.Error;
                message = ex.Message;
                error = ex;
                _liveLogger.LogError($"Manual execution failed for {routineName}", ex);
            }
            finally
            {
                sw.Stop();
                try
                {
                    using var logScope = _scopeFactory.CreateScope();
                    var logService = logScope.ServiceProvider.GetRequiredService<IActivityLogService>();
                    await logService.AddLogAsync(new ActivityLogDto
                    {
                        Id = Guid.NewGuid(),
                        RoutineName = routineName,
                        Status = status,
                        Message = message,
                        ExecutedAt = DateTime.UtcNow,
                        RelativeTime = "now",
                        DurationSeconds = (int)sw.Elapsed.TotalSeconds
                    });
                }
                catch (Exception ex)
                {
                    _liveLogger.LogError("Failed to persist manual execution log", ex);
                }

                RoutineCompleted?.Invoke(this, new RoutineCompletedEventArgs(routineId, error == null, error));

                if (_runningCts.TryRemove(routineId, out var existing))
                {
                    try { existing.Dispose(); } catch { }
                }
            }
        }, perRunCts.Token);
    }

    private const int MaxConcurrencyLimit = 5;

    public string GetRoutineDiagnostic(Guid routineId)
    {
        if (_triggerCache.TryGetValue(routineId, out var trigger))
        {
            return trigger.GetDiagnosticInfo();
        }
        return "Not monitoring";
    }

    private async Task EngineLoopAsync(CancellationToken ct)
    {
        // 1s precision heartbeat
        while (!ct.IsCancellationRequested)
        {
            try
            {
                IReadOnlyList<SmartRoutines.Core.Domain.Entities.Routine> active;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    active = (await uow.Routines.GetActiveNotDeletedWithActionsAsync()).ToList();
                }

                var activeSet = active.Select(r => r.Id).ToHashSet();

                // 1. Maintain Trigger Cache
                foreach (var r in active)
                {
                    var cfg = r.TriggerConfig ?? string.Empty;
                    if (_triggerCache.TryGetValue(r.Id, out var existing))
                    {
                        if (!_triggerConfig.TryGetValue(r.Id, out var old) || old != cfg)
                        {
                            try { existing.Configure(cfg); _triggerConfig[r.Id] = cfg; } catch { }
                        }
                    }
                    else
                    {
                        var trig = TriggerFactory.Create(r.TriggerType);
                        if (trig != null)
                        {
                            try 
                            { 
                                trig.Configure(cfg); 
                                
                                // NEW: If the trigger's condition is ALREADY met upon creation,
                                // we treat it as having "fired" so it waits for the next occurrence.
                                if (trig.IsEnabled && await trig.ShouldFireAsync())
                                {
                                    trig.OnFired();
                                }

                                _triggerCache[r.Id] = trig; 
                                _triggerConfig[r.Id] = cfg; 
                            } 
                            catch { }
                        }
                    }
                }

                // 2. Cleanup stale triggers
                foreach (var key in _triggerCache.Keys)
                {
                    if (!activeSet.Contains(key))
                    {
                        if (_triggerCache.TryRemove(key, out var t)) { try { t.Dispose(); } catch { } _triggerConfig.TryRemove(key, out _); }
                    }
                }

                // 3. Evaluate and Fire
                foreach (var r in active)
                {
                    if (ct.IsCancellationRequested) break;
                    
                    // Don't fire if already running
                    if (_runningCts.ContainsKey(r.Id)) continue;
                    
                    if (!_triggerCache.TryGetValue(r.Id, out var trigger)) continue;

                    if (trigger.IsEnabled && await trigger.ShouldFireAsync())
                    {
                        // Check concurrency limit before dispatching
                        if (_runningCts.Count >= MaxConcurrencyLimit)
                        {
                            _liveLogger.LogWarning($"Routine '{r.Name}' trigger skipped: Max concurrency limit ({MaxConcurrencyLimit}) reached.");
                            continue;
                        }

                        trigger.OnFired();
                        _liveLogger.LogInfo($"Trigger fired: {r.Name}");
                        
                        // Fire-and-forget execution
                        _ = DispatchExecutionAsync(r.Id, r.Name, ct);
                    }
                }
            }
            catch (Exception ex) { _liveLogger.LogError("Engine heartbeat error", ex); }

            try { await Task.Delay(1000, ct); } catch (TaskCanceledException) { break; }
        }
    }

    private async Task DispatchExecutionAsync(Guid routineId, string routineName, CancellationToken ct)
    {
        // Note: Re-check concurrency just in case of race between loops (minimal risk)
        if (_runningCts.Count >= MaxConcurrencyLimit) return;

        var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        if (!_runningCts.TryAdd(routineId, perRunCts))
        {
            perRunCts.Dispose();
            return;
        }

        Exception? error = null;
        var sw = Stopwatch.StartNew();
        LogStatus status = LogStatus.Success;
        string message = string.Empty;

        try
        {
            RuntimeRoutine? runtime = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var entity = await uow.Routines.GetByIdWithActionsAsync(routineId);
                if (entity != null)
                {
                    runtime = new RuntimeRoutine(entity.Id, entity.Name, entity.TriggerType, entity.TriggerConfig,
                        entity.Actions.OrderBy(a => a.ExecutionOrder).Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder)));
                }
            }

            if (runtime == null) return;

            using (var runScope = _scopeFactory.CreateScope())
            {
                var runner = runScope.ServiceProvider.GetRequiredService<ActionRunner>();
                var ctx = new ActionContext { RoutineName = runtime.Name, TriggerTime = DateTime.UtcNow, TriggerData = new Dictionary<string, object?>(), IsManualTrigger = false };
                await runner.RunAsync(runtime.Actions, ctx, perRunCts.Token).ConfigureAwait(false);
                message = $"Executed {runtime.Actions.Count} actions.";
            }
        }
        catch (OperationCanceledException)
        {
            status = LogStatus.Warning;
            message = "Execution cancelled.";
        }
        catch (Exception ex)
        {
            status = LogStatus.Error;
            message = ex.Message;
            error = ex;
            _liveLogger.LogError($"Error in routine {routineName}", ex);
        }
        finally
        {
            sw.Stop();
            try
            {
                if (status != LogStatus.Success || !string.IsNullOrEmpty(message))
                {
                    using var logScope = _scopeFactory.CreateScope();
                    var logService = logScope.ServiceProvider.GetRequiredService<IActivityLogService>();
                    await logService.AddLogAsync(new ActivityLogDto
                    {
                        Id = Guid.NewGuid(),
                        RoutineName = routineName,
                        Status = status,
                        Message = message,
                        ExecutedAt = DateTime.UtcNow,
                        RelativeTime = "now",
                        DurationSeconds = (int)sw.Elapsed.TotalSeconds
                    });
                }
            }
            catch { }

            RoutineCompleted?.Invoke(this, new RoutineCompletedEventArgs(routineId, error == null, error));
            if (_runningCts.TryRemove(routineId, out var existingCts)) { try { existingCts.Dispose(); } catch { } }
        }
    }

    private void OnLogEmitted(ActivityLogDto dto) => LogEmitted?.Invoke(this, new LogEmittedEventArgs(dto));

    public void Dispose()
    {
        foreach (var kv in _triggerCache) { try { kv.Value.Dispose(); } catch { } }
        _triggerCache.Clear(); _triggerConfig.Clear(); _cts.Dispose();
    }
}
