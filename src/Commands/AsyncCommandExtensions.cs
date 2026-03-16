using System.ComponentModel;
using System.Diagnostics;

namespace DevExpress.Mvvm;

/// <summary>
/// Provides extension methods for handling asynchronous commands.
/// </summary>
public static class AsyncCommandExtensions
{
    /// <summary>
    /// Cancels the execution of the specified asynchronous command.
    /// </summary>
    /// <param name="command">The asynchronous command to cancel.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="command"/> is null.</exception>
    public static void Cancel(this IAsyncCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        try
        {
            command.CancelCommand.Execute(null);
        }
        catch (Exception ex)
        {
            Debug.Assert(ex is ObjectDisposedException, $"Unexpected exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Waits asynchronously until the specified command has finished executing.
    /// </summary>
    /// <param name="command">The asynchronous command to wait for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="command"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the wait is canceled.</exception>
    public static async Task WaitAsync(this IAsyncCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.IsExecuting == false || command is not INotifyPropertyChanged npc)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IAsyncCommand.IsExecuting) && command.IsExecuting == false)
            {
                tcs.TrySetResult(true);
            }
        }

        npc.PropertyChanged += OnPropertyChanged;
        try
        {
            if (command.IsExecuting == false)
            {
                tcs.TrySetResult(true);
            }

            using (cancellationToken.CanBeCanceled
                       ? cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false)
                       : null as IDisposable)
            {
                await tcs.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            npc.PropertyChanged -= OnPropertyChanged;
        }
    }
}
