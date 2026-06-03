using PF.UI.Shared.Data.ColorManipulation;
using Prism.Mvvm;
using System.Windows.Media;

namespace PF.UI.ViewModels.Demos
{
    public class ColorManipulationDemoViewModel : BindableBase
    {
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

        private string _hexCode = string.Empty;
        public string HexCode
        {
            get => _hexCode;
            set => SetProperty(ref _hexCode, value);
        }

        private string _hsbText = string.Empty;
        public string HsbText
        {
            get => _hsbText;
            set => SetProperty(ref _hsbText, value);
        }

        public ColorManipulationDemoViewModel()
        {
            Refresh();
        }

        private void Refresh()
        {
            var hsb = new Hsb(Hue, Saturation, Brightness);
            var color = hsb.ToColor();

            PreviewColor = color;
            PreviewBrush = new SolidColorBrush(color);
            HueGradient = new LinearGradientBrush(Colors.White, new Hsb(Hue, 1, 1).ToColor(), 0);
            HexCode = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            HsbText = $"H: {Hue:F0}°   S: {Saturation:P0}   B: {Brightness:P0}";

            // 验证 ConvertBack：Color → Hsb
            var roundTrip = color.ToHsb();
            RoundTripText = $"ToHsb → H:{roundTrip.Hue:F1}° S:{roundTrip.Saturation:F2} B:{roundTrip.Brightness:F2}";
        }

        private string _roundTripText = string.Empty;
        public string RoundTripText
        {
            get => _roundTripText;
            set => SetProperty(ref _roundTripText, value);
        }
    }
}
