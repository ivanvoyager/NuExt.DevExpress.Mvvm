using System.ComponentModel;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents a ViewModel in the MVVM (Model-View-ViewModel) pattern, providing a base set of functionalities 
/// for property change notification, parent view model support, service provision, parameter handling, 
/// and asynchronous disposal and dispatching.
/// </summary>
public interface IViewModel: INotifyPropertyChanged, ISupportParentViewModel, ISupportServices, ISupportParameter, IAsyncDisposable, IDispatcherObject
{
    /// <summary>
    /// Gets or sets the display name of the ViewModel, primarily used for debugging purposes.
    /// </summary>
    string? DisplayName { get; set; }
    /// <summary>
    /// Gets a value indicating whether the ViewModel is currently disposing.
    /// </summary>
    bool IsDisposing { get; }
    /// <summary>
    /// Gets a value indicating whether the ViewModel has been disposed.
    /// </summary>
    bool IsDisposed { get; }
    /// <summary>
    /// Gets a value indicating whether the ViewModel has been initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Occurs when the ViewModel starts the disposing process asynchronously.
    /// </summary>
    event AsyncEventHandler? Disposing;

    /// <summary>
    /// Asynchronously initializes the ViewModel.
    /// This method performs various checks and throws exceptions if certain conditions are met,
    /// such as if the parent ViewModel is null or if the ViewModel is already initialized.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the initialization process.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    ValueTask InitializeAsync(CancellationToken cancellationToken = default);
}

public static class ViewModelExtensions
{
    public static IViewModel SetParentViewModel(this IViewModel viewModel, object? parentViewModel)
    {
        viewModel.ParentViewModel = parentViewModel;
        return viewModel;
    }

    public static IViewModel SetParameter(this IViewModel viewModel, object? parameter)
    {
        viewModel.Parameter = parameter;
        return viewModel;
    }
}
