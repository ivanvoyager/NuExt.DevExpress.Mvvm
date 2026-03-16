using DevExpress.Mvvm.Native;

namespace DevExpress.Mvvm;

/// <summary>
/// Provides extension methods for the <see cref="IAsyncDialogService"/>.
/// </summary>
public static class AsyncDialogServiceExtensions
{
    /// <summary>
    /// Shows a dialog with the specified commands, title, and view model.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogCommands">The commands to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="viewModel">The view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UI command representing the user's action, or null if the dialog was dismissed.</returns>
    public static ValueTask<UICommand?> ShowDialogAsync(this IAsyncDialogService service, IEnumerable<UICommand> dialogCommands, string title, object viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.ShowDialogAsync(dialogCommands, title, null, viewModel, null, null, cancellationToken);
    }

    /// <summary>
    /// Shows a dialog with the specified buttons, title, and view model, and returns the message result.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogButtons">The buttons to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="viewModel">The view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the message result based on the user's action.</returns>
    public static async ValueTask<MessageResult> ShowDialogAsync(this IAsyncDialogService service, MessageButton dialogButtons, string title, object viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        var res = await service.ShowDialogAsync(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, null, viewModel, null, null, cancellationToken);
        return GetMessageResult(res);
    }

    /// <summary>
    /// Shows a dialog with the specified commands, title, document type, and view model.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogCommands">The commands to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the document associated with the dialog.</param>
    /// <param name="viewModel">The view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UI command representing the user's action, or null if the dialog was dismissed.</returns>
    public static ValueTask<UICommand?> ShowDialogAsync(this IAsyncDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.ShowDialogAsync(dialogCommands, title, documentType, viewModel, null, null, cancellationToken);
    }

    /// <summary>
    /// Shows a dialog with the specified buttons, title, document type, and view model, and returns the message result.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogButtons">The buttons to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the document associated with the dialog.</param>
    /// <param name="viewModel">The view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the message result based on the user's action.</returns>
    public static async ValueTask<MessageResult> ShowDialogAsync(this IAsyncDialogService service, MessageButton dialogButtons, string title, string documentType, object viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        var res = await service.ShowDialogAsync(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, documentType, viewModel, null, null, cancellationToken);
        return GetMessageResult(res);
    }

    /// <summary>
    /// Shows a dialog with the specified commands, title, document type, parameter, and parent view model.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogCommands">The commands to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the document associated with the dialog.</param>
    /// <param name="parameter">An additional parameter for the dialog.</param>
    /// <param name="parentViewModel">The parent view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a UI command representing the user's action, or null if the dialog was dismissed.</returns>
    public static ValueTask<UICommand?> ShowDialogAsync(this IAsyncDialogService service, IEnumerable<UICommand> dialogCommands, string title, string documentType, object parameter, object parentViewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.ShowDialogAsync(dialogCommands, title, documentType, null, parameter, parentViewModel, cancellationToken);
    }

    /// <summary>
    /// Shows a dialog with the specified buttons, title, document type, parameter, and parent view model, and returns the message result.
    /// </summary>
    /// <param name="service">The dialog service.</param>
    /// <param name="dialogButtons">The buttons to be displayed in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the document associated with the dialog.</param>
    /// <param name="parameter">An additional parameter for the dialog.</param>
    /// <param name="parentViewModel">The parent view model associated with the dialog.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the message result based on the user's action.</returns>
    public static async ValueTask<MessageResult> ShowDialogAsync(this IAsyncDialogService service, MessageButton dialogButtons, string title, string documentType, object parameter, object parentViewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(service);

        var res = await service.ShowDialogAsync(UICommand.GenerateFromMessageButton(dialogButtons, GetLocalizer(service)), title, documentType, null, parameter, parentViewModel, cancellationToken);
        return GetMessageResult(res);
    }

    internal static IMessageButtonLocalizer GetLocalizer(IAsyncDialogService service)
    {
        return service as IMessageButtonLocalizer ?? (service as IMessageBoxButtonLocalizer).With(x => x.ToMessageButtonLocalizer()) ?? new DefaultMessageButtonLocalizer();
    }

    private static MessageResult GetMessageResult(UICommand? result)
    {
        if (result == null)
            return MessageResult.None;
        return (MessageResult)result.Tag;
    }
}
