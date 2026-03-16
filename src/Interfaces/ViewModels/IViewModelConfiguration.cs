namespace DevExpress.Mvvm;

/// <summary>
/// Represents configuration settings for a ViewModel, including debugging options and exception handling behaviors.
/// </summary>
public interface IViewModelConfiguration
{
    /// <summary>
    /// Gets a value indicating whether debug mode is overridden.
    /// </summary>
    bool? IsInDebugModeOverride { get; }

    /// <summary>
    /// Gets a value indicating whether an exception should be thrown during finalization.
    /// </summary>
    bool ThrowFinalizerException { get; }

    /// <summary>
    /// Gets a value indicating whether an exception should be thrown if the parent ViewModel is null.
    /// </summary>
    bool ThrowParentViewModelIsNullException { get; }

    /// <summary>
    /// Gets a value indicating whether an exception should be thrown when an already disposed ViewModel is accessed.
    /// </summary>
    bool ThrowAlreadyDisposedException { get; }

    /// <summary>
    /// Gets a value indicating whether an exception should be thrown when an already initialized ViewModel is re-initialized.
    /// </summary>
    bool ThrowAlreadyInitializedException { get; }

}
