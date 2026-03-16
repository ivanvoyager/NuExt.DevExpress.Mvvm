using System.ComponentModel;

namespace DevExpress.Mvvm;

/// <summary>
/// Provides a base class for view models that represent document content. 
/// This class extends <see cref="ControlViewModel"/> and implements the <see cref="IDocumentContent"/> interface.
/// </summary>
public abstract class DocumentContentViewModelBase : ControlViewModel, IDocumentContent
{
    #region IDocumentContent

    /// <summary>
    /// Gets or sets the owner of the document.
    /// </summary>
    public IDocumentOwner DocumentOwner
    {
        get { return GetProperty(() => DocumentOwner); }
        set { SetProperty(() => DocumentOwner, value); }
    }

    /// <summary>
    /// Gets or sets the title of the document.
    /// </summary>
    public object? Title
    {
        get { return GetProperty(() => Title); }
        set { SetProperty(() => Title, value); }
    }

    /// <summary>
    /// Called when the document is about to be closed. Allows for custom logic to be executed before the document is closed.
    /// </summary>
    /// <param name="e">Provides data for a cancelable event.</param>
    public virtual void OnClose(CancelEventArgs e)
    {
    }

    /// <summary>
    /// Called when the document is destroyed. Allows for cleanup logic to be executed.
    /// </summary>
    public virtual void OnDestroy()
    {
    }

    #endregion
}
