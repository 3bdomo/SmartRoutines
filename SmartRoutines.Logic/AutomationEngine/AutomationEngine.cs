using Microsoft.Extensions.DependencyInjection;
using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;
using SmartRoutines.Core.DTOs;
using SmartRoutines.Core.Events;
using SmartRoutines.Core.Exceptions;
using SmartRoutines.Core.Interfaces.Data;
using SmartRoutines.Core.Interfaces.Logic;
using SmartRoutines.Logic.Services;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SmartRoutines.Logic.AutomationEngine;

/// <inheritdoc/>
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

    private const int MaxConcurrencyLimit = 5;

    // RULE: NO captive dependencies. Only resolve Singletons (ScopeFactory/Logger).
    public AutomationEngine(IServiceScopeFactory scopeFactory, ILiveLogger liveLogger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _liveLogger = liveLogger ?? throw new ArgumentNullException(nameof(liveLogger));

        // Forward live logger emissions into engine events for UI subscription
        if (_liveLogger is LiveLogger ll)
        {
            ll.OnLogReceived += (lvl, msg) => OnLogEmitted(new ActivityLogDto
            {
                Id = Guid.NewGuid(),
                RoutineName = "Engine",
                Status = LogStatus.Unknown,
                Message = msg,
                ExecutedAt = DateTime.UtcNow,
                RelativeTime = "now",
                DurationSeconds = 0
            });
        }
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        if (IsRunning) return Task.CompletedTask;
        IsRunning = true;
        _liveLogger.LogInfo("Automation engine started");

        // Start engine loop on a background thread
        _engineTask = Task.Run(() => EngineLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync()
    {
        if (!IsRunning) return;
        _liveLogger.LogInfo("Automation engine stopping");

        // Cancel the core heartbeat token, which cascades up to CreateLinkedTokenSource
        _cts.Cancel();
        if (_engineTask != null) await _engineTask.ConfigureAwait(false);

        // Dispose cached triggers securely
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
    /// Refactored from V2 to adhere exclusively to V1's Scoped Dependency requirements.
    /// </summary>
    public async Task RunRoutineNowAsync(Guid routineId)
    {
        // 1. Resolve routine details from short-lived scope to prevent context overlaps
        var runtime = await GetRuntimeRoutineByIdAsync(routineId).ConfigureAwait(false);
        if (runtime == null) throw new RoutineNotFoundException(routineId);

        // Prevent double-run securely
        if (_runningCts.ContainsKey(routineId))
        {
            _liveLogger.LogWarning($"Routine {runtime.Name} is already running.");
            return;
        }

        // 2. Link child CTS so it immediately cancels if Main Engine stops
        var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
        if (!_runningCts.TryAdd(routineId, perRunCts))
        {
            perRunCts.Dispose();
            _liveLogger.LogWarning($"Failed to start manual run for {runtime.Name} (already running).");
            return;
        }


        // 3. Launch isolated Task
        _ = Task.Run(async () =>
        {
            // Notify subscribers
            RoutineStarted?.Invoke(this, new RoutineStartedEventArgs(routineId));

            var sw = Stopwatch.StartNew();
            LogStatus status = LogStatus.Success;
            string message = string.Empty;
            Exception? error = null;

            try
            {
                _liveLogger.LogInfo($"Manual execution started: {runtime.Name}");

                // Resolve transient runner and dependencies freshly
                using var runScope = _scopeFactory.CreateScope();
                var runner = runScope.ServiceProvider.GetRequiredService<ActionRunner>();

                var ctx = new ActionContext
                {
                    RoutineName = runtime.Name,
                    TriggerTime = DateTime.UtcNow,
                    TriggerData = new Dictionary<string, object?>(),
                    IsManualTrigger = true
                };

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
                    // 4. Resolve activity log securely via dynamic scoped persistence helper
                    await PersistLogAsync(new ActivityLogDto
                    {
                        Id = Guid.NewGuid(),
                        RoutineName = runtime.Name,
                        Status = status,
                        Message = message,
                        ExecutedAt = DateTime.UtcNow,
                        RelativeTime = "now",
                        DurationSeconds = (int)sw.Elapsed.TotalSeconds
                    }).ConfigureAwait(false);
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

    /// <summary>
    /// Cancels a running routine if present (Ported from V2).
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
        // Heartbeat loop: high precision 1s interval
        while (!ct.IsCancellationRequested)
        {
            var loopSw = Stopwatch.StartNew();
            try
            {
                var active = await GetActiveRuntimeRoutinesAsync().ConfigureAwait(false);
                var activeSet = active.Select(r => r.Id).ToHashSet();

                // 1. Maintain Trigger Cache
                foreach (var r in active)
                {
                    var cfg = r.TriggerConfig ?? string.Empty;
                    if (_triggerCache.TryGetValue(r.Id, out var existing))
                    {
                        if (!_triggerConfig.TryGetValue(r.Id, out var old) || old != cfg)
                        {
                            try
                            {
                                existing.Configure(cfg);
                                _triggerConfig.AddOrUpdate(r.Id, cfg, (_, _) => cfg);
                            }
                            catch (Exception ex)
                            {
                                _liveLogger.LogError($"Failed to re-configure trigger for {r.Name}", ex);
                            }
                        }
                    }
                    else
                    {
                        var trig = TriggerFactory.Create(r.Type);
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

                                _triggerCache.TryAdd(r.Id, trig);
                                _triggerConfig.TryAdd(r.Id, cfg);
                            }
                            catch (Exception ex)
                            {
                                _liveLogger.LogError($"Failed to create or initial-configure trigger for {r.Name}", ex);
                            }
                        }
                    }
                }

                // 2. Cleanup stale triggers
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

                // 3. Evaluate and Fire
                foreach (var r in active)
                {
                    if (ct.IsCancellationRequested) break;

                    // Don't fire if already running
                    if (_runningCts.ContainsKey(r.Id)) continue;

                    if (!_triggerCache.TryGetValue(r.Id, out var trigger)) continue;

                    if (trigger.IsEnabled && await trigger.ShouldFireAsync() && !trigger.HasFired)
                    {
                        // Check concurrency limit before dispatching 
                        if (_runningCts.Count >= MaxConcurrencyLimit)
                        {
                            _liveLogger.LogWarning($"Routine '{r.Name}' trigger skipped: Max concurrency limit ({MaxConcurrencyLimit}) reached.");
                            continue;
                        }

                        var perRunCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

                        if (!_runningCts.TryAdd(r.Id, perRunCts))
                        {
                            perRunCts.Dispose();
                            continue;
                        }

                        trigger.OnFired();
                        _liveLogger.LogInfo($"Trigger fired: {r.Name}");

                        // Execute via fire-and-forget task using the refined dispatcher
                        _ = Task.Run(() => DispatchExecutionAsync(r, perRunCts), ct);
                    }
                }
            }
            catch (Exception ex) { _liveLogger.LogError("Engine heartbeat error", ex); }

            loopSw.Stop();
            int delayTime = 1000 - (int)loopSw.ElapsedMilliseconds;

            if (delayTime > 0)
            {
                try { await Task.Delay(delayTime, ct); } catch (TaskCanceledException) { break; }
            }
            else
            {
                _liveLogger.LogWarning($"Engine lag detected! Loop took {loopSw.ElapsedMilliseconds}ms.");
            }
        }
    }

    private async Task DispatchExecutionAsync(RuntimeRoutine routine, CancellationTokenSource perRunCts)
    {

        RoutineStarted?.Invoke(this, new RoutineStartedEventArgs(routine.Id));

        Exception? error = null;
        var sw = Stopwatch.StartNew();
        LogStatus status = LogStatus.Success;
        string message = string.Empty;

        try
        {
            // Fully encapsulated runtime block prevents DbContext collisions
            using var runScope = _scopeFactory.CreateScope();

            var runner = runScope.ServiceProvider.GetRequiredService<ActionRunner>();
            var actionCtx = new ActionContext
            {
                RoutineName = routine.Name,
                TriggerTime = DateTime.UtcNow,
                TriggerData = new Dictionary<string, object?>(),
                IsManualTrigger = false
            };

            await runner.RunAsync(routine.Actions, actionCtx, perRunCts.Token).ConfigureAwait(false);
            message = $"Successfully executed {routine.Actions.Count} actions.";
        }
        catch (OperationCanceledException)
        {
            status = LogStatus.Warning;
            message = "Routine execution was cancelled.";
        }
        catch (Exception ex)
        {
            status = LogStatus.Error;
            message = ex.Message;
            error = ex;
            _liveLogger.LogError($"Error in routine {routine.Name}", ex);
        }
        finally
        {
            sw.Stop();
            try
            {
                await PersistLogAsync(new ActivityLogDto
                {
                    Id = Guid.NewGuid(),
                    RoutineName = routine.Name,
                    Status = status,
                    Message = message,
                    ExecutedAt = DateTime.UtcNow,
                    RelativeTime = "now",
                    DurationSeconds = (int)sw.Elapsed.TotalSeconds
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _liveLogger.LogError($"Failed to persist log for {routine.Name}", ex);
            }

            RoutineCompleted?.Invoke(this, new RoutineCompletedEventArgs(routine.Id, error == null, error));

            if (_runningCts.TryRemove(routine.Id, out var existingCts))
            {
                try { existingCts.Dispose(); } catch { }
            }
        }
    }

    private void OnLogEmitted(ActivityLogDto dto) => LogEmitted?.Invoke(this, new LogEmittedEventArgs(dto));

    // =========================================================================
    // SCOPED DATABASE ACCESS HELPERS - NO CAPTIVE DEPENDENCIES
    // =========================================================================

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
            (entity.Actions ?? new List<ActionEntry>())
                .OrderBy(a => a.ExecutionOrder)
                .Select(a => new RuntimeAction(a.Type, a.Arguments, a.ExecutionOrder))
                .ToList()
        );
    }

    public void Dispose()
    {
        foreach (var kv in _triggerCache) { try { kv.Value.Dispose(); } catch { } }
        _triggerCache.Clear(); _triggerConfig.Clear(); _cts.Dispose();
    }
}
