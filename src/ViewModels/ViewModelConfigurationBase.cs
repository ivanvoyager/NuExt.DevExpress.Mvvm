namespace DevExpress.Mvvm;

public abstract class ViewModelConfigurationBase
{
    public bool? IsInDebugModeOverride { get; set; }

    public bool ThrowFinalizerException { get; set; } = true;

    public bool ThrowParentViewModelIsNullException { get; set; } = true;

    public  bool ThrowAlreadyDisposedException { get; set; } = true;

    public bool ThrowAlreadyInitializedException { get; set; } = true;
}
