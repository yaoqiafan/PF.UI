using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using PF.UI.Controls;
using PF.UI.Models;
using PF.UI.Views.Demos;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace PF.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public ObservableCollection<NavItem> NavItems { get; } = new();

        private NavItem? _selectedItem;
        public NavItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                    _regionManager.RequestNavigate("DemoRegion", value.ViewName);
            }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            NavItems.Add(new NavItem
            {
                Title = "PackIcon",
                Description = "700+ Material 图标库",
                ViewName = nameof(PackIconDemoView),
                Icon = PackIconKind.StarOutline
            });

            NavItems.Add(new NavItem
            {
                Title = "Ripple",
                Description = "涟漪点击效果",
                ViewName = nameof(RippleDemoView),
                Icon = PackIconKind.Waves
            });

            NavItems.Add(new NavItem
            {
                Title = "ColorManipulation",
                Description = "HSB 色彩空间工具",
                ViewName = nameof(ColorManipulationDemoView),
                Icon = PackIconKind.Palette
            });

            NavItems.Add(new NavItem
            {
                Title = "ColorPicker",
                Description = "颜色选择器",
                ViewName = nameof(ColorPickerDemoView),
                Icon = PackIconKind.EyedropperVariant
            });

            NavItems.Add(new NavItem
            {
                Title = "Chip",
                Description = "标签 / 胶囊控件",
                ViewName = nameof(ChipDemoView),
                Icon = PackIconKind.Label
            });

            NavItems.Add(new NavItem
            {
                Title = "Dialog",
                Description = "页内模态对话框",
                ViewName = nameof(DialogDemoView),
                Icon = PackIconKind.WindowMaximize
            });

            NavItems.Add(new NavItem
            {
                Title = "Color Palette",
                Description = "19种 Material Design 颜色",
                ViewName = nameof(ColorPaletteDemoView),
                Icon = PackIconKind.Palette
            });

            // Region 在窗口 Loaded 后才可用，延迟到 Loaded 优先级再导航
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                () => SelectedItem = NavItems[0]);
        }
    }
}
