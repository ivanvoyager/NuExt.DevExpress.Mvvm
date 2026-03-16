using DevExpress.Mvvm.UI.Interactivity;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevExpress.Mvvm.UI;

public class TreeViewDragDropBehavior : Behavior<TreeView>
{
    public static readonly DependencyProperty MoveCommandProperty = DependencyProperty.Register(
        nameof(MoveCommand), typeof(ICommand), typeof(TreeViewDragDropBehavior));

    private Rectangle? _dragBoxFromMouseDown;
    private object? _mouseDownOriginalSource;

    #region Properties

    public ICommand? MoveCommand
    {
        get => (ICommand?)GetValue(MoveCommandProperty);
        set => SetValue(MoveCommandProperty, value);
    }

    #endregion

    #region Event Handlers

    private static void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.GetData(typeof(IDragDrop)) is not IDragDrop draggedObject || GetDragDrop(e.OriginalSource)?.CanDrop(draggedObject) != true)
        {
            e.Effects = DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.Move;
        }
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.GetData(typeof(IDragDrop)) is IDragDrop draggedObject && MoveCommand?.CanExecute(draggedObject) != false && GetDragDrop(e.OriginalSource)?.Drop(draggedObject) == true)
        {
            e.Handled = true;
            MoveCommand?.Execute(draggedObject);
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            _dragBoxFromMouseDown = null;
            _mouseDownOriginalSource = null;
            return;
        }

        var position = e.GetPosition(null);

        if (_dragBoxFromMouseDown == null || _dragBoxFromMouseDown.Value.Contains((int)position.X, (int)position.Y))
        {
            return;
        }

        IDragDrop? dragDrop;
        try
        {
            dragDrop = GetDragDrop(_mouseDownOriginalSource);
        }
        finally
        {
            _dragBoxFromMouseDown = null;
            _mouseDownOriginalSource = null;
        }

        if (dragDrop is { CanDrag: true })
        {
            DragDrop.DoDragDrop(AssociatedObject, new DataObject(typeof(IDragDrop), dragDrop), DragDropEffects.Move);
        }
    }

    private void OnMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
    {
        Debug.Assert(Equals(sender, AssociatedObject));

        var position = e.GetPosition(null);

        _mouseDownOriginalSource = e.OriginalSource;

        _dragBoxFromMouseDown = new Rectangle(
            (int)(position.X - SystemParameters.MinimumHorizontalDragDistance),
            (int)(position.Y - SystemParameters.MinimumVerticalDragDistance),
            (int)(SystemParameters.MinimumHorizontalDragDistance * 2),
            (int)(SystemParameters.MinimumVerticalDragDistance * 2));
    }

    private void OnMouseLeftButtonUp(object? sender, MouseButtonEventArgs e)
    {
        _dragBoxFromMouseDown = null;
        _mouseDownOriginalSource = null;
    }

    #endregion

    #region Methods

    private static IDragDrop? GetDragDrop(object? source)
    {
        if (source is not DependencyObject obj) return null;
        var treeViewItem = obj as TreeViewItem ?? obj.FindParent<TreeViewItem>();
        return treeViewItem?.DataContext as IDragDrop;
    }

    protected override void OnAttached()
    {
        AssociatedObject.DragOver += OnDragOver;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.PreviewMouseMove += OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
        }
        base.OnDetaching();
    }

    #endregion
}
