namespace DevExpress.Mvvm;

/// <summary>
/// Represents the ViewModel for a window control, inheriting from IControlViewModel.
/// Contains properties and methods specific to window functionality such as the title and forced closing of the window.
/// </summary>
public interface IWindowViewModel: IControlViewModel
{
    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    object? Title { get; set; }

    /// <summary>
    /// Closes the window asynchronously, with an option to force closure.
    /// </summary>
    /// <param name="forceClose">A boolean value indicating whether to force the window to close. The default is true.</param>
    /// <returns>A ValueTask that represents the asynchronous operation.</returns>
    ValueTask CloseForcedAsync(bool forceClose = true);
}
