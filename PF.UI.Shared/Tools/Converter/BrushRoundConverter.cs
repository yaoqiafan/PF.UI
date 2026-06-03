using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PF.UI.Shared.Data.ColorManipulation;

namespace PF.UI.Shared.Tools.Converter;

/// <summary>
/// 根据画刷亮度返回高对比色（浅色背景→黑，深色背景→白）。
/// 用于 Chip Outlined 变体的鼠标悬浮覆盖层颜色。
/// </summary>
public class BrushRoundConverter : IValueConverter
{
    public Brush HighValue { get; set; } = Brushes.White;
    public Brush LowValue { get; set; } = Brushes.Black;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush) return null;
        return brush.Color.IsLightColor() ? HighValue : LowValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
