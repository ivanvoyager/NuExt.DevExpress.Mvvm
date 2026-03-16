using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents an abstract base class for ViewModels, providing common functionality and properties.
/// This class includes support for design mode detection, thread-awareness, and property change notifications.
/// </summary>
/// <remarks>
/// Inherits from <see cref="ViewModelBase"/> and implements <see cref="IViewModel"/>.
/// Provides properties to manage the state of the ViewModel such as <see cref="IsDisposing"/>, <see cref="IsDisposed"/>, 
/// <see cref="IsInitialized"/>, and others. It also handles debugging and design-time scenarios.
/// Note that this class has a finalizer, but it is generally undesirable for the finalizer to be called. 
/// Ensure that <see cref="DisposeAsync"/> is properly invoked to suppress finalization.
/// </remarks>
[DebuggerDisplay("{DisplayName}")]
[DebuggerStepThrough]
public abstract partial class ViewModel : ViewModelBase, IViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModel"/> class.
    /// </summary>
    protected ViewModel()
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ViewModel"/> class.
    /// This finalizer is called by the garbage collector before the object is reclaimed.
    /// It checks a condition to determine if an exception should be thrown during finalization
    /// using the overridable method <see cref="IViewModelConfiguration.ThrowFinalizerException"/> property.
    /// If the property returns true, <see cref="ThrowFinalizerException"/> is invoked to generate an exception.
    /// </summary>
    ~ViewModel()
    {
        if (IsInDesignMode) return;
        if (!ViewModelConfig.ThrowFinalizerException)
        {
            Debug.Fail($"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) was finalized without proper disposal.");
            return;
        }
        ThrowFinalizerException();
    }

    #region Properties

    /// <summary>
    /// Gets or sets the display name of the ViewModel, primarily used for debugging purposes.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets a value indicating whether the application is in debug mode.
    /// The value is true if the application is compiled in DEBUG mode,
    /// or if <see cref="IViewModelConfiguration.IsInDebugModeOverride"/> is set to true.
    /// </summary>
    public bool IsInDebugMode
    {
        get
        {
#if DEBUG
            return ViewModelConfig.IsInDebugModeOverride ?? true;
#else
            return ViewModelConfig.IsInDebugModeOverride == true;
#endif
        }
    }

    /// <summary>
    /// Gets a value indicating whether the ViewModel has been disposed.
    /// </summary>
    public bool IsDisposed
    {
        get { return GetProperty(() => IsDisposed); }
        private set { SetProperty(() => IsDisposed, value, () => RaisePropertyChanged(nameof(IsUsable))); }
    }

    /// <summary>
    /// Gets a value indicating whether the ViewModel is currently disposing.
    /// </summary>
    public bool IsDisposing
    {
        get { return GetProperty(() => IsDisposing); }
        private set { SetProperty(() => IsDisposing, value, () => RaisePropertyChanged(nameof(IsUsable))); }
    }

    /// <summary>
    /// Gets a value indicating whether the ViewModel has been initialized.
    /// </summary>
    public bool IsInitialized
    {
        get { return GetProperty(() => IsInitialized); }
        private set { SetProperty(() => IsInitialized, value, () => RaisePropertyChanged(nameof(IsUsable))); }
    }

    /// <summary>
    /// Gets a value indicating whether the object is usable.
    /// </summary>
    /// <remarks>
    /// The object is considered usable if it has been initialized and 
    /// is neither in the process of being disposed nor already disposed.
    /// This property ensures that the object is in a valid state for operations.
    /// </remarks>
    public bool IsUsable => IsInitialized && IsDisposed == false && IsDisposing == false;

    /// <summary>
    /// Gets the configuration settings for the ViewModel.
    /// </summary>
    protected IViewModelConfiguration ViewModelConfig => GetService<IViewModelConfiguration>() ?? ViewModelConfiguration.Default;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the ViewModel starts the disposing process asynchronously.
    /// </summary>
    public event AsyncEventHandler? Disposing;

    #endregion

    #region Methods

    /// <summary>
    /// Checks if the object has been disposed and throws an <see cref="ObjectDisposedException"/> if it has.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void CheckDisposed()
    {
        if (IsDisposed)
        {
            var message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) has been disposed.";
            Trace.WriteLine(message);
            Debug.Fail(message);
            Throw.ObjectDisposedException(this, message);
        }
    }

    /// <summary>
    /// Asynchronously disposes of the resources used by the instance.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        Debug.Assert(!IsDisposed, $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) is already disposed");
        Debug.Assert(!IsDisposing, $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) is already disposing");
        if (ViewModelConfig.ThrowAlreadyDisposedException)
        {
            CheckDisposed();
        }
        if (IsDisposed || IsDisposing)
        {
            return;
        }
        IsDisposing = true;
        try
        {
            await Disposing.InvokeAsync(this, EventArgs.Empty);
            await OnDisposeAsync().ConfigureAwait(false);
            IsDisposed = true;
            Debug.Assert(Disposing is null, $"{GetType().FullName} ({GetHashCode()}): {nameof(Disposing)} is not null");
            //Debug.Assert(HasPropertyChangedSubscribers == false, $"{GetType().FullName} ({GetHashCode()}): {nameof(PropertyChanged)} is not null");
        }
        catch (Exception ex)
        {
            Debug.Assert(false, $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}):{Environment.NewLine}{ex.Message}");
            throw;
        }
        finally
        {
            IsDisposing = false;
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously initializes the ViewModel.
    /// This method performs various checks and throws exceptions if certain conditions are met,
    /// such as if the parent ViewModel is null or if the ViewModel is already initialized.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the initialization process.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="IViewModelConfiguration.ThrowParentViewModelIsNullException"/> is true and the parent ViewModel is null,
    /// or if <see cref="IViewModelConfiguration.ThrowAlreadyInitializedException"/> is true and the ViewModel has already been initialized.
    /// </exception>
    public async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (ViewModelConfig.ThrowParentViewModelIsNullException)
        {
            ThrowIfParentViewModelIsNull();
        }
        if (ViewModelConfig.ThrowAlreadyInitializedException)
        {
            ThrowAlreadyInitializedException();
        }
        if (IsInitialized)
        {
            return;
        }
        Debug.Assert(ViewModelConfig != null, $"{nameof(ViewModelConfig)} is null");
        await OnInitializeAsync(cancellationToken).ConfigureAwait(false);
        IsInitialized = true;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting managed resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    protected abstract ValueTask OnDisposeAsync();

    /// <summary>
    /// When overridden in a derived class, asynchronously performs the initialization logic for the ViewModel.
    /// This method should contain any custom initialization logic required by the derived ViewModel class.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the initialization process.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    protected abstract ValueTask OnInitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the ViewModel has already been initialized.
    /// This method is called when re-initialization is not allowed and provides detailed information about the ViewModel instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the ViewModel has already been initialized.</exception>
    private void ThrowAlreadyInitializedException()
    {
        if (IsInitialized)
        {
            string message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) has already been initialized and cannot be reinitialized.";
            Trace.WriteLine(message);
            Debug.Fail(message);
            Throw.InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Method invoked by the finalizer to generate an exception.
    /// It outputs debug messages and throws an exception with information about the type and hash code of the object.
    /// This method can be used for debugging and diagnosing issues related to improper object usage.
    /// </summary>
    private void ThrowFinalizerException()
    {
        string message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}) was finalized without proper disposal.";
        Trace.WriteLine(message);
        Debug.Fail(message);
        Throw.InvalidOperationException(message);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the parent ViewModel is null.
    /// This method is used to ensure that operations dependent on the parent ViewModel
    /// are not performed when the parent is not set.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parent ViewModel is null.</exception>
    protected void ThrowIfParentViewModelIsNull()
    {
        if ((this as ISupportParentViewModel).ParentViewModel is null)
        {
            var message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}): {nameof(ISupportParentViewModel.ParentViewModel)} is null";
            Trace.WriteLine(message);
            Debug.Fail(message);
            Throw.InvalidOperationException(message);
        }
    }

    #endregion
}
