using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PF.UI.Controls;

/// <summary>
/// 页内模态对话框。代码构建遮罩层（Border + 居中卡片），不依赖 ControlTemplate。
/// 静态 API：DialogOverlay.Show(content, token) / DialogOverlay.Close(result, token)
/// </summary>
public class DialogOverlay : ContentControl
{
    private static readonly Dictionary<string, DialogOverlay> _registry = new();

    private TaskCompletionSource<object?>? _tcs;

    // 遮罩层
    private readonly Border _overlayBorder;
    // 对话卡片容器
    private readonly ContentControl _cardHost;
    private readonly ScaleTransform _cardScale;
    // 根 Grid（存放 ContentPresenter + 遮罩 + 卡片）
    private Grid? _rootGrid;

    // ── 静态 API ────────────────────────────────────────────────────
    public static readonly RoutedCommand CloseDialogCommand = new("CloseDialogCommand", typeof(DialogOverlay));

    static DialogOverlay()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogOverlay),
            new FrameworkPropertyMetadata(typeof(DialogOverlay)));
    }

    public DialogOverlay()
    {
        // 构建遮罩
        _overlayBorder = new Border
        {
            Visibility = Visibility.Collapsed,
            Opacity = 0,
            IsHitTestVisible = true,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        // 绑定 OverlayBackground
        var bgBinding = new System.Windows.Data.Binding(nameof(OverlayBackground)) { Source = this };
        _overlayBorder.SetBinding(Border.BackgroundProperty, bgBinding);
        _overlayBorder.MouseLeftButtonDown += OnOverlayClick;

        // 构建卡片
        _cardScale = new ScaleTransform(0.7, 0.7);
        _cardHost = new ContentControl
        {
            Visibility = Visibility.Collapsed,
            Opacity = 0,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = _cardScale,
            Focusable = false,
        };
        var cardBinding = new System.Windows.Data.Binding(nameof(DialogContent)) { Source = this };
        _cardHost.SetBinding(ContentControl.ContentProperty, cardBinding);

        // 路由命令（按钮关闭）
        CommandBindings.Add(new CommandBinding(CloseDialogCommand, (_, e) =>
        {
            DialogResult = e.Parameter;
            IsOpen = false;
        }));

        Loaded += RegisterMe;
        Unloaded += UnregisterMe;
    }

    private void RegisterMe(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Token))
            _registry[Token] = this;
    }

    private void UnregisterMe(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Token) && _registry.TryGetValue(Token, out var v) && v == this)
            _registry.Remove(Token);
    }

    private void OnOverlayClick(object sender, MouseButtonEventArgs e)
    {
        if (CloseOnClickAway && e.OriginalSource == _overlayBorder)
        {
            DialogResult = null;
            IsOpen = false;
        }
    }

    // ── 重写：把自己的遮罩和卡片插入到 Grid 中 ────────────────────────
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // 找到模板根 Grid，把遮罩和卡片加进去
        if (VisualTreeHelper.GetChildrenCount(this) > 0
            && VisualTreeHelper.GetChild(this, 0) is Grid grid)
        {
            _rootGrid = grid;
            _rootGrid.Children.Add(_overlayBorder);
            _rootGrid.Children.Add(_cardHost);
        }
    }

    // ── Token ──────────────────────────────────────────────────────
    public static readonly DependencyProperty TokenProperty =
        DependencyProperty.Register(nameof(Token), typeof(string), typeof(DialogOverlay),
            new PropertyMetadata(string.Empty, OnTokenChanged));

    public string Token { get => (string)GetValue(TokenProperty); set => SetValue(TokenProperty, value); }

    private static void OnTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ov = (DialogOverlay)d;
        if (e.OldValue is string old && _registry.TryGetValue(old, out var s) && s == ov)
            _registry.Remove(old);
        if (e.NewValue is string tok && !string.IsNullOrEmpty(tok))
            _registry[tok] = ov;
    }

    // ── IsOpen ─────────────────────────────────────────────────────
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(DialogOverlay),
            new PropertyMetadata(false, OnIsOpenChanged));

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ov = (DialogOverlay)d;
        if ((bool)e.NewValue) ov.PlayOpen(); else ov.PlayClose();
    }

    // ── 其余 DP ────────────────────────────────────────────────────
    public static readonly DependencyProperty DialogContentProperty =
        DependencyProperty.Register(nameof(DialogContent), typeof(object), typeof(DialogOverlay));
    public object? DialogContent { get => GetValue(DialogContentProperty); set => SetValue(DialogContentProperty, value); }

    public static readonly DependencyProperty DialogResultProperty =
        DependencyProperty.Register(nameof(DialogResult), typeof(object), typeof(DialogOverlay));
    public object? DialogResult { get => GetValue(DialogResultProperty); set => SetValue(DialogResultProperty, value); }

    public static readonly DependencyProperty OverlayBackgroundProperty =
        DependencyProperty.Register(nameof(OverlayBackground), typeof(Brush), typeof(DialogOverlay),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x8F, 0, 0, 0))));
    public Brush OverlayBackground { get => (Brush)GetValue(OverlayBackgroundProperty); set => SetValue(OverlayBackgroundProperty, value); }

    public static readonly DependencyProperty CloseOnClickAwayProperty =
        DependencyProperty.Register(nameof(CloseOnClickAway), typeof(bool), typeof(DialogOverlay), new PropertyMetadata(false));
    public bool CloseOnClickAway { get => (bool)GetValue(CloseOnClickAwayProperty); set => SetValue(CloseOnClickAwayProperty, value); }

    // ── 静态 API ────────────────────────────────────────────────────
    public static Task<object?> Show(object content, string token = "")
    {
        var ov = Resolve(token)
            ?? throw new InvalidOperationException("未找到 DialogOverlay 实例。请在 XAML 中声明并确保页面已加载。");
        return ov.Dispatcher.CheckAccess() ? ov.OpenInternal(content)
            : ov.Dispatcher.Invoke(() => ov.OpenInternal(content));
    }

    public static void Close(object? result = null, string token = "")
    {
        var ov = Resolve(token);
        if (ov == null) return;
        if (ov.Dispatcher.CheckAccess())
        {
            ov.DialogResult = result;
            ov.IsOpen = false;
        }
        else
        {
            ov.Dispatcher.Invoke(() => { ov.DialogResult = result; ov.IsOpen = false; });
        }
    }

    private static DialogOverlay? Resolve(string token)
    {
        if (!string.IsNullOrEmpty(token) && _registry.TryGetValue(token, out var n)) return n;
        return _registry.Count > 0 ? _registry.Values.First() : null;
    }

    private Task<object?> OpenInternal(object content)
    {
        _tcs = new TaskCompletionSource<object?>();
        DialogContent = content;
        DialogResult = null;
        IsOpen = true;
        return _tcs.Task;
    }

    // ── 动画 ────────────────────────────────────────────────────────
    private void PlayOpen()
    {
        // 确保 Grid 已就绪
        if (_rootGrid == null && VisualTreeHelper.GetChildrenCount(this) > 0
            && VisualTreeHelper.GetChild(this, 0) is Grid g)
        {
            _rootGrid = g;
            _rootGrid.Children.Add(_overlayBorder);
            _rootGrid.Children.Add(_cardHost);
        }

        // 重置
        _overlayBorder.BeginAnimation(OpacityProperty, null);
        _cardHost.BeginAnimation(OpacityProperty, null);
        _cardScale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
        _cardScale.BeginAnimation(ScaleTransform.ScaleYProperty, null);

        _overlayBorder.Opacity = 0;
        _overlayBorder.Visibility = Visibility.Visible;
        _cardHost.Opacity = 0;
        _cardHost.Visibility = Visibility.Visible;
        _cardScale.ScaleX = _cardScale.ScaleY = 0.7;

        var dur = new Duration(TimeSpan.FromMilliseconds(280));
        var ease = new SineEase { EasingMode = EasingMode.EaseOut };

        _overlayBorder.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 0.56, dur) { EasingFunction = ease });
        _cardHost.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 1, dur) { EasingFunction = ease });
        _cardScale.BeginAnimation(ScaleTransform.ScaleXProperty,
            new DoubleAnimation(0.7, 1, dur) { EasingFunction = ease });
        _cardScale.BeginAnimation(ScaleTransform.ScaleYProperty,
            new DoubleAnimation(0.7, 1, dur) { EasingFunction = ease });
    }

    private void PlayClose()
    {
        _overlayBorder.BeginAnimation(OpacityProperty, null);
        _cardHost.BeginAnimation(OpacityProperty, null);

        var dur = new Duration(TimeSpan.FromMilliseconds(200));
        var ease = new SineEase { EasingMode = EasingMode.EaseIn };

        var anim = new DoubleAnimation(0, dur) { EasingFunction = ease };
        anim.Completed += (_, _) =>
        {
            _overlayBorder.Visibility = Visibility.Collapsed;
            _cardHost.Visibility = Visibility.Collapsed;
            _tcs?.TrySetResult(DialogResult);
            _tcs = null;
        };
        _overlayBorder.BeginAnimation(OpacityProperty, anim);

        _cardHost.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, dur) { EasingFunction = ease });
        _cardScale.BeginAnimation(ScaleTransform.ScaleXProperty,
            new DoubleAnimation(0.85, dur) { EasingFunction = ease });
        _cardScale.BeginAnimation(ScaleTransform.ScaleYProperty,
            new DoubleAnimation(0.85, dur) { EasingFunction = ease });
    }
}
