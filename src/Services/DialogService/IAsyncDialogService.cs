using System.ComponentModel;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents an asynchronous dialog service that can show dialogs with specified commands and parameters.
/// </summary>
public interface IAsyncDialogService
{
    /// <summary>
    /// Shows a dialog with the specified commands, title, document type, view model, and additional parameters.
    /// </summary>
    /// <param name="dialogCommands">The commands to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the document associated with the dialog.</param>
    /// <param name="viewModel">The view model associated with the dialog.</param>
    /// <param name="parameter">Additional parameters for the dialog.</param>
    /// <param name="parentViewModel">The parent view model of the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UI command representing the user's action, or null if the dialog was dismissed.</returns>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    ValueTask<UICommand?> ShowDialogAsync(IEnumerable<UICommand> dialogCommands, string? title, string? documentType, object? viewModel, object? parameter, object? parentViewModel, CancellationToken cancellationToken = default);
}
