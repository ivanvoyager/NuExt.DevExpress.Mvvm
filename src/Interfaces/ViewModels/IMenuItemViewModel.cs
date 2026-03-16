using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents the ViewModel for a menu item, encapsulating properties such as 
/// the command to execute, command parameters, display header, and any sub-menu items.
/// </summary>
public interface IMenuItemViewModel
{
    /// <summary>
    /// Gets the command that will be executed when the menu item is activated.
    /// </summary>
    ICommand? Command { get; }
    /// <summary>
    /// Gets the parameter to pass to the command when it is executed.
    /// </summary>
    object? CommandParameter { get; }
    /// <summary>
    /// Gets the text to display as the menu item's header.
    /// </summary>
    string Header { get; }
    /// <summary>
    /// Gets the collection of sub-menu items associated with this menu item.
    /// </summary>
    ObservableCollection<IMenuItemViewModel?>? SubMenuItems { get; }
}
