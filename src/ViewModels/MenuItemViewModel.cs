using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents a view model for a menu item that can include a command, header, and submenu items.
/// Inherits from BindableBase to support property change notifications.
/// </summary>
public class MenuItemViewModel : BindableBase, IMenuItemViewModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the command associated with this menu item.
    /// </summary>
    public ICommand? Command
    {
        get => GetProperty(() => Command);
        set { SetProperty(() => Command, value); }
    }

    /// <summary>
    /// Gets or sets the parameter to be passed to the command.
    /// </summary>
    public object? CommandParameter
    {
        get => GetProperty(() => CommandParameter);
        set { SetProperty(() => CommandParameter, value); }
    }

    /// <summary>
    /// Gets or sets the header text of the menu item.
    /// </summary>
    public string Header
    {
        get => GetProperty(() => Header);
        set { SetProperty(() => Header, value); }
    }

    /// <summary>
    /// Gets or sets the collection of submenu items.
    /// </summary>
    public ObservableCollection<IMenuItemViewModel?>? SubMenuItems
    {
        get => GetProperty(() => SubMenuItems);
        set { SetProperty(() => SubMenuItems, value); }
    }

    #endregion
}
