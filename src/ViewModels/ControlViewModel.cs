using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents an abstract base class for control-specific ViewModels, extending the functionality of the <see cref="ViewModel"/> class.
/// This class is designed to support various services and handle common tasks such as disposal, error handling, initialization,
/// and managing asynchronous commands.
/// </summary>
/// <remarks>
/// Inherits from <see cref="ViewModel"/> and implements <see cref="IControlViewModel"/>.
/// Provides properties and methods specific to control ViewModels, including service accessors, lifecycle management,
/// and command handling.
/// </remarks>
public abstract partial class ControlViewModel: ViewModel, IControlViewModel
{
    #region Properties

    /// <summary>
    /// Gets the contract for managing the asynchronous lifecycle of resources and actions.
    /// </summary>
    protected IAsyncLifetime Lifetime { get; } = new AsyncLifetime() { ContinueOnCapturedContext = true };

    #endregion

    #region Services

    /// <summary>
    /// Gets the dispatcher service for managing thread-aware operations.
    /// </summary>
    protected IDispatcherService? DispatcherService => GetService<IDispatcherService>();

    #endregion

    #region Methods

    /// <summary>
    /// Asynchronously disposes resources used by this ViewModel, ensuring proper cleanup.
    /// </summary>
    protected override async ValueTask OnDisposeAsync()
    {
        ValidateDisposingState();

        Debug.Assert(Lifetime.IsTerminated == false);
        await Lifetime.DisposeAsync();

        ValidateFinalState();
    }

    /// <summary>
    /// Handles errors that occur within the ViewModel, providing a mechanism to display error messages.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    protected virtual void OnError(Exception ex, [CallerMemberName] string? callerName = null)
    {
        Trace.WriteLine($"An error has occurred in {callerName}:{Environment.NewLine}{ex.Message}");
    }

    /// <summary>
    /// Initializes the ViewModel at runtime, setting up commands and other necessary components.
    /// </summary>
    protected override void OnInitializeInRuntime()
    {
        Debug.Assert(IsInitialized == false);
        base.OnInitializeInRuntime();

        CreateCommands();
        Lifetime.Add(NullifyCommands);//last operation after CommandManager.WaitAll
        Lifetime.AddAsync(() => CommandManager.WaitAll());
    }

    #endregion
}
