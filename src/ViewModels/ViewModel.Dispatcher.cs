using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace DevExpress.Mvvm;

partial class ViewModel
{
    #region Properties

    /// <summary>
    /// Gets the dispatcher associated with the UI thread.
    /// </summary>
    public Dispatcher Dispatcher { get; } = Dispatcher.CurrentDispatcher;

    /// <summary>
    /// Gets the thread on which the current instance was created.
    /// </summary>
    public Thread Thread => Dispatcher.Thread;

    #endregion

    #region Methods

    /// <summary>
    /// Checks if the current thread is the same as the thread on which this instance was created.
    /// </summary>
    /// <returns>True if the current thread is the same as the creation thread; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CheckAccess()
    {
        return Dispatcher.CheckAccess();
    }


    /// <summary>
    /// Checks if the current thread is the same as the thread on which this instance was created and throws an <see cref="InvalidOperationException"/> if not.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the current thread is not the same as the thread on which this instance was created.</exception>
    void IDispatcherObject.VerifyAccess()
    {
        if (!CheckAccess())
        {
            ThrowVerifyAccess();
        }
    }

    private void ThrowVerifyAccess()
    {
        var message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}): method was called from an invalid thread.";
        Trace.WriteLine(message);
        Debug.Fail(message);
        Throw.InvalidOperationException(message);
    }

    #endregion
}
