#define DEBUG_EVENTS_
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DevExpress.Mvvm;

partial class ControlViewModel
{
    private readonly ConcurrentDictionary<string, IAsyncCommand> _asyncCommands = new();

    #region Properties

    /// <summary>
    /// Gets a value indicating whether there are any async commands executing.
    /// </summary>
    public bool HasActiveCommands => CommandManager.HasActiveCommands;

    /// <summary>
    /// Manages instances of <see cref="IAsyncCommand"/>.
    /// </summary>
    protected AsyncCommandManager CommandManager { get; } = new();

    #endregion

    #region Commands

    /// <summary>
    /// Command executed when the view is loaded.
    /// </summary>
    public ICommand? LoadedCommand
    {
        get => GetProperty(() => LoadedCommand);
        private set { SetProperty(() => LoadedCommand, value); }
    }

    /// <summary>
    /// Command executed when the view is unloaded.
    /// </summary>
    public ICommand? UnloadedCommand
    {
        get => GetProperty(() => UnloadedCommand);
        private set { SetProperty(() => UnloadedCommand, value); }
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// Method to be called when the view is loaded.
    /// </summary>
    protected virtual void OnLoaded()
    {
#if DEBUG_EVENTS
        Debug.WriteLine($"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()})::OnLoaded");
#endif
        Debug.Assert(DispatcherService != null, $"{nameof(DispatcherService)} is null");
    }

    /// <summary>
    /// Method to be called when the view is unloaded.
    /// </summary>
    protected virtual void OnUnloaded()
    {
#if DEBUG_EVENTS
        Debug.WriteLine($"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()})::OnUnloaded");
#endif
        Debug.Assert(false, "TODO remove this line");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Intended for creating and registering commands as needed.
    /// </summary>
    protected virtual void CreateCommands()
    {
        LoadedCommand = RegisterCommand(OnLoaded);
        UnloadedCommand = RegisterCommand(OnUnloaded);
    }

    /// <summary>
    /// Retrieves all commands defined in the ViewModel.
    /// </summary>
    /// <returns>A list of tuples containing command names and their corresponding <see cref="ICommand"/> instances.</returns>
    protected IList<(string Name, ICommand? Value)> GetAllCommands()
    {
        var commandProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(prop => typeof(ICommand).IsAssignableFrom(prop.PropertyType) && prop.CanRead);
        return commandProperties.Select(prop => (prop.Name, (ICommand?)prop.GetValue(this))).ToList();

    }

    /// <summary>
    /// Retrieves the asynchronous command associated with the calling method's name.
    /// </summary>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    /// <returns>The <see cref="IAsyncCommand"/> associated with the calling method's name, if found; otherwise, null.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="callerName"/> is null or empty.</exception>
    protected IAsyncCommand? GetAsyncCommand([CallerMemberName] string? callerName = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(callerName), $"{nameof(callerName)} is null or empty");
        ArgumentException.ThrowIfNullOrEmpty(callerName);
        Debug.Assert(_asyncCommands.ContainsKey(callerName), $"Can't find command {callerName}");
        if (_asyncCommands.TryGetValue(callerName, out var command))
        {
            Debug.Assert(command.IsExecuting || command.CancellationTokenSource == null);
            return command;
        }
        return default;
    }

    /// <summary>
    /// Gets the cancellation token for the currently executing asynchronous command associated with the calling method's name.
    /// </summary>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    /// <returns>The <see cref="CancellationToken"/> for the ongoing asynchronous command, if any; otherwise, a default <see cref="CancellationToken"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="callerName"/> is null or empty.</exception>
    protected CancellationToken GetCurrentCancellationToken([CallerMemberName] string? callerName = null)
    {
        var command = GetAsyncCommand(callerName);
        Debug.Assert(command is { IsExecuting: true, CancellationTokenSource.Token.CanBeCanceled: true });
        return command?.CancellationTokenSource?.Token ?? default;
    }

    /// <summary>
    /// Registers an asynchronous command internally with the specified method name.
    /// Ensures that the command is properly managed and disposed of when no longer needed.
    /// </summary>
    /// <param name="methodName">The name of the method associated with the command.</param>
    /// <param name="command">The asynchronous command to register.</param>
    private void InternalRegisterAsyncCommand(string methodName, IAsyncCommand command)
    {
        Debug.Assert(!string.IsNullOrEmpty(methodName));
        ArgumentException.ThrowIfNullOrEmpty(methodName);
        Lifetime.AddBracket(() => PropertyChanged += OnPropertyChanged, () => PropertyChanged -= OnPropertyChanged);

        var result = _asyncCommands.TryAdd(methodName, command);
        Debug.Assert(result, $"Command already registered for '{methodName}'");
        Throw.InvalidOperationExceptionIf(!result, $"Command already registered for '{methodName}'");

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Debug.Assert(ReferenceEquals(sender, this));
            if (e.PropertyName == nameof(IsDisposing) && IsDisposing)
            {
                command.Cancel();//Cancel active commands while disposing
                CommandManager.Unregister(command);
                result = _asyncCommands.TryRemove(methodName, out var asyncCommand);
                Debug.Assert(result && asyncCommand == command);
                command = null!;
            }
        }
    }

    /// <summary>
    /// Nullifies all command properties in the ViewModel.
    /// This is useful for cleanup purposes before the ViewModel is disposed.
    /// </summary>
    private void NullifyCommands()
    {
        var commandProperties = GetType().GetAllPropertiesOfType<ICommand>(typeof(ControlViewModel), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var prop in commandProperties)
        {
            if (prop.CanWrite == false)
            {
                Debug.Assert(false);
                continue;
            }
            prop.SetValue(this, null);
        }
    }

    /// <summary>
    /// Registers an asynchronous command with the specified execute method and optional can-execute method.
    /// </summary>
    /// <param name="executeMethod">The asynchronous method to execute.</param>
    /// <param name="canExecuteMethod">An optional method that determines whether the command can execute.</param>
    /// <param name="useCommandManager">An optional flag indicating whether to use the command manager.</param>
    /// <returns>The registered asynchronous command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="executeMethod"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the ViewModel has been disposed.</exception>
    public IAsyncCommand RegisterAsyncCommand(Func<Task> executeMethod, Func<bool>? canExecuteMethod = null,
        bool? useCommandManager = null)
    {
        ArgumentNullException.ThrowIfNull(executeMethod);
        CheckDisposed();
        string methodName = executeMethod.Method.Name;
        var command = CommandManager.Register(new AsyncCommand(async () =>
        {
            try
            {
                await executeMethod();
            }
            catch (Exception ex)
            {
                OnError(ex, methodName);
            }
        }, canExecuteMethod, useCommandManager));
        InternalRegisterAsyncCommand(methodName, command);
        return command;
    }

    /// <summary>
    /// Registers an asynchronous command with the specified execute method and optional can-execute method.
    /// </summary>
    /// <typeparam name="T">The type of parameter passed to the execute and can-execute methods.</typeparam>
    /// <param name="executeMethod">The asynchronous method to execute.</param>
    /// <param name="canExecuteMethod">An optional method that determines whether the command can execute with the given parameter.</param>
    /// <param name="useCommandManager">An optional flag indicating whether to use the command manager.</param>
    /// <returns>The registered asynchronous command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="executeMethod"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the ViewModel has been disposed.</exception>
    public IAsyncCommand RegisterAsyncCommand<T>(Func<T, Task> executeMethod,
        Func<T, bool>? canExecuteMethod = null, bool? useCommandManager = null)
    {
        ArgumentNullException.ThrowIfNull(executeMethod);
        CheckDisposed();
        string methodName = executeMethod.Method.Name;
        var command = CommandManager.Register(new AsyncCommand<T>(async x =>
        {
            try
            {
                await executeMethod(x);
            }
            catch (Exception ex)
            {
                OnError(ex, methodName);
            }
        }, canExecuteMethod, useCommandManager));
        InternalRegisterAsyncCommand(methodName, command);
        return command;
    }

    /// <summary>
    /// Registers a synchronous command with the specified execute method and optional can-execute method.
    /// </summary>
    /// <param name="executeMethod">The method to execute when the command is invoked.</param>
    /// <param name="canExecuteMethod">An optional method that determines whether the command can execute.</param>
    /// <param name="useCommandManager">An optional flag indicating whether to use the command manager.</param>
    /// <returns>The registered synchronous command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="executeMethod"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the ViewModel has been disposed.</exception>
    public ICommand RegisterCommand(Action executeMethod, Func<bool>? canExecuteMethod = null, bool? useCommandManager = null)
    {
        ArgumentNullException.ThrowIfNull(executeMethod);
        CheckDisposed();
        string methodName = executeMethod.Method.Name;
        return new DelegateCommand(() =>
        {
            try
            {
                executeMethod();
            }
            catch (Exception ex)
            {
                OnError(ex, methodName);
            }
        }, canExecuteMethod, useCommandManager);
    }

    /// <summary>
    /// Registers a synchronous command with the specified execute method and optional can-execute method.
    /// </summary>
    /// <typeparam name="T">The type of parameter passed to the execute and can-execute methods.</typeparam>
    /// <param name="executeMethod">The method to execute when the command is invoked.</param>
    /// <param name="canExecuteMethod">An optional method that determines whether the command can execute with the given parameter.</param>
    /// <param name="useCommandManager">An optional flag indicating whether to use the command manager.</param>
    /// <returns>The registered synchronous command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="executeMethod"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the ViewModel has been disposed.</exception>
    public ICommand RegisterCommand<T>(Action<T> executeMethod, Func<T, bool>? canExecuteMethod = null, bool? useCommandManager = null)
    {
        ArgumentNullException.ThrowIfNull(executeMethod);
        CheckDisposed();
        string methodName = executeMethod.Method.Name;
        return new DelegateCommand<T>(x =>
        {
            try
            {
                executeMethod(x);
            }
            catch (Exception ex)
            {
                OnError(ex, methodName);
            }
        }, canExecuteMethod, useCommandManager);
    }

    #endregion
}
