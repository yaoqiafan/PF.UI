using PF.UI.Shared.Data.MaterialColors;
using PF.UI.Shared.Data.ColorManipulation;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PF.UI.ViewModels.Demos
{
    public class SwatchItem
    {
        public string Name { get; init; } = string.Empty;
        public Color SampleColor { get; init; }
        public SolidColorBrush SampleBrush { get; init; } = Brushes.Transparent;
        public string SampleHex { get; init; } = string.Empty;
        public IEnumerable<HueItem> Hues { get; init; } = [];
    }

    public class HueItem
    {
        public string Label { get; init; } = string.Empty;
        public Color Color { get; init; }
        public SolidColorBrush Brush { get; init; } = Brushes.Transparent;
        public string Hex { get; init; } = string.Empty;
        public bool IsDarkText { get; init; }
    }

    public class ColorPaletteDemoViewModel : BindableBase
    {
        public ObservableCollection<SwatchItem> Swatches { get; } = new();

        public ColorPaletteDemoViewModel()
        {
            foreach (var swatch in SwatchHelper.Swatches)
            {
                var lookup = swatch.Lookup;
                var hues = new List<HueItem>();
                foreach (var kv in lookup.OrderBy(k => (int)k.Key))
                {
                    var c = kv.Value;
                    hues.Add(new HueItem
                    {
                        Label = kv.Key.ToString(),
                        Color = c,
                        Brush = new SolidColorBrush(c),
                        Hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}",
                        IsDarkText = !c.IsLightColor()
                    });
                }

                // 取 500 层级颜色作为样色
                var sampleKey = Enum.Parse<MaterialDesignColor>(swatch.Name);
                var sampleColor = lookup.TryGetValue(sampleKey, out var c5) ? c5 : lookup.Values.First();

                Swatches.Add(new SwatchItem
                {
                    Name = swatch.Name,
                    SampleColor = sampleColor,
                    SampleBrush = new SolidColorBrush(sampleColor),
                    SampleHex = $"#{sampleColor.R:X2}{sampleColor.G:X2}{sampleColor.B:X2}",
                    Hues = hues
                });
            }
        }
    }
}
