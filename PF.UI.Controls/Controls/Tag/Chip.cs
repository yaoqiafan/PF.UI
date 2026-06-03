using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PF.UI.Controls;

[TemplatePart(Name = DeleteButtonPartName, Type = typeof(Button))]
public class Chip : ButtonBase
{
    public const string DeleteButtonPartName = "PART_DeleteButton";

    private ButtonBase? _deleteButton;

    static Chip()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Chip), new FrameworkPropertyMetadata(typeof(Chip)));
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(object), typeof(Chip), new PropertyMetadata(default(object)));

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconBackgroundProperty =
        DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(Chip), new PropertyMetadata(default(Brush)));

    public Brush? IconBackground
    {
        get => (Brush?)GetValue(IconBackgroundProperty);
        set => SetValue(IconBackgroundProperty, value);
    }

    public static readonly DependencyProperty IconForegroundProperty =
        DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(Chip), new PropertyMetadata(default(Brush)));

    public Brush? IconForeground
    {
        get => (Brush?)GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    public static readonly DependencyProperty IsDeletableProperty =
        DependencyProperty.Register(nameof(IsDeletable), typeof(bool), typeof(Chip), new PropertyMetadata(false));

    public bool IsDeletable
    {
        get => (bool)GetValue(IsDeletableProperty);
        set => SetValue(IsDeletableProperty, value);
    }

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(Chip), new PropertyMetadata(default(ICommand)));

    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public static readonly DependencyProperty DeleteCommandParameterProperty =
        DependencyProperty.Register(nameof(DeleteCommandParameter), typeof(object), typeof(Chip), new PropertyMetadata(default(object)));

    public object? DeleteCommandParameter
    {
        get => GetValue(DeleteCommandParameterProperty);
        set => SetValue(DeleteCommandParameterProperty, value);
    }

    public static readonly DependencyProperty DeleteToolTipProperty =
        DependencyProperty.Register(nameof(DeleteToolTip), typeof(object), typeof(Chip), new PropertyMetadata(default(object)));

    public object? DeleteToolTip
    {
        get => GetValue(DeleteToolTipProperty);
        set => SetValue(DeleteToolTipProperty, value);
    }

    [Category("Behavior")]
    public event RoutedEventHandler DeleteClick
    {
        add => AddHandler(DeleteClickEvent, value);
        remove => RemoveHandler(DeleteClickEvent, value);
    }

    public static readonly RoutedEvent DeleteClickEvent =
        EventManager.RegisterRoutedEvent(nameof(DeleteClick), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(Chip));

    public override void OnApplyTemplate()
    {
        if (_deleteButton != null)
            _deleteButton.Click -= DeleteButtonOnClick;

        _deleteButton = GetTemplateChild(DeleteButtonPartName) as ButtonBase;

        if (_deleteButton != null)
            _deleteButton.Click += DeleteButtonOnClick;

        base.OnApplyTemplate();
    }

    protected virtual void OnDeleteClick()
    {
        RaiseEvent(new RoutedEventArgs(DeleteClickEvent, this));
        if (DeleteCommand?.CanExecute(DeleteCommandParameter) ?? false)
            DeleteCommand.Execute(DeleteCommandParameter);
    }

    private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
    {
        OnDeleteClick();
        e.Handled = true;
    }
}
