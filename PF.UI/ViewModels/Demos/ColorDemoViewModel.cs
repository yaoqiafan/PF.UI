using PF.UI.Shared.Data.ColorManipulation;
using PF.UI.Shared.Data.MaterialColors;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace PF.UI.ViewModels.Demos
{
    // 调色板数据模型（从 ColorPaletteDemoViewModel 迁移）
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

    public class ColorDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "ThemeBrushes", Title = "主题色系",      Sub = "Color · Brush Key" },
            new DemoTocItem { Anchor = "ColorPicker",  Title = "ColorPicker",   Sub = "HSB 拾色器" },
            new DemoTocItem { Anchor = "HSB",          Title = "HSB 工具",      Sub = "色彩空间转换" },
            new DemoTocItem { Anchor = "Palette",      Title = "Material 调色板", Sub = "19种 × 14个色调" },
        };

        // ─── Section: ColorPicker ──────────────────────────────────────────

        private Color _selectedColor = Color.FromRgb(2, 173, 139);
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (SetProperty(ref _selectedColor, value))
                {
                    SelectedBrush = new SolidColorBrush(value);
                    PickerHexCode = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                    PickerRgbText = $"R: {value.R}  G: {value.G}  B: {value.B}";
                }
            }
        }

        private SolidColorBrush _selectedBrush = new(Color.FromRgb(2, 173, 139));
        public SolidColorBrush SelectedBrush
        {
            get => _selectedBrush;
            set => SetProperty(ref _selectedBrush, value);
        }

        private string _pickerHexCode = "#02AD8B";
        public string PickerHexCode
        {
            get => _pickerHexCode;
            set => SetProperty(ref _pickerHexCode, value);
        }

        private string _pickerRgbText = "R: 2  G: 173  B: 139";
        public string PickerRgbText
        {
            get => _pickerRgbText;
            set => SetProperty(ref _pickerRgbText, value);
        }

        private Dock _hueSliderPosition = Dock.Bottom;
        public Dock HueSliderPosition
        {
            get => _hueSliderPosition;
            set => SetProperty(ref _hueSliderPosition, value);
        }

        // ─── Section: HSB 工具 ─────────────────────────────────────────────

        private double _hue = 180;
        public double Hue
        {
            get => _hue;
            set { SetProperty(ref _hue, value); Refresh(); }
        }

        private double _saturation = 0.8;
        public double Saturation
        {
            get => _saturation;
            set { SetProperty(ref _saturation, value); Refresh(); }
        }

        private double _brightness = 0.9;
        public double Brightness
        {
            get => _brightness;
            set { SetProperty(ref _brightness, value); Refresh(); }
        }

        private Color _previewColor;
        public Color PreviewColor
        {
            get => _previewColor;
            set => SetProperty(ref _previewColor, value);
        }

        private SolidColorBrush _previewBrush = Brushes.Transparent;
        public SolidColorBrush PreviewBrush
        {
            get => _previewBrush;
            set => SetProperty(ref _previewBrush, value);
        }

        private LinearGradientBrush _hueGradient = new();
        public LinearGradientBrush HueGradient
        {
            get => _hueGradient;
            set => SetProperty(ref _hueGradient, value);
        }

        private string _hsbHexCode = string.Empty;
        public string HsbHexCode
        {
            get => _hsbHexCode;
            set => SetProperty(ref _hsbHexCode, value);
        }

        private string _hsbText = string.Empty;
        public string HsbText
        {
            get => _hsbText;
            set => SetProperty(ref _hsbText, value);
        }

        private string _roundTripText = string.Empty;
        public string RoundTripText
        {
            get => _roundTripText;
            set => SetProperty(ref _roundTripText, value);
        }

        private void Refresh()
        {
            var hsb = new Hsb(Hue, Saturation, Brightness);
            var color = hsb.ToColor();
            PreviewColor = color;
            PreviewBrush = new SolidColorBrush(color);
            HueGradient = new LinearGradientBrush(Colors.White, new Hsb(Hue, 1, 1).ToColor(), 0);
            HsbHexCode = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            HsbText = $"H: {Hue:F0}°   S: {Saturation:P0}   B: {Brightness:P0}";
            var rt = color.ToHsb();
            RoundTripText = $"ToHsb → H:{rt.Hue:F1}° S:{rt.Saturation:F2} B:{rt.Brightness:F2}";
        }

        // ─── Section: Material Design 调色板 ──────────────────────────────

        public ObservableCollection<SwatchItem> Swatches { get; } = new();

        public ColorDemoViewModel()
        {
            Refresh();

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

        // ─── 代码示例 ──────────────────────────────────────────────────────

        public const string XamlThemeBrushes = @"<!-- 直接使用主题画刷（DynamicResource 支持深色/浅色主题切换）-->
<Border Background=""{DynamicResource PrimaryBrush}"" />
<Border Background=""{DynamicResource DangerBrush}"" />
<TextBlock Foreground=""{DynamicResource PrimaryTextBrush}"" />
<Border Effect=""{StaticResource EffectShadow3}"" />

<!-- 自定义主题色：在 App.xaml 中覆写 ColorDefault.xaml 的 Color Key -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source=""/PF.UI.Resources;component/Themes/Default.xaml""/>
            <ResourceDictionary Source=""/PF.UI.Resources;component/Colors/Default.xaml""/>
        </ResourceDictionary.MergedDictionaries>
        <!-- 覆写主色（Color Key，Brush 自动跟随）-->
        <Color x:Key=""PrimaryColor"">#6200EE</Color>
        <Color x:Key=""DarkPrimaryColor"">#3700B3</Color>
    </ResourceDictionary>
</Application.Resources>

<!-- 深色主题：将 Default.xaml 替换为 Dark.xaml -->
<ResourceDictionary Source=""/PF.UI.Resources;component/Colors/Dark.xaml""/>

<!-- 所有 Brush Key 一览（SolidColorBrush 除 LinearGradient 外均可 StaticResource）
     品牌：LightPrimaryBrush / PrimaryBrush / DarkPrimaryBrush / AccentBrush
     状态：LightDangerBrush / DangerBrush / DarkDangerBrush  （Warning/Info/Success 同结构）
     文字：PrimaryTextBrush / SecondaryTextBrush / ThirdlyTextBrush / ReverseTextBrush
     背景：BackgroundBrush / RegionBrush / SecondaryRegionBrush / ThirdlyRegionBrush
     边框：BorderBrush / SecondaryBorderBrush
     阴影：EffectShadow1 ~ EffectShadow5 -->";

        public const string XamlColorPicker = @"<!-- ColorPicker：HSB 颜色选择器 -->
<pf:ColorPicker Color=""{Binding SelectedColor, Mode=TwoWay}""
                HueSliderPosition=""Bottom""
                Height=""280"" />

<!-- HueSliderPosition 枚举：Bottom（默认）| Top | Left | Right -->
<!-- Color 属性类型：System.Windows.Media.Color
     常配合 Color2HexStringConverter 显示十六进制字符串 -->
<TextBlock Text=""{Binding SelectedColor,
           Converter={StaticResource Color2Hex}}"" />";

        public const string XamlHsb = @"<!-- HSB 色彩空间：H（色调 0-360）S（饱和度 0-1）B（亮度 0-1）-->
using PF.UI.Shared.Data.ColorManipulation;

// Hsb → Color
var hsb = new Hsb(hue: 180, saturation: 0.8, brightness: 0.9);
Color color = hsb.ToColor();

// Color → Hsb（扩展方法）
Hsb roundTrip = color.ToHsb();

// HsbLinearGradientConverter：色调渐变（Hue → LinearGradientBrush）
// White → 当前色调满饱和度色（常用于颜色选择器背景渐变）
<Border>
    <Border.Background>
        <Binding Path=""Hue"" Converter=""{StaticResource Hsb2Grad}"" />
    </Border.Background>
</Border>

// HsbToColorConverter（IMultiValueConverter：[H, S, B] → Color）
<MultiBinding Converter=""{StaticResource Hsb2Color}"">
    <Binding Path=""Hue"" />
    <Binding Path=""Saturation"" />
    <Binding Path=""Brightness"" />
</MultiBinding>";

        public const string XamlPalette = @"using PF.UI.Shared.Data.MaterialColors;

// 直接查色（MaterialDesignColor 枚举包含全部 266 个色值 Key）
Color red500   = SwatchHelper.Lookup[MaterialDesignColor.Red];
Color tealA200 = SwatchHelper.Lookup[MaterialDesignColor.TealA200];

// 遍历所有 19 种颜色的调色板
foreach (var swatch in SwatchHelper.Swatches)
{
    Console.WriteLine(swatch.Name); // Red, Blue, Teal, Green ...
    foreach (var kv in swatch.Lookup)
        Console.WriteLine($""{kv.Key}: #{kv.Value.R:X2}{kv.Value.G:X2}{kv.Value.B:X2}"");
}

// 创建特定调色板实例
var teal = new TealSwatch();
Color tealA700 = teal.Lookup[MaterialDesignColor.TealA700];

// IsLightColor() 扩展方法：判断颜色明度，用于决定文字颜色
bool useDarkText = !color.IsLightColor(); // true → 用白色文字";
    }
}
