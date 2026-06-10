using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PF.UI.Shared.Tools.Converter;

public class BorderClipConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 3 && values[0] is double width && values[1] is double height && values[2] is CornerRadius radius)
        {
            if (width < double.Epsilon || height < double.Epsilon)
            {
                return Geometry.Empty;
            }

            // 等值圆角：用 RectangleGeometry，WPF 内置快速路径，比 PathGeometry 轻约 10 倍
            if (radius.TopLeft == radius.TopRight && radius.TopLeft == radius.BottomRight && radius.TopLeft == radius.BottomLeft)
            {
                var rectClip = new RectangleGeometry(new Rect(0, 0, width, height), radius.TopLeft, radius.TopLeft);
                rectClip.Freeze();
                return rectClip;
            }

            // 非等值圆角：回退到 PathGeometry
            var clip = new PathGeometry
            {
                Figures = new PathFigureCollection
                {
                    new(new Point(radius.TopLeft, 0), new PathSegment[]
                    {
                        new LineSegment(new Point(width - radius.TopRight, 0), false),
                        new ArcSegment(new Point(width, radius.TopRight), new Size(radius.TopRight, radius.TopRight), 90, false, SweepDirection.Clockwise, false),
                        new LineSegment(new Point(width, height - radius.BottomRight), false),
                        new ArcSegment(new Point(width - radius.BottomRight, height), new Size(radius.BottomRight, radius.BottomRight), 90, false, SweepDirection.Clockwise, false),
                        new LineSegment(new Point(radius.BottomLeft, height), false),
                        new ArcSegment(new Point(0, height - radius.BottomLeft), new Size(radius.BottomLeft, radius.BottomLeft), 90, false, SweepDirection.Clockwise, false),
                        new LineSegment(new Point(0, radius.TopLeft), false),
                        new ArcSegment(new Point(radius.TopLeft, 0), new Size(radius.TopLeft, radius.TopLeft), 90, false, SweepDirection.Clockwise, false),
                    }, false)
                }
            };
            clip.Freeze();

            return clip;
        }

        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
