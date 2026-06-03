using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PF.UI.Controls;

public class PackIcon : Control
{
    private static readonly Lazy<IDictionary<PackIconKind, string>> _dataIndex
        = new Lazy<IDictionary<PackIconKind, string>>(PackIconDataFactory.Create);

    static PackIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
    }

    public static readonly DependencyProperty KindProperty
        = DependencyProperty.Register(nameof(Kind), typeof(PackIconKind), typeof(PackIcon),
            new PropertyMetadata(default(PackIconKind), KindPropertyChangedCallback));

    private static void KindPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((PackIcon)d).UpdateData();

    public PackIconKind Kind
    {
        get => (PackIconKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    private static readonly DependencyPropertyKey DataPropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(""));

    public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

    [TypeConverter(typeof(GeometryConverter))]
    public string? Data
    {
        get => (string?)GetValue(DataProperty);
        private set => SetValue(DataPropertyKey, value);
    }

    public static readonly DependencyProperty ScaleToSizeOfWithProperty =
        DependencyProperty.Register(nameof(ScaleToSizeOfWith), typeof(FrameworkElement), typeof(PackIcon),
            new PropertyMetadata(null, OnScaleToSizeOfWithChanged));

    public FrameworkElement? ScaleToSizeOfWith
    {
        get => (FrameworkElement?)GetValue(ScaleToSizeOfWithProperty);
        set => SetValue(ScaleToSizeOfWithProperty, value);
    }

    private static void OnScaleToSizeOfWithChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var icon = (PackIcon)d;
        BindingOperations.ClearBinding(icon, HeightProperty);
        BindingOperations.ClearBinding(icon, WidthProperty);

        if (e.NewValue is FrameworkElement source)
        {
            var binding = new Binding(nameof(FrameworkElement.ActualHeight))
            {
                Source = source,
                Mode = BindingMode.OneWay
            };
            icon.SetBinding(HeightProperty, binding);
            icon.SetBinding(WidthProperty, binding);
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateData();
    }

    private void UpdateData()
    {
        string? data = null;
        _dataIndex.Value?.TryGetValue(Kind, out data);
        Data = data;
    }
}
