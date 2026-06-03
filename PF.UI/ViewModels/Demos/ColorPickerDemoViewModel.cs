using System.Windows.Media;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ColorPickerDemoViewModel : BindableBase
    {
        private Color _selectedColor = Color.FromRgb(2, 173, 139);
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (SetProperty(ref _selectedColor, value))
                {
                    SelectedBrush = new SolidColorBrush(value);
                    HexCode = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                    RgbText = $"R: {value.R}  G: {value.G}  B: {value.B}";
                }
            }
        }

        private SolidColorBrush _selectedBrush = new(Color.FromRgb(2, 173, 139));
        public SolidColorBrush SelectedBrush
        {
            get => _selectedBrush;
            set => SetProperty(ref _selectedBrush, value);
        }

        private string _hexCode = "#02AD8B";
        public string HexCode
        {
            get => _hexCode;
            set => SetProperty(ref _hexCode, value);
        }

        private string _rgbText = "R: 2  G: 173  B: 139";
        public string RgbText
        {
            get => _rgbText;
            set => SetProperty(ref _rgbText, value);
        }

        private System.Windows.Controls.Dock _hueSliderPosition = System.Windows.Controls.Dock.Bottom;
        public System.Windows.Controls.Dock HueSliderPosition
        {
            get => _hueSliderPosition;
            set => SetProperty(ref _hueSliderPosition, value);
        }
    }
}
