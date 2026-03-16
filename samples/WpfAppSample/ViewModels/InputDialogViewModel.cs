using DevExpress.Mvvm;

namespace MovieWpfApp.ViewModels;

internal class InputDialogViewModel : ControlViewModel
{
    #region Properties

    public string? InputMessage
    {
        get => GetProperty(() => InputMessage);
        set { SetProperty(() => InputMessage, value); }
    }

    public string? InputText
    {
        get => GetProperty(() => InputText);
        set { SetProperty(() => InputText, value); }
    }

    #endregion

    #region Methods

    protected override ValueTask OnInitializeAsync(CancellationToken cancellationToken)
    {
        if (Parameter is string text)
        {
            InputText = text;
        }
        return default;
    }

    #endregion
}
