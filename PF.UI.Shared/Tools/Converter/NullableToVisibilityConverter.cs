using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PF.UI.Shared.Tools.Converter;

public class NullableToVisibilityConverter : IValueConverter
{
    public Visibility NullValue { get; set; } = Visibility.Collapsed;
    public Visibility NotNullValue { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is null ? NullValue : NotNullValue;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
