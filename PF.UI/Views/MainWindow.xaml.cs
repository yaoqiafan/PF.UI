using System.Windows;
using System.Windows.Controls;
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
            NavSearchBar.Focus();
        }

        private void NavDrawer_Closed(object sender, RoutedEventArgs e)
        {
            HamburgerIcon.Kind = PackIconKind.Menu;
            // 关闭抽屉时清空搜索并归还焦点
            NavSearchBar.Text = string.Empty;
        }

        private void NavSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SidebarNav.FilterText = NavSearchBar.Text;
        }
    }
}
