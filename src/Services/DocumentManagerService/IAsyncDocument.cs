namespace DevExpress.Mvvm;

/// <summary>
/// Defines an interface for an asynchronous document that extends the <see cref="IDocument"/> interface.
/// </summary>
public interface IAsyncDocument: IDocument
{
    /// <summary>
    /// Closes the document asynchronously.
    /// </summary>
    /// <param name="force">If set to <see langword="true"/>, forces the document to close. Default is <see langword="true"/>.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask CloseAsync(bool force = true);
}
