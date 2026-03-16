using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DevExpress.Mvvm;

partial class ViewModel
{
    private static readonly FieldInfo? s_propertyChangedFieldInfo =
        typeof(BindableBase).GetField(nameof(PropertyChanged), BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Gets a value indicating whether there are any subscribers to the PropertyChanged event.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasPropertyChangedSubscribers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetPropertyChangedEventHandlers()?.Length > 0;
    }

    /// <summary>
    /// Gets the current subscribers of the PropertyChanged event.
    /// </summary>
    /// <returns>
    /// An array of <see cref="PropertyChangedEventHandler"/> delegates, or <see langword="null"/> if there are no subscribers.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PropertyChangedEventHandler[]? GetPropertyChangedEventHandlers()
    {
        // eventDelegate will be null if no listeners are attached to the event
        if (s_propertyChangedFieldInfo?.GetValue(this) is not PropertyChangedEventHandler eventDelegate)
        {
            return null;
        }
        var subscribers = Array.ConvertAll(eventDelegate.GetInvocationList(), del => (PropertyChangedEventHandler)del);
        return subscribers;
    }
}
