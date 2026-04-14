using SmartRoutines.Core.Domain.Entities;
using SmartRoutines.Core.Domain.Enums;
using SmartRoutines.Core.Domain.Models;

namespace SmartRoutines.Core.Interfaces.Logic;

/// <summary>
/// Defines a strategy that can execute one or more action types in the routine pipeline.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Gets the action types handled by this executor.
    /// </summary>
    IReadOnlyCollection<ActionType> SupportedActionTypes { get; }

    /// <summary>
    /// Executes the provided action entry asynchronously.
    /// </summary>
    /// <param name="entry">The action entry to execute.</param>
    /// <param name="context">The runtime context of the active routine.</param>
    /// <param name="cancellationToken">A token that can cancel the execution request.</param>
    /// <returns>A task that completes when execution finishes.</returns>
    Task ExecuteAsync(ActionEntry entry, ActionContext context, CancellationToken cancellationToken = default);
}