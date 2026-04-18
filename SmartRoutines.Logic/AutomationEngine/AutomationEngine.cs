using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Domain.Entities;
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

    private readonly IServiceScopeFactory _scopeFactory;

    public AutomationEngine(ILiveLogger liveLogger, IServiceScopeFactory scopeFactory)
    {
        _liveLogger = liveLogger ?? throw new ArgumentNullException(nameof(liveLogger));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

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
        // Use RunRoutineNowAsync to centralize manual execution logic
        await RunRoutineNowAsync(routineId).ConfigureAwait(false);
    }

    /// <summary>
    /// Runs a routine immediately, creating a per-run CancellationTokenSource to allow cancellation.
    /// </summary>
    public async Task RunRoutineNowAsync(Guid routineId)
    {
        var runtime = await GetRuntimeRoutineByIdAsync(routineId).ConfigureAwait(false);
        if (runtime == null) throw new RoutineNotFoundException(routineId);

        // Prevent double-run
        if (_runningCts.ContainsKey(routineId))
        {
            _liveLogger.LogWarning($"Routine {runtime.Name} is already running.");
            return;
        }

        var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
        if (!_runningCts.TryAdd(routineId, perRunCts))
        {
            perRunCts.Dispose();
            _liveLogger.LogWarning($"Failed to start manual run for {runtime.Name} (already running).");
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
                _liveLogger.LogInfo($"Manual execution started: {runtime.Name}");

                using var scope = _scopeFactory.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<ActionRunner>();
                var ctx = new ActionContext { RoutineName = runtime.Name, TriggerTime = DateTime.UtcNow, TriggerData = new Dictionary<string, object?>(), IsManualTrigger = true };
                await runner.RunAsync(runtime.Actions, ctx, perRunCts.Token).ConfigureAwait(false);
                message = $"Manual executed {runtime.Actions.Count} actions.";
            }
            catch (OperationCanceledException)
            {
                status = LogStatus.Warning;
                message = "Manual execution cancelled.";
                _liveLogger.LogWarning($"Manual run cancelled for {runtime.Name}");
            }
            catch (Exception ex)
            {
                status = LogStatus.Error;
                message = ex.Message;
                error = ex;
                _liveLogger.LogError($"Manual execution failed for {runtime.Name}", ex);
            }
            finally
            {
                sw.Stop();
                try
                {
                    await PersistLogAsync(new ActivityLogDto
                    {
                        Id = runtime.Id,
                        RoutineName = runtime.Name,
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

                // Completion
                RoutineCompleted?.Invoke(this, new RoutineCompletedEventArgs(routineId, error == null, error));

                if (_runningCts.TryRemove(routineId, out var existing))
                {
                    try { existing.Dispose(); } catch { }
                }
            }
        }, perRunCts.Token);
    }

    /// <summary>
    /// Cancels a running routine if present.
    /// </summary>
    public void StopRoutine(Guid routineId)
    {
        if (_runningCts.TryRemove(routineId, out var cts))
        {
            try { cts.Cancel(); } catch { }
            try { cts.Dispose(); } catch { }
            _liveLogger.LogInfo($"Stop requested for routine {routineId}");
        }
    }

    private async Task EngineLoopAsync(CancellationToken ct)
    {
        // Heartbeat loop: high precision 1s interval, non-blocking
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var active = await GetActiveRuntimeRoutinesAsync().ConfigureAwait(false);
                _liveLogger.LogInfo($"Engine monitoring {active.Count()} routines");

                var activeSet = active.Select(r => r.Id).ToHashSet();

                // Add or update triggers in cache
                foreach (var r in active)
                {
                    var cfg = r.TriggerConfig ?? string.Empty;
                    if (_triggerCache.TryGetValue(r.Id, out var existing))
                    {
                        // Re-configure only if JSON changed
                        if (!_triggerConfig.TryGetValue(r.Id, out var old) || old != cfg)
                        {
                            try { existing.Configure(cfg); _triggerConfig[r.Id] = cfg; }
                            catch (Exception ex) { _liveLogger.LogError($"Failed to configure trigger for {r.Name}", ex); }
                        }
                    }
                    else
                    {
                        var trig = TriggerFactory.Create(r.Type);
                        if (trig != null)
                        {
                            try { trig.Configure(cfg); _triggerCache[r.Id] = trig; _triggerConfig[r.Id] = cfg; }
                            catch (Exception ex) { _liveLogger.LogError($"Failed to create/config trigger for {r.Name}", ex); }
                        }
                    }
                }

                // Remove triggers for routines no longer active
                foreach (var key in _triggerCache.Keys)
                {
                    if (!activeSet.Contains(key))
                    {
                        if (_triggerCache.TryRemove(key, out var t))
                        {
                            try { t.Dispose(); } catch { }
                            _triggerConfig.TryRemove(key, out _);
                        }
                    }
                }

                // Evaluate triggers and dispatch executions without blocking the loop
                foreach (var r in active)
                {
                    if (ct.IsCancellationRequested) break;

                    // Prevent overlapping execution: create per-run CTS linked to engine token
                    if (_runningCts.ContainsKey(r.Id)) continue;

                    var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    if (!_runningCts.TryAdd(r.Id, perRunCts))
                    {
                        perRunCts.Dispose();
                        continue;
                    }

                    // Execute asynchronously so loop keeps ticking
                    _ = Task.Run(async () =>
                    {
                        Exception? error = null;
                        var sw = Stopwatch.StartNew();
                        LogStatus status = LogStatus.Success;
                        string message = string.Empty;
                        try
                        {
                            if (!_triggerCache.TryGetValue(r.Id, out var trigger))
                            {
                                _liveLogger.LogWarning($"No trigger instance for routine {r.Name}");
                                return;
                            }

                            if (trigger.IsEnabled && trigger.ShouldFire())
                            {
                                trigger.OnFired();
                                _liveLogger.LogInfo($"Trigger fired for routine {r.Name}");

                                // Execute via ActionRunner resolved from a scope to obtain transient IAction implementations
                                try
                                {
                                    using var scope = _scopeFactory.CreateScope();
                                    var runner = scope.ServiceProvider.GetRequiredService<ActionRunner>();
                                    var ctx = new ActionContext { RoutineName = r.Name, TriggerTime = DateTime.UtcNow, TriggerData = new Dictionary<string, object?>(), IsManualTrigger = false };
                                    await runner.RunAsync(r.Actions, ctx, perRunCts.Token).ConfigureAwait(false);
                                    message = $"Executed {r.Actions.Count} actions.";
                                }
                                catch (OperationCanceledException)
                                {
                                    status = LogStatus.Warning;
                                    message = "Execution cancelled.";
                                    _liveLogger.LogWarning($"Routine {r.Name} execution was cancelled.");
                                }
                                catch (Exception ex)
                                {
                                    status = LogStatus.Error;
                                    message = ex.Message;
                                    error = ex;
                                    _liveLogger.LogError($"Error executing routine {r.Name}", ex);
                                }
                            }
                        }
                        finally
                        {
                            sw.Stop();
                            try
                            {
                                await PersistLogAsync(new ActivityLogDto
                                {
                                    Id = r.Id,
                                    RoutineName = r.Name,
                                    Status = status,
                                    Message = message,
                                    ExecutedAt = DateTime.UtcNow,
                                    RelativeTime = "now",
                                    DurationSeconds = (int)sw.Elapsed.TotalSeconds
                                });
                            }
                            catch (Exception ex)
                            {
                                _liveLogger.LogError("Failed to persist execution log", ex);
                            }

                            // Fire completion event
                            RoutineCompleted?.Invoke(this, new RoutineCompletedEventArgs(r.Id, error == null, error));

                            // Remove running token
                            if (_runningCts.TryRemove(r.Id, out var existingCts))
                            {
                                try { existingCts.Dispose(); } catch { }
                            }
                        }
                    }, perRunCts.Token);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _liveLogger.LogError("Engine loop error", ex);
            }

            try { await Task.Delay(1000, ct); } catch (TaskCanceledException) { break; }
        }
    }

    private void OnLogEmitted(ActivityLogDto dto) => LogEmitted?.Invoke(this, new LogEmittedEventArgs(dto));

    private async Task<IReadOnlyList<RuntimeRoutine>> GetActiveRuntimeRoutinesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var entities = await uow.Routines.GetActiveNotDeletedWithActionsAsync().ConfigureAwait(false);
        return entities.Select(MapToRuntimeRoutine).ToList();
    }

    private async Task<RuntimeRoutine?> GetRuntimeRoutineByIdAsync(Guid routineId)
    {
        using var scope = _scopeFactory.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var entity = await uow.Routines.GetByIdWithActionsAsync(routineId).ConfigureAwait(false);
        return entity == null ? null : MapToRuntimeRoutine(entity);
    }

    private async Task PersistLogAsync(ActivityLogDto dto)
    {
        using var scope = _scopeFactory.CreateScope();
        var logService = scope.ServiceProvider.GetRequiredService<IActivityLogService>();
        await logService.AddLogAsync(dto).ConfigureAwait(false);
    }

    private static RuntimeRoutine MapToRuntimeRoutine(Routine entity)
    {
        return new RuntimeRoutine(
            entity.Id,
            entity.Name,
            entity.TriggerType,
            entity.TriggerConfig,
            entity.Actions.OrderBy(a => a.ExecutionOrder)
                .Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder)));
    }

    public void Dispose()
    {
        foreach (var kv in _triggerCache)
        {
            try { kv.Value.Dispose(); } catch { }
        }
        _triggerCache.Clear();
        _triggerConfig.Clear();
        _cts.Dispose();
    }
}
