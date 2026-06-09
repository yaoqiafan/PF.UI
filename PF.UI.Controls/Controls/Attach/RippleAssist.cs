using System.Windows;
using System.Windows.Media;

namespace PF.UI.Controls;

public static class RippleAssist
{
    #region ClipToBounds

    public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.RegisterAttached(
        "ClipToBounds", typeof(bool), typeof(RippleAssist),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetClipToBounds(DependencyObject element, bool value) => element.SetValue(ClipToBoundsProperty, value);
    public static bool GetClipToBounds(DependencyObject element) => (bool)element.GetValue(ClipToBoundsProperty);

    #endregion

    #region IsCentered

    public static readonly DependencyProperty IsCenteredProperty = DependencyProperty.RegisterAttached(
        "IsCentered", typeof(bool), typeof(RippleAssist),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetIsCentered(DependencyObject element, bool value) => element.SetValue(IsCenteredProperty, value);
    public static bool GetIsCentered(DependencyObject element) => (bool)element.GetValue(IsCenteredProperty);

    #endregion

    #region IsDisabled

    public static readonly DependencyProperty IsDisabledProperty = DependencyProperty.RegisterAttached(
        "IsDisabled", typeof(bool), typeof(RippleAssist),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetIsDisabled(DependencyObject element, bool value) => element.SetValue(IsDisabledProperty, value);
    public static bool GetIsDisabled(DependencyObject element) => (bool)element.GetValue(IsDisabledProperty);

    #endregion

    #region RippleSizeMultiplier

    public static readonly DependencyProperty RippleSizeMultiplierProperty = DependencyProperty.RegisterAttached(
        "RippleSizeMultiplier", typeof(double), typeof(RippleAssist),
        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetRippleSizeMultiplier(DependencyObject element, double value) => element.SetValue(RippleSizeMultiplierProperty, value);
    public static double GetRippleSizeMultiplier(DependencyObject element) => (double)element.GetValue(RippleSizeMultiplierProperty);

    #endregion

    #region Feedback

    public static readonly DependencyProperty FeedbackProperty = DependencyProperty.RegisterAttached(
        "Feedback", typeof(Brush), typeof(RippleAssist),
        new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnFeedbackChanged));

    private static void OnFeedbackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Ripple ripple && ripple.TemplatedParent is FrameworkElement parent)
            ripple.SetCurrentValue(Ripple.FeedbackProperty, GetFeedback(parent));
    }

    public static void SetFeedback(DependencyObject element, Brush value) => element.SetValue(FeedbackProperty, value);
    public static Brush GetFeedback(DependencyObject element) => (Brush)element.GetValue(FeedbackProperty);

    #endregion

    #region RippleOnTop

    public static readonly DependencyProperty RippleOnTopProperty = DependencyProperty.RegisterAttached(
        "RippleOnTop", typeof(bool), typeof(RippleAssist),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

    public static void SetRippleOnTop(DependencyObject element, bool value) => element.SetValue(RippleOnTopProperty, value);
    public static bool GetRippleOnTop(DependencyObject element) => (bool)element.GetValue(RippleOnTopProperty);

    #endregion

    #region IsRippleEnabled

    /// <summary>
    /// 主开关：设置为 True 时，模板内嵌的 Ripple 效果将激活。
    /// 继承属性，父级设置后自动传递给所有后代。
    /// 命名为 IsRippleEnabled 以避免与 UIElement.IsEnabled 产生绑定路径歧义。
    /// </summary>
    public static readonly DependencyProperty IsRippleEnabledProperty = DependencyProperty.RegisterAttached(
        "IsRippleEnabled", typeof(bool), typeof(RippleAssist),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsRippleEnabledChanged));

    private static void OnIsRippleEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Ripple ripple && ripple.TemplatedParent is FrameworkElement parent)
            ripple.SetCurrentValue(IsDisabledProperty, !GetIsRippleEnabled(parent));
    }

    public static void SetIsRippleEnabled(DependencyObject element, bool value) => element.SetValue(IsRippleEnabledProperty, value);
    public static bool GetIsRippleEnabled(DependencyObject element) => (bool)element.GetValue(IsRippleEnabledProperty);

    #endregion

    #region CornerRadius

    /// <summary>
    /// 传递给模板内嵌 Ripple 的圆角半径，使涟漪裁剪匹配宿主控件的圆角。
    /// 继承属性，通常由控件样式统一设置。
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
        "CornerRadius", typeof(CornerRadius), typeof(RippleAssist),
        new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.Inherits));

    public static void SetCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);
    public static CornerRadius GetCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(CornerRadiusProperty);

    #endregion
}
