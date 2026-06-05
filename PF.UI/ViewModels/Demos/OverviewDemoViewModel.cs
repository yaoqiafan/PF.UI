using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PF.UI.ViewModels.Demos
{
    public class OverviewDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "About",        Title = "关于 PF.UI",  Sub = "库简介与特性" },
            new DemoTocItem { Anchor = "QuickStart",   Title = "快速开始",    Sub = "安装与配置" },
            new DemoTocItem { Anchor = "Architecture", Title = "项目架构",    Sub = "四层依赖结构" },
            new DemoTocItem { Anchor = "Stats",        Title = "控件统计",    Sub = "覆盖控件数量" },
            new DemoTocItem { Anchor = "Usage",        Title = "使用本演示",  Sub = "导航与复制" },
        };
    }
}
