using System.Windows;
using PF.UI.Controls;

namespace PF.UI.Views
{
    public partial class MainWindow : PF.UI.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            NavDrawer.IsOpen = !NavDrawer.IsOpen;
        }

        private void NavDrawer_Opened(object sender, RoutedEventArgs e)
        {
            HamburgerIcon.Kind = PackIconKind.MenuOpen;
        }

        private void NavDrawer_Closed(object sender, RoutedEventArgs e)
        {
            HamburgerIcon.Kind = PackIconKind.Menu;
        }
    }
}
