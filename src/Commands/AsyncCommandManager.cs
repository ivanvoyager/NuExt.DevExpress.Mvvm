using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;

namespace DevExpress.Mvvm;

/// <summary>
/// Manages instances of <see cref="IAsyncCommand"/>.
/// </summary>
public class AsyncCommandManager : IAsyncCommandManager
{
    private const byte InitialState = 0;
    private const byte RequestToRemove = 1;

    private readonly ConcurrentDictionary<IAsyncCommand, byte> _commands = new();

    /// <summary>
    /// Gets the shared instance of the <see cref="AsyncCommandManager"/>.
    /// </summary>
    public static readonly IAsyncCommandManager Shared = new AsyncCommandManager();

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncCommandManager"/> class.
    /// </summary>
    public AsyncCommandManager()
    {

    }

#if DEBUG
    /// <summary>
    /// Gets the collection of registered commands for debugging purposes.
    /// </summary>
    public ConcurrentDictionary<IAsyncCommand, byte> Commands => _commands;
#endif
    /// <summary>
    /// Gets the count of registered commands.
    /// </summary>
    public int Count => _commands.Count;

    /// <summary>
    /// Gets a value indicating whether there are any async commands executing.
    /// </summary>
    public bool HasActiveCommands => _commands.Any(pair => pair.Key.IsExecuting);

    /// <summary>
    /// Handles the PropertyChanged event of an <see cref="IAsyncCommand"/> to manage its execution state.
    /// </summary>
    private void AsyncCommand_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is IAsyncCommand command && e.PropertyName == nameof(IAsyncCommand.IsExecuting))
        {
            if (command.IsExecuting == false)
            {
                Debug.Assert(command.CancellationTokenSource != null);
                command.CancellationTokenSource?.Dispose();
                if (_commands.TryGetValue(command, out var value) && value == RequestToRemove)
                {
                    RemoveAndUnsubscribeCommand(command);
                }
            }
        }
    }

    /// <summary>
    /// Determines whether all asynchronous commands in the collection satisfy a specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each command for a condition.</param>
    /// <returns><see langword="true"/> if every command in the collection matches the specified condition; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="predicate"/> is null.</exception>
    /// <remarks>
    /// This method iterates through each command in the internal collection and applies the specified predicate.
    /// It returns <see langword="true"/> only if the predicate returns <see langword="true"/> for all commands; otherwise, it returns <see langword="false"/>.
    /// </remarks>
    public bool All(Func<IAsyncCommand, bool> predicate)
    {
        return _commands.All(pair => predicate(pair.Key));
    }

    /// <summary>
    /// Cancels all executing asynchronous commands.
    /// </summary>
    /// <remarks>
    /// This method iterates through all commands stored in the internal collection
    /// and calls the <see cref="IAsyncCommand.CancelCommand"/> on each one.
    /// </remarks>
    public void CancelAll()
    {
        foreach (var pair in _commands)
        {
            pair.Key.Cancel();
        }
    }

    /// <summary>
    /// Registers an <see cref="IAsyncCommand"/> with the manager.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <returns>The registered command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public IAsyncCommand Register(IAsyncCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!_commands.TryAdd(command, InitialState)) return command;
        if (command is INotifyPropertyChanged p)
        {
            p.PropertyChanged += AsyncCommand_PropertyChanged;
        }
        return command;
    }

    /// <summary>
    /// Unregisters an <see cref="IAsyncCommand"/> from the manager.
    /// </summary>
    /// <param name="command">The command to unregister.</param>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public void Unregister(IAsyncCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_commands.TryGetValue(command, out var value) && value == InitialState)
        {
            if (command.IsExecuting == false)
            {
                RemoveAndUnsubscribeCommand(command);
                return;
            }
            if (!_commands.TryUpdate(command, RequestToRemove, InitialState))
            {
                Debug.Assert(false, $"Failed to update command status to {RequestToRemove}.");
            }
        }
    }

    /// <summary>
    /// Removes and unsubscribes an <see cref="IAsyncCommand"/> from property change notifications.
    /// </summary>
    /// <param name="command">The command to remove and unsubscribe.</param>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    private void RemoveAndUnsubscribeCommand(IAsyncCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (_commands.TryRemove(command, out _) && command is INotifyPropertyChanged p)
        {
            p.PropertyChanged -= AsyncCommand_PropertyChanged;
            return;
        }
        Debug.Assert(false, "Failed to remove or unsubscribe command.");
    }

    /// <summary>
    /// Waits asynchronously until all commands in the collection have finished executing.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the wait is canceled.</exception>
    /// <remarks>
    /// This method waits for all commands stored in the internal collection to complete execution.
    /// If the collection is empty, the method returns immediately.
    /// The operation can be canceled using the provided <paramref name="cancellationToken"/>.
    /// </remarks>
    public async ValueTask WaitAll(CancellationToken cancellationToken = default)
    {
        var commands = _commands.Keys;
        if (commands.Count == 0) return;
        await Task.WhenAll(commands.Where(command => command.IsExecuting).Select(command => command.WaitAsync(cancellationToken)));
    }
}
