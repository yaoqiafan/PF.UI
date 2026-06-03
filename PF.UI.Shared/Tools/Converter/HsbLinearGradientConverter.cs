using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PF.UI.Shared.Data.ColorManipulation;

namespace PF.UI.Shared.Tools.Converter;

public class HsbLinearGradientConverter : IValueConverter
{
    public static readonly HsbLinearGradientConverter Instance = new();

    // hue (double) → LinearGradientBrush（白色 → 该色调满饱和度色）
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double hue) return Binding.DoNothing;
        return new LinearGradientBrush(Colors.White, new Hsb(hue, 1, 1).ToColor(), 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
