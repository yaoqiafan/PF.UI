using System.Windows.Media;

namespace PF.UI.Shared.Data.ColorManipulation;

public static class ColorAssist
{
    public static Color ContrastingForegroundColor(this Color color)
        => color.IsLightColor() ? Colors.Black : Colors.White;

    public static bool IsLightColor(this Color color)
    {
        static double ToLinear(double d)
        {
            d /= 255.0;
            return d > 0.03928 ? Math.Pow((d + 0.055) / 1.055, 2.4) : d / 12.92;
        }
        double luminance = 0.2126 * ToLinear(color.R)
                         + 0.7152 * ToLinear(color.G)
                         + 0.0722 * ToLinear(color.B);
        return luminance > 0.179;
    }

    public static bool IsDarkColor(this Color color) => !color.IsLightColor();
}
