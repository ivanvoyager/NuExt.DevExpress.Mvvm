using System.Text.Json.Serialization;
using System.Windows;

namespace DevExpress.Mvvm;

/// <summary>
/// Represents a base class for models that can be expanded or collapsed.
/// </summary>
public abstract class ExpandableBase : Bindable, IExpandable
{
    /// <summary>
    /// Gets or sets a value indicating whether the object is expanded.
    /// </summary>
    [JsonIgnore]
    public bool IsExpanded
    {
        get { return GetProperty(() => IsExpanded); }
        set { SetProperty(() => IsExpanded, value); }
    }

    /// <summary>
    /// Collapses the object by setting the <see cref="IsExpanded"/> property to <see langword="false"/>.
    /// </summary>
    public virtual void Collapse()
    {
        IsExpanded = false;
    }

    /// <summary>
    /// Expands the object by setting the <see cref="IsExpanded"/> property to <see langword="true"/>.
    /// </summary>
    public virtual void Expand()
    {
        IsExpanded = true;
    }
}
