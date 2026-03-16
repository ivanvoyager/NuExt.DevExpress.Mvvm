using System.ComponentModel;

namespace DevExpress.Mvvm;

/// <summary>
/// Defines an interface for managing asynchronous documents.
/// </summary>
public interface IAsyncDocumentManagerService: IAsyncDisposable
{
    /// <summary>
    /// Creates a new asynchronous document.
    /// This method is not intended to be used directly from your code.
    /// </summary>
    /// <param name="documentType">The type of the document.</param>
    /// <param name="viewModel">The view model associated with the document.</param>
    /// <param name="parameter">Additional parameters for creating the document.</param>
    /// <param name="parentViewModel">The parent view model of the document.</param>
    /// <returns>The created asynchronous document.</returns>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    IAsyncDocument CreateDocument(string? documentType, object? viewModel, object? parameter, object? parentViewModel);

    /// <summary>
    /// Gets or sets the currently active document.
    /// </summary>
    IAsyncDocument? ActiveDocument { get; set; }

    /// <summary>
    /// Occurs when the active document has changed.
    /// </summary>
    event ActiveDocumentChangedEventHandler? ActiveDocumentChanged;

    /// <summary>
    /// Gets a collection of all open asynchronous documents.
    /// </summary>
    IEnumerable<IAsyncDocument> Documents { get; }
}
