using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PF.UI.Shared.Data.ColorManipulation;

namespace PF.UI.Controls;

[TemplatePart(Name = HueSliderPartName, Type = typeof(Slider))]
[TemplatePart(Name = SaturationBrightnessPickerPartName, Type = typeof(Canvas))]
[TemplatePart(Name = SaturationBrightnessPickerThumbPartName, Type = typeof(Thumb))]
public class ColorPicker : Control
{
    public const string HueSliderPartName = "PART_HueSlider";
    public const string SaturationBrightnessPickerPartName = "PART_SaturationBrightnessPicker";
    public const string SaturationBrightnessPickerThumbPartName = "PART_SaturationBrightnessPickerThumb";

    static ColorPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
    }

    private Thumb? _saturationBrightnessThumb;
    private Canvas? _saturationBrightnessCanvas;
    private Slider? _hueSlider;
    private bool _inCallback;

    // ── Color（双向绑定，对外公开）──────────────────────────────────
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color), typeof(Color), typeof(ColorPicker),
        new FrameworkPropertyMetadata(default(Color),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            ColorPropertyChangedCallback));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private static void ColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var cp = (ColorPicker)d;
        if (cp._inCallback) return;
        cp._inCallback = true;
        cp.SetCurrentValue(HsbProperty, ((Color)e.NewValue).ToHsb());
        cp.RaiseEvent(new RoutedPropertyChangedEventArgs<Color>((Color)e.OldValue, (Color)e.NewValue)
        {
            RoutedEvent = ColorChangedEvent
        });
        cp._inCallback = false;
    }

    // ── ColorChanged 路由事件 ───────────────────────────────────────
    public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ColorChanged), RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));

    public event RoutedPropertyChangedEventHandler<Color> ColorChanged
    {
        add => AddHandler(ColorChangedEvent, value);
        remove => RemoveHandler(ColorChangedEvent, value);
    }

    // ── Hsb（内部，驱动 2D 画布渐变）──────────────────────────────
    internal static readonly DependencyProperty HsbProperty = DependencyProperty.Register(
        nameof(Hsb), typeof(Hsb), typeof(ColorPicker),
        new FrameworkPropertyMetadata(default(Hsb),
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            HsbPropertyChangedCallback));

    internal Hsb Hsb
    {
        get => (Hsb)GetValue(HsbProperty);
        set => SetValue(HsbProperty, value);
    }

    private static void HsbPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var cp = (ColorPicker)d;
        if (cp._inCallback) return;
        cp._inCallback = true;
        cp.SetCurrentValue(ColorProperty, e.NewValue is Hsb hsb ? hsb.ToColor() : default(Color));
        cp._inCallback = false;
    }

    // ── HueSliderPosition ──────────────────────────────────────────
    public static readonly DependencyProperty HueSliderPositionProperty = DependencyProperty.Register(
        nameof(HueSliderPosition), typeof(Dock), typeof(ColorPicker),
        new PropertyMetadata(Dock.Bottom));

    public Dock HueSliderPosition
    {
        get => (Dock)GetValue(HueSliderPositionProperty);
        set => SetValue(HueSliderPositionProperty, value);
    }

    // ── 模板应用 ────────────────────────────────────────────────────
    public override void OnApplyTemplate()
    {
        if (_saturationBrightnessCanvas != null)
        {
            _saturationBrightnessCanvas.MouseDown -= SaturationBrightnessCanvasMouseDown;
            _saturationBrightnessCanvas.MouseMove -= SaturationBrightnessCanvasMouseMove;
            _saturationBrightnessCanvas.MouseUp   -= SaturationBrightnessCanvasMouseUp;
        }
        _saturationBrightnessCanvas = GetTemplateChild(SaturationBrightnessPickerPartName) as Canvas;
        if (_saturationBrightnessCanvas != null)
        {
            _saturationBrightnessCanvas.MouseDown += SaturationBrightnessCanvasMouseDown;
            _saturationBrightnessCanvas.MouseMove += SaturationBrightnessCanvasMouseMove;
            _saturationBrightnessCanvas.MouseUp   += SaturationBrightnessCanvasMouseUp;
        }

        if (_saturationBrightnessThumb != null) _saturationBrightnessThumb.DragDelta -= SaturationBrightnessThumbDragDelta;
        _saturationBrightnessThumb = GetTemplateChild(SaturationBrightnessPickerThumbPartName) as Thumb;
        if (_saturationBrightnessThumb != null) _saturationBrightnessThumb.DragDelta += SaturationBrightnessThumbDragDelta;

        if (_hueSlider != null) _hueSlider.ValueChanged -= HueSliderOnValueChanged;
        _hueSlider = GetTemplateChild(HueSliderPartName) as Slider;
        if (_hueSlider != null) _hueSlider.ValueChanged += HueSliderOnValueChanged;

        base.OnApplyTemplate();
    }

    // ── 事件处理 ────────────────────────────────────────────────────
    private void HueSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        => Hsb = new Hsb(e.NewValue, Hsb.Saturation, Hsb.Brightness);

    private void SaturationBrightnessCanvasMouseDown(object sender, MouseButtonEventArgs e)
        => _saturationBrightnessThumb?.CaptureMouse();

    private void SaturationBrightnessCanvasMouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.Captured != _saturationBrightnessThumb) return;
        if (e.LeftButton == MouseButtonState.Pressed)
            ApplyThumbPosition(e.GetPosition(_saturationBrightnessCanvas));
    }

    private void SaturationBrightnessCanvasMouseUp(object sender, MouseButtonEventArgs e)
        => _saturationBrightnessThumb?.ReleaseMouseCapture();

    private void SaturationBrightnessThumbDragDelta(object sender, DragDeltaEventArgs e)
        => ApplyThumbPosition(Mouse.GetPosition(_saturationBrightnessCanvas));

    // ── 布局与定位 ──────────────────────────────────────────────────
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var result = base.ArrangeOverride(arrangeBounds);
        SetThumbLeft();
        SetThumbTop();
        return result;
    }

    private void ApplyThumbPosition(Point position)
    {
        if (_saturationBrightnessCanvas is null) return;
        double left = Math.Clamp(position.X, 0, _saturationBrightnessCanvas.ActualWidth);
        double top  = Math.Clamp(position.Y, 0, _saturationBrightnessCanvas.ActualHeight);

        double saturation = _saturationBrightnessCanvas.ActualWidth > 0
            ? left / _saturationBrightnessCanvas.ActualWidth : 0;
        double brightness = _saturationBrightnessCanvas.ActualHeight > 0
            ? 1 - top / _saturationBrightnessCanvas.ActualHeight : 0;

        SetCurrentValue(HsbProperty, new Hsb(Hsb.Hue, saturation, brightness));
    }

    private void SetThumbLeft()
    {
        if (_saturationBrightnessCanvas is null || _saturationBrightnessThumb is null) return;
        Canvas.SetLeft(_saturationBrightnessThumb, Hsb.Saturation * _saturationBrightnessCanvas.ActualWidth);
    }

    private void SetThumbTop()
    {
        if (_saturationBrightnessCanvas is null || _saturationBrightnessThumb is null) return;
        Canvas.SetTop(_saturationBrightnessThumb, (1 - Hsb.Brightness) * _saturationBrightnessCanvas.ActualHeight);
    }
}
