using System.Diagnostics;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents a ViewModel for a window, providing properties and methods for managing the window's state,
/// services for handling various window-related operations, and commands for interacting with the UI.
/// </summary>
public abstract partial class WindowViewModel: ControlViewModel, IWindowViewModel
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CancellationTokenSource"/> used for managing cancellation of asynchronous operations.
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    public object? Title
    {
        get => GetProperty(() => Title);
        set { SetProperty(() => Title, value); }
    }

    #endregion

    #region Services

    /// <summary>
    /// Gets the service responsible for managing the current window.
    /// </summary>
    protected ICurrentWindowService? CurrentWindowService => GetService<ICurrentWindowService>();

    /// <summary>
    /// Gets the service responsible for managing open windows.
    /// </summary>
    protected IOpenWindowsService? OpenWindowsService => GetService<IOpenWindowsService>();

    /// <summary>
    /// Gets the service responsible for managing window placement.
    /// </summary>
    protected IWindowPlacementService? WindowPlacementService => GetService<IWindowPlacementService>();

    #endregion

    #region Methods

    /// <summary>
    /// Closes the window asynchronously, optionally forcing closure.
    /// </summary>
    /// <param name="forceClose">If true, forces the window to close.</param>
    /// <returns>A task representing the asynchronous close operation.</returns>
    public async ValueTask CloseForcedAsync(bool forceClose = true)
    {
        if (IsDisposed || IsDisposing)
        {
            return;
        }

        Debug.Assert(CancellationTokenSource.IsCancellationRequested || forceClose);
        if (forceClose)
        {
#if NET8_0_OR_GREATER
            await CancellationTokenSource.CancelAsync();
#else
            CancellationTokenSource.Cancel();
#endif
            WindowPlacementService?.SavePlacement();//TODO check
        }

        try
        {
            await DisposeAsync();

            Debug.Assert(CheckAccess());
            Debug.Assert(CurrentWindowService != null, $"{nameof(CurrentWindowService)} is null");
            CurrentWindowService?.Close();
            CancellationTokenSource.Dispose();
        }
        catch (Exception ex)
        {
            //TODO logging
            OnError(ex);
        }
    }

    #endregion
}
