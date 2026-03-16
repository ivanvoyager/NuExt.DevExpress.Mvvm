namespace DevExpress.Mvvm;

/// <summary>
/// Provides extension methods for the <see cref="IAsyncDocumentManagerService"/>.
/// </summary>
public static class AsyncDocumentManagerServiceExtensions
{
    /// <summary>
    /// Creates a new asynchronous document with the specified view model.
    /// </summary>
    /// <param name="service">The document manager service.</param>
    /// <param name="viewModel">The view model associated with the document.</param>
    /// <returns>The created asynchronous document.</returns>
    public static IAsyncDocument CreateDocument(this IAsyncDocumentManagerService service, object viewModel)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.CreateDocument(null, viewModel, null, null);
    }

    /// <summary>
    /// Creates a new asynchronous document with the specified document type and view model.
    /// </summary>
    /// <param name="service">The document manager service.</param>
    /// <param name="documentType">The type of the document.</param>
    /// <param name="viewModel">The view model associated with the document.</param>
    /// <returns>The created asynchronous document.</returns>
    public static IAsyncDocument CreateDocument(this IAsyncDocumentManagerService service, string documentType, object viewModel)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.CreateDocument(documentType, viewModel, null, null);
    }

    /// <summary>
    /// Creates a new asynchronous document with the specified document type, parameter, and parent view model.
    /// </summary>
    /// <param name="service">The document manager service.</param>
    /// <param name="documentType">The type of the document.</param>
    /// <param name="parameter">Additional parameters for creating the document.</param>
    /// <param name="parentViewModel">The parent view model of the document.</param>
    /// <returns>The created asynchronous document.</returns>
    public static IAsyncDocument CreateDocument(this IAsyncDocumentManagerService service, string documentType, object parameter, object parentViewModel)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.CreateDocument(documentType, null, parameter, parentViewModel);
    }

    /// <summary>
    /// Finds an asynchronous document by its ID.
    /// </summary>
    /// <param name="service">The document manager service.</param>
    /// <param name="id">The ID of the document to find.</param>
    /// <returns>The found asynchronous document, or <see langword="null"/> if no document is found.</returns>
    public static IAsyncDocument? FindDocumentById(this IAsyncDocumentManagerService service, object id)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.Documents.FirstOrDefault(x => object.Equals(x.Id, id));
    }

    /// <summary>
    /// Finds an asynchronous document by its ID, or creates a new document if it does not exist.
    /// </summary>
    /// <param name="service">The document manager service.</param>
    /// <param name="id">The ID of the document to find or create.</param>
    /// <param name="createDocumentCallback">The callback function to create a new document if none is found.</param>
    /// <returns>The found or newly created asynchronous document.</returns>
    public static async ValueTask<IAsyncDocument> FindDocumentByIdOrCreateAsync(this IAsyncDocumentManagerService service, object id, Func<IAsyncDocumentManagerService, ValueTask<IAsyncDocument>> createDocumentCallback)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(createDocumentCallback);

        IAsyncDocument? document = service.FindDocumentById(id);
        if (document == null)
        {
            document = await createDocumentCallback(service);
            document.Id = id;
        }
        return document;
    }
}
