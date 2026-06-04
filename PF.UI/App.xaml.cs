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
            // 已有演示
            containerRegistry.RegisterForNavigation<PackIconDemoView>();
            containerRegistry.RegisterForNavigation<RippleDemoView>();
            containerRegistry.RegisterForNavigation<ColorManipulationDemoView>();
            containerRegistry.RegisterForNavigation<ColorPickerDemoView>();
            containerRegistry.RegisterForNavigation<ChipDemoView>();
            containerRegistry.RegisterForNavigation<DialogDemoView>();
            containerRegistry.RegisterForNavigation<ColorPaletteDemoView>();

            // 概览
            containerRegistry.RegisterForNavigation<OverviewDemoView>();

            // 按钮
            containerRegistry.RegisterForNavigation<ButtonsDemoView>();

            // 输入
            containerRegistry.RegisterForNavigation<TextInputDemoView>();
            containerRegistry.RegisterForNavigation<SelectorsDemoView>();

            // 数据
            containerRegistry.RegisterForNavigation<DataDemoView>();

            // 布局
            containerRegistry.RegisterForNavigation<PanelsDemoView>();

            // 导航
            containerRegistry.RegisterForNavigation<NavigationDemoView>();

            // 反馈（拆分为3页）
            containerRegistry.RegisterForNavigation<GrowlNotificationView>();
            containerRegistry.RegisterForNavigation<StatusIndicatorView>();
            containerRegistry.RegisterForNavigation<ProgressLoadingView>();

            // 动画
            containerRegistry.RegisterForNavigation<AnimationDemoView>();

            // 标签 & 评分
            containerRegistry.RegisterForNavigation<TagsRateDemoView>();

            // 滑块
            containerRegistry.RegisterForNavigation<SlidersDemoView>();

            // 时间
            containerRegistry.RegisterForNavigation<TimeDemoView>();

            // 附加属性
            containerRegistry.RegisterForNavigation<AttachedDemoView>();

            // 转换器
            containerRegistry.RegisterForNavigation<ConvertersDemoView>();

            // 交互
            containerRegistry.RegisterForNavigation<InteractivityDemoView>();

            // 窗口
            containerRegistry.RegisterForNavigation<WindowsDemoView>();
        }
    }
}
