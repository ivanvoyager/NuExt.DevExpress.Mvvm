using System.Diagnostics;

namespace DevExpress.Mvvm;

partial class ControlViewModel
{
    [Conditional("DEBUG")]
    private void ValidateDisposingState()
    {
        var typeName = GetType().FullName;
        var displayName = DisplayName ?? "Unnamed";
        var hashCode = GetHashCode();

        Debug.Assert(CheckAccess());
        Debug.Assert(CommandManager.Count == 0 ||
                     CommandManager.All(command => command.IsExecuting == false || command.IsCancellationRequested),
            $"{typeName} ({displayName}) ({hashCode}) has unexpected state of async commands.");
    }

    [Conditional("DEBUG")]
    private void ValidateFinalState()
    {
        var typeName = GetType().FullName;
        var displayName = DisplayName ?? "Unnamed";
        var hashCode = GetHashCode();

        Debug.Assert(HasActiveCommands == false, $"{typeName} ({displayName}) ({hashCode}) has active commands.");
        Debug.Assert(CommandManager.Count == 0, $"{typeName} ({displayName}) ({hashCode}) has {CommandManager.Count} registered commands.");
        Debug.Assert(_asyncCommands.IsEmpty);

        var commands = GetAllCommands();
        Debug.Assert(commands.All(c => c.Value is null));

        Debug.Assert(CheckAccess());
    }
}
