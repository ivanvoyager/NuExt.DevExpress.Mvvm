using System.ComponentModel;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents a bindable object that notifies when a property value changes and 
/// provides functionality to initialize and set properties dynamically.
/// </summary>
public interface IBindable: INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether the object has been initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Retrieves the type of the specified property by name.
    /// </summary>
    /// <param name="propertyName">The name of the property whose type is to be retrieved.</param>
    /// <returns>The <see cref="Type"/> of the specified property, or null if the property does not exist.</returns>
    Type? GetPropertyType(string propertyName);
    /// <summary>
    /// Initializes the object, setting up any necessary state or properties.
    /// </summary>
    void Initialize();
    /// <summary>
    /// Sets the value of the specified property by name.
    /// </summary>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The new value to assign to the property.</param>
    /// <returns>true if the property was set successfully; otherwise, false.</returns>
    bool SetProperty(string propertyName, object? value);
}
