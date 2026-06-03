using System.Windows.Controls;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class ColorPickerDemoView : UserControl
    {
        public ColorPickerDemoView()
        {
            InitializeComponent();
        }

        private ColorPickerDemoViewModel? VM => DataContext as ColorPickerDemoViewModel;

        private void BottomChecked(object sender, System.Windows.RoutedEventArgs e) { if (VM != null) VM.HueSliderPosition = Dock.Bottom; }
        private void TopChecked(object sender, System.Windows.RoutedEventArgs e)    { if (VM != null) VM.HueSliderPosition = Dock.Top; }
        private void LeftChecked(object sender, System.Windows.RoutedEventArgs e)   { if (VM != null) VM.HueSliderPosition = Dock.Left; }
        private void RightChecked(object sender, System.Windows.RoutedEventArgs e)  { if (VM != null) VM.HueSliderPosition = Dock.Right; }
    }
}
