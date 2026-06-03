using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PF.UI.Controls;

/// <summary>
/// 标准弹窗内容容器：头部（图标 + 标题）· 内容区 · 按钮脚部。
/// 供 Prism IDialogService 的 View 使用，与 PFDialogBaseWindow 配合。
/// </summary>
public class DialogFrame : ContentControl
{
    static DialogFrame()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogFrame),
            new FrameworkPropertyMetadata(typeof(DialogFrame)));
    }

    // ── 标题文字 ───────────────────────────────────────────────────
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(DialogFrame),
            new PropertyMetadata(string.Empty));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    // ── 图标种类（PackIconKind） ────────────────────────────────────
    public static readonly DependencyProperty IconKindProperty =
        DependencyProperty.Register(nameof(IconKind), typeof(PackIconKind), typeof(DialogFrame),
            new PropertyMetadata(PackIconKind.InformationOutline));

    public PackIconKind IconKind
    {
        get => (PackIconKind)GetValue(IconKindProperty);
        set => SetValue(IconKindProperty, value);
    }

    // ── 是否显示图标 ────────────────────────────────────────────────
    public static readonly DependencyProperty ShowIconProperty =
        DependencyProperty.Register(nameof(ShowIcon), typeof(bool), typeof(DialogFrame),
            new PropertyMetadata(true));

    public bool ShowIcon
    {
        get => (bool)GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    // ── 图标背景色 ─────────────────────────────────────────────────
    public static readonly DependencyProperty IconBackgroundProperty =
        DependencyProperty.Register(nameof(IconBackground), typeof(Brush), typeof(DialogFrame),
            new PropertyMetadata(default(Brush)));

    public Brush? IconBackground
    {
        get => (Brush?)GetValue(IconBackgroundProperty);
        set => SetValue(IconBackgroundProperty, value);
    }

    // ── 图标前景色 ─────────────────────────────────────────────────
    public static readonly DependencyProperty IconForegroundProperty =
        DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(DialogFrame),
            new PropertyMetadata(Brushes.White));

    public Brush IconForeground
    {
        get => (Brush)GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    // ── 脚部内容（按钮区） ──────────────────────────────────────────
    public static readonly DependencyProperty FooterContentProperty =
        DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(DialogFrame),
            new PropertyMetadata(default(object)));

    public object? FooterContent
    {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    // ── 是否显示脚部 ────────────────────────────────────────────────
    public static readonly DependencyProperty ShowFooterProperty =
        DependencyProperty.Register(nameof(ShowFooter), typeof(bool), typeof(DialogFrame),
            new PropertyMetadata(true));

    public bool ShowFooter
    {
        get => (bool)GetValue(ShowFooterProperty);
        set => SetValue(ShowFooterProperty, value);
    }
}
