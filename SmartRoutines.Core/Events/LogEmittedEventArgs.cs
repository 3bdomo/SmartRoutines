using SmartRoutines.Core.DTOs;
using System;

namespace SmartRoutines.Core.Events;

/// <summary>
/// Event arguments raised when the engine emits an activity log entry.
/// </summary>
public sealed class LogEmittedEventArgs : EventArgs
{
    public ActivityLogDto Log { get; }
    public LogEmittedEventArgs(ActivityLogDto log) => Log = log;
}
