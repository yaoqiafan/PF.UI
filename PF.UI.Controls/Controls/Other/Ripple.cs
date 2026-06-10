using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PF.UI.Controls;

[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateNormal)]
[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMousePressed)]
[TemplateVisualState(GroupName = "CommonStates", Name = TemplateStateMouseOut)]
public class Ripple : ContentControl
{
    public const string TemplateStateNormal = "Normal";
    public const string TemplateStateMousePressed = "MousePressed";
    public const string TemplateStateMouseOut = "MouseOut";

    private static readonly HashSet<Ripple> PressedInstances = new();

    static Ripple()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));

        EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
        EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
        EventManager.RegisterClassHandler(typeof(Popup), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
        EventManager.RegisterClassHandler(typeof(Popup), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
    }

    public Ripple()
    {
        SizeChanged += OnSizeChanged;
    }

    private static void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    {
        if (PressedInstances.Count == 0)
            return;

        foreach (var ripple in PressedInstances)
        {
            var scaleTrans = ripple.Template.FindName("ScaleTransform", ripple) as ScaleTransform;
            if (scaleTrans != null)
            {
                double currentScale = scaleTrans.ScaleX;
                var newTime = TimeSpan.FromMilliseconds(300 * (1.0 - currentScale));

                if (ripple.Template.FindName("MousePressedToNormalScaleXKeyFrame", ripple) is EasingDoubleKeyFrame scaleX)
                    scaleX.KeyTime = KeyTime.FromTimeSpan(newTime);
                if (ripple.Template.FindName("MousePressedToNormalScaleYKeyFrame", ripple) is EasingDoubleKeyFrame scaleY)
                    scaleY.KeyTime = KeyTime.FromTimeSpan(newTime);
            }

            VisualStateManager.GoToState(ripple, TemplateStateNormal, true);
        }
        PressedInstances.Clear();
    }

    private static void MouseMoveEventHandler(object sender, MouseEventArgs e)
    {
        // 该处理器注册在所有 ContentControl 上，应用内任何鼠标移动都会途经此处，
        // 必须在无涟漪按压时零开销返回
        if (PressedInstances.Count == 0)
            return;

        // 已在 UI 线程；移除项延后收集，常规路径（按住未移出）零分配
        List<Ripple>? toRemove = null;
        foreach (var ripple in PressedInstances)
        {
            var pos = Mouse.GetPosition(ripple);
            if (pos.X < 0 || pos.Y < 0 || pos.X >= ripple.ActualWidth || pos.Y >= ripple.ActualHeight)
            {
                VisualStateManager.GoToState(ripple, TemplateStateMouseOut, true);
                (toRemove ??= new List<Ripple>()).Add(ripple);
            }
        }

        if (toRemove != null)
        {
            foreach (var ripple in toRemove)
                PressedInstances.Remove(ripple);
        }
    }

    public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register(
        nameof(Feedback), typeof(Brush), typeof(Ripple), new PropertyMetadata(default(Brush)));

    public Brush? Feedback
    {
        get => (Brush?)GetValue(FeedbackProperty);
        set => SetValue(FeedbackProperty, value);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (RippleAssist.GetIsCentered(this))
        {
            if (Content is FrameworkElement inner)
            {
                var pos = inner.TransformToAncestor(this).Transform(new Point(0, 0));
                RippleX = FlowDirection == FlowDirection.RightToLeft
                    ? pos.X - inner.ActualWidth / 2 - RippleSize / 2
                    : pos.X + inner.ActualWidth / 2 - RippleSize / 2;
                RippleY = pos.Y + inner.ActualHeight / 2 - RippleSize / 2;
            }
            else
            {
                RippleX = ActualWidth / 2 - RippleSize / 2;
                RippleY = ActualHeight / 2 - RippleSize / 2;
            }
        }
        else
        {
            var point = e.GetPosition(this);
            RippleX = point.X - RippleSize / 2;
            RippleY = point.Y - RippleSize / 2;
        }

        if (!RippleAssist.GetIsDisabled(this))
        {
            VisualStateManager.GoToState(this, TemplateStateNormal, false);
            VisualStateManager.GoToState(this, TemplateStateMousePressed, true);
            PressedInstances.Add(this);
        }

        base.OnPreviewMouseLeftButtonDown(e);
    }

    private static readonly DependencyPropertyKey RippleSizePropertyKey =
        DependencyProperty.RegisterReadOnly("RippleSize", typeof(double), typeof(Ripple), new PropertyMetadata(default(double)));
    public static readonly DependencyProperty RippleSizeProperty = RippleSizePropertyKey.DependencyProperty;
    public double RippleSize
    {
        get => (double)GetValue(RippleSizeProperty);
        private set => SetValue(RippleSizePropertyKey, value);
    }

    private static readonly DependencyPropertyKey RippleXPropertyKey =
        DependencyProperty.RegisterReadOnly("RippleX", typeof(double), typeof(Ripple), new PropertyMetadata(default(double)));
    public static readonly DependencyProperty RippleXProperty = RippleXPropertyKey.DependencyProperty;
    public double RippleX
    {
        get => (double)GetValue(RippleXProperty);
        private set => SetValue(RippleXPropertyKey, value);
    }

    private static readonly DependencyPropertyKey RippleYPropertyKey =
        DependencyProperty.RegisterReadOnly("RippleY", typeof(double), typeof(Ripple), new PropertyMetadata(default(double)));
    public static readonly DependencyProperty RippleYProperty = RippleYPropertyKey.DependencyProperty;
    public double RippleY
    {
        get => (double)GetValue(RippleYProperty);
        private set => SetValue(RippleYPropertyKey, value);
    }

    public static readonly DependencyProperty RecognizesAccessKeyProperty =
        DependencyProperty.Register(nameof(RecognizesAccessKey), typeof(bool), typeof(Ripple), new PropertyMetadata(default(bool)));
    public bool RecognizesAccessKey
    {
        get => (bool)GetValue(RecognizesAccessKeyProperty);
        set => SetValue(RecognizesAccessKeyProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(Ripple),
        new PropertyMetadata(default(CornerRadius), OnCornerRadiusChanged));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((Ripple)d).UpdateClip();

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        VisualStateManager.GoToState(this, TemplateStateNormal, false);
        if (TemplatedParent is FrameworkElement parent)
        {
            // Only propagate IsRippleEnabled → IsDisabled when the parent has explicitly set the
            // property. If it's still at the metadata default (false), leave IsDisabled alone so
            // that ItemsControl containers (e.g. ListBoxItem) aren't permanently disabled due to
            // the timing gap between PrepareContainerForItemOverride and template application.
            var src = DependencyPropertyHelper.GetValueSource(parent, RippleAssist.IsRippleEnabledProperty);
            if (src.BaseValueSource > BaseValueSource.Default)
                SetCurrentValue(RippleAssist.IsDisabledProperty, !RippleAssist.GetIsRippleEnabled(parent));
            SetCurrentValue(FeedbackProperty, RippleAssist.GetFeedback(parent));
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        double width, height;
        if (RippleAssist.GetIsCentered(this) && Content is FrameworkElement inner)
        {
            width = inner.ActualWidth;
            height = inner.ActualHeight;
        }
        else
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
        }

        var radius = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
        RippleSize = 2 * radius * RippleAssist.GetRippleSizeMultiplier(this);
        UpdateClip();
    }

    private void UpdateClip()
    {
        var cr = CornerRadius;
        if (cr == default || ActualWidth < double.Epsilon || ActualHeight < double.Epsilon)
        {
            Clip = null;
            return;
        }

        double w = ActualWidth, h = ActualHeight;

        // 等值圆角：用 RectangleGeometry，WPF 内置快速路径，比 PathGeometry 轻约 10 倍
        if (cr.TopLeft == cr.TopRight && cr.TopLeft == cr.BottomRight && cr.TopLeft == cr.BottomLeft)
        {
            var rectClip = new RectangleGeometry(new Rect(0, 0, w, h), cr.TopLeft, cr.TopLeft);
            rectClip.Freeze();
            Clip = rectClip;
            return;
        }

        // 非等值圆角：回退到 PathGeometry
        var clip = new PathGeometry
        {
            Figures = new PathFigureCollection
            {
                new PathFigure(new Point(cr.TopLeft, 0), new PathSegment[]
                {
                    new LineSegment(new Point(w - cr.TopRight, 0), false),
                    new ArcSegment(new Point(w, cr.TopRight), new Size(cr.TopRight, cr.TopRight), 90, false, SweepDirection.Clockwise, false),
                    new LineSegment(new Point(w, h - cr.BottomRight), false),
                    new ArcSegment(new Point(w - cr.BottomRight, h), new Size(cr.BottomRight, cr.BottomRight), 90, false, SweepDirection.Clockwise, false),
                    new LineSegment(new Point(cr.BottomLeft, h), false),
                    new ArcSegment(new Point(0, h - cr.BottomLeft), new Size(cr.BottomLeft, cr.BottomLeft), 90, false, SweepDirection.Clockwise, false),
                    new LineSegment(new Point(0, cr.TopLeft), false),
                    new ArcSegment(new Point(cr.TopLeft, 0), new Size(cr.TopLeft, cr.TopLeft), 90, false, SweepDirection.Clockwise, false),
                }, false)
            }
        };
        clip.Freeze();
        Clip = clip;
    }
}
