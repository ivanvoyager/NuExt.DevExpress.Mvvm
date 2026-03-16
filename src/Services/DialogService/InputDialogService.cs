using System.ComponentModel;
using System.Windows;

namespace DevExpress.Mvvm.UI;

/// <summary>
/// Provides asynchronous methods to show and manage modal dialogs.
/// Extends ViewServiceBase and implements IAsyncDialogService interface.
/// </summary>
public class InputDialogService : ViewServiceBase, IAsyncDialogService
{
    #region Dependency Properties

    /// <summary>
    /// Identifies the <see cref="ValidatesOnDataErrors"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidatesOnDataErrorsProperty = DependencyProperty.Register(
        nameof(ValidatesOnDataErrors), typeof(bool), typeof(InputDialogService), new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="ValidatesOnNotifyDataErrors"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidatesOnNotifyDataErrorsProperty = DependencyProperty.Register(
        nameof(ValidatesOnNotifyDataErrors), typeof(bool), typeof(InputDialogService), new PropertyMetadata(false));

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the service should check for validation errors
    /// when closing the dialog. If true, the service will prevent the dialog from closing if there are validation errors.
    /// This applies only if the ViewModel implements the <see cref="IDataErrorInfo"/> interface.
    /// </summary>
    public bool ValidatesOnDataErrors
    {
        get => (bool)GetValue(ValidatesOnDataErrorsProperty);
        set => SetValue(ValidatesOnDataErrorsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog should check for validation errors
    /// when closing. If true, the dialog will prevent closing if there are validation errors.
    /// This applies only if the ViewModel implements the <see cref="INotifyDataErrorInfo"/> interface.
    /// </summary>
    public bool ValidatesOnNotifyDataErrors
    {
        get => (bool)GetValue(ValidatesOnNotifyDataErrorsProperty);
        set => SetValue(ValidatesOnNotifyDataErrorsProperty, value);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Attempts to retrieve the Window associated with the current object.
    /// </summary>
    /// <returns>
    /// The Window instance associated with the current object if available; otherwise, null.
    /// </returns>
    protected Window? GetWindow()
    {
        return AssociatedObject != null ? AssociatedObject as Window ?? Window.GetWindow(AssociatedObject) : null;
    }

    /// <summary>
    /// Displays a dialog asynchronously with the specified parameters.
    /// </summary>
    /// <param name="dialogCommands">A collection of UICommand objects representing the buttons available in the dialog.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="documentType">The type of the view to display within the dialog.</param>
    /// <param name="viewModel">The ViewModel associated with the view.</param>
    /// <param name="parameter">Additional parameters for initializing the view.</param>
    /// <param name="parentViewModel">The parent ViewModel for context.</param>
    /// <param name="cancellationToken">A token to cancel the dialog operation if needed.</param>
    /// <returns>A ValueTask&lt;UICommand?&gt; representing the command selected by the user.</returns>
    public ValueTask<UICommand?> ShowDialogAsync(IEnumerable<UICommand> dialogCommands, string? title, string? documentType, object? viewModel, object? parameter, object? parentViewModel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var view = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel);

        var dialog = new InputDialog
        {
            CommandsSource = dialogCommands,
            Content = view,
            Owner = GetWindow(),
            ValidatesOnDataErrors = ValidatesOnDataErrors,
            ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        if (string.IsNullOrEmpty(title))
        {
            DocumentUIServiceBase.SetTitleBinding(dialog.Content, Window.TitleProperty, dialog, true);
        }
        else
        {
            dialog.Title = title;
        }

        return new ValueTask<UICommand?>(dialog.ShowDialog(cancellationToken));
    }

    #endregion
}
