namespace DevExpress.Mvvm;

public class ViewModelConfiguration: ViewModelConfigurationBase, IViewModelConfiguration
{
    private static readonly IViewModelConfiguration s_default = new DefaultViewModelConfiguration();

    private static IViewModelConfiguration? s_custom;

    public ViewModelConfiguration()
    {

    }

    public ViewModelConfiguration(IViewModelConfiguration viewModelConfiguration)
    {
        _ = viewModelConfiguration ?? throw new ArgumentNullException(nameof(viewModelConfiguration));

        IsInDebugModeOverride = viewModelConfiguration.IsInDebugModeOverride;
        ThrowFinalizerException = viewModelConfiguration.ThrowFinalizerException;
        ThrowParentViewModelIsNullException = viewModelConfiguration.ThrowParentViewModelIsNullException;
        ThrowAlreadyDisposedException = viewModelConfiguration.ThrowAlreadyDisposedException;
        ThrowAlreadyInitializedException = viewModelConfiguration.ThrowAlreadyInitializedException;
    }

    public static IViewModelConfiguration Default
    {
        get => s_custom ?? s_default;
        set => s_custom = value;
    }
}
