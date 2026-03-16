using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace DevExpress.Mvvm;

partial class WindowViewModel
{
    #region Commands

    /// <summary>
    /// Gets or sets the command that is executed when the content is rendered.
    /// </summary>
    public ICommand? ContentRenderedCommand
    {
        get => GetProperty(() => ContentRenderedCommand);
        private set { SetProperty(() => ContentRenderedCommand, value); }
    }

    /// <summary>
    /// Gets or sets the command that is executed to close the window.
    /// </summary>
    public ICommand? CloseCommand
    {
        get => GetProperty(() => CloseCommand);
        private set { SetProperty(() => CloseCommand, value); }
    }

    /// <summary>
    /// Gets or sets the command that is executed during the closing event of the window.
    /// </summary>
    public ICommand? ClosingCommand
    {
        get => GetProperty(() => ClosingCommand);
        private set { SetProperty(() => ClosingCommand, value); }
    }


    /// <summary>
    /// Gets or sets the command that is executed after the window placement has been restored.
    /// </summary>
    public ICommand? PlacementRestoredCommand
    {
        get => GetProperty(() => PlacementRestoredCommand);
        private set { SetProperty(() => PlacementRestoredCommand, value); }
    }

    /// <summary>
    /// Gets or sets the command that is executed after the window placement has been saved.
    /// </summary>
    public ICommand? PlacementSavedCommand
    {
        get => GetProperty(() => PlacementSavedCommand);
        private set { SetProperty(() => PlacementSavedCommand, value); }
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// Asynchronously executes operations when the content of the window is rendered.
    /// </summary>
    private async Task ContentRenderedAsync()
    {
        try
        {
            await OnContentRenderedAsync(CancellationTokenSource.Token);
        }
        catch (OperationCanceledException ex)
        {
            if (CancellationTokenSource.IsCancellationRequested == false)
            {
                Debug.Fail(ex.Message);
                throw;
            }
        }
        catch (Exception ex)
        {
            //TODO logging
            if (CancellationTokenSource.IsCancellationRequested == false)
            {
                OnError(ex);
            }
        }

        if (CancellationTokenSource.IsCancellationRequested) return;

        var openWindowsService = OpenWindowsService;
        if (openWindowsService != null)
        {
            Lifetime.AddBracket(() => openWindowsService.Register(this), () => openWindowsService.Unregister(this));
        }
    }

    /// <summary>
    /// Determines whether the window can be closed. Override this method to provide custom close logic.
    /// </summary>
    /// <returns>True if the window can be closed; otherwise, false.</returns>
    protected virtual bool CanClose()
    {
        return true;
    }

    /// <summary>
    /// Closes the window by calling the current window service.
    /// </summary>
    public virtual void Close()
    {
        CurrentWindowService?.Close();
    }

    /// <summary>
    /// Handles the closing event of the window, managing cancellation and disposal states.
    /// </summary>
    /// <param name="arg">The arguments for the cancel event.</param>
    private void Closing(CancelEventArgs arg)
    {
        //https://weblog.west-wind.com/posts/2019/Sep/02/WPF-Window-Closing-Errors
        if (CancellationTokenSource.IsCancellationRequested || IsDisposed)
        {
            Debug.Assert(arg.Cancel == false);
            arg.Cancel = IsDisposing;//do not close while disposing
            return;
        }
        if (CanClose() == false)
        {
            arg.Cancel = false;
            return;
        }
        CancellationTokenSource.Cancel();
        arg.Cancel = true;
        Debug.Assert(DispatcherService != null, $"{nameof(DispatcherService)} is null");
        DispatcherService?.BeginInvoke(async () => { await CloseForcedAsync(false); });
    }

    /// <summary>
    /// This method is called after the window placement has been restored.
    /// Override this method to add custom logic that should run after placement is restored.
    /// </summary>
    protected virtual void OnPlacementRestored()
    {

    }

    /// <summary>
    /// This method is called after the window placement has been saved.
    /// Override this method to add custom logic that should run after placement is saved.
    /// </summary>
    protected virtual void OnPlacementSaved()
    {

    }

    #endregion

    #region Methods

    protected override void CreateCommands()
    {
        base.CreateCommands();

        ContentRenderedCommand = RegisterAsyncCommand(ContentRenderedAsync);
        CloseCommand = RegisterCommand(Close, CanClose);
        ClosingCommand = RegisterCommand<CancelEventArgs>(Closing);
        PlacementRestoredCommand = RegisterCommand(OnPlacementRestored);
        PlacementSavedCommand = RegisterCommand(OnPlacementSaved);
    }

    /// <summary>
    /// Called when the content of the window is rendered.
    /// Allows for additional initialization or setup that depends on the window's content being ready.
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual ValueTask OnContentRenderedAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(CurrentWindowService != null, $"{nameof(CurrentWindowService)} is null");
        Debug.Assert(DispatcherService != null, $"{nameof(DispatcherService)} is null");
        Debug.Assert(OpenWindowsService != null, $"{nameof(OpenWindowsService)} is null");
        Debug.Assert(WindowPlacementService != null, $"{nameof(WindowPlacementService)} is null");
        return default;
    }

    #endregion
}
