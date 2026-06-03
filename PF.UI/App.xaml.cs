using System.Windows;
using PF.UI.Views;
using PF.UI.Views.Demos;
using Prism.Ioc;

namespace PF.UI
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<PackIconDemoView>();
            containerRegistry.RegisterForNavigation<RippleDemoView>();
            containerRegistry.RegisterForNavigation<ColorManipulationDemoView>();
            containerRegistry.RegisterForNavigation<ColorPickerDemoView>();
            containerRegistry.RegisterForNavigation<ChipDemoView>();
            containerRegistry.RegisterForNavigation<DialogDemoView>();
            containerRegistry.RegisterForNavigation<ColorPaletteDemoView>();
        }
    }
}
