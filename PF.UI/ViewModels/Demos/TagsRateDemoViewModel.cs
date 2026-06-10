using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class TagsRateDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Tag",          Title = "Tag",          Sub = "可选中 / 可关闭标签" },
            new DemoTocItem { Anchor = "TagContainer", Title = "TagContainer", Sub = "集合容器自动管理" },
            new DemoTocItem { Anchor = "Rate",         Title = "Rate",         Sub = "星级评分控件" },
        };

        private string _lastResult = "与下方控件交互查看效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ===== Rate bindings =====
        private double _rateValue;
        public double RateValue
        {
            get => _rateValue;
            set
            {
                if (SetProperty(ref _rateValue, value))
                    LastResult = $"评分：{value} 星  ({DateTime.Now:HH:mm:ss})";
            }
        }

        private double _rateValueHalf;
        public double RateValueHalf
        {
            get => _rateValueHalf;
            set
            {
                if (SetProperty(ref _rateValueHalf, value))
                    LastResult = $"半星评分：{value} 星  ({DateTime.Now:HH:mm:ss})";
            }
        }

        // ===== TagContainer 数据绑定示例 =====
        public ObservableCollection<string> TagItems { get; } = new()
        {
            "WPF", ".NET 8", "MVVM", "Prism", "EF Core", "SQLite"
        };

        public const string XamlTag = @"<!-- Tag 基本用法 -->
<pf:Tag Content=""基础标签"" />
<pf:Tag Content=""无关闭按钮"" ShowCloseButton=""False"" />

<!-- Header 属性 — 在内容前显示前缀徽章 -->
<pf:Tag Header=""NEW"" Content=""新功能标签"" />
<pf:Tag Header=""HOT"" Content=""热门内容"" />

<!-- Selectable=""True"" — 点击后 IsSelected=True，高亮显示 -->
<pf:Tag Content=""可选中标签"" Selectable=""True"" />

<!-- Closing / Closed 路由事件 — 关闭前可取消操作 -->
<pf:Tag Content=""可关闭标签"" Closing=""Tag_Closing"" Closed=""Tag_Closed"" />";

        public const string XamlTagContainer = @"<!-- TagContainer — 自动将子项包装为 Tag，关闭时自动移除 -->
<pf:TagContainer>
    <pf:Tag Content=""WPF"" />
    <pf:Tag Content="".NET 8"" />
    <pf:Tag Content=""MVVM"" />
</pf:TagContainer>

<!-- ShowCloseButton 附加属性（Inherits）— 全局隐藏关闭按钮 -->
<pf:TagContainer pf:TagContainer.ShowCloseButton=""False"">
    <pf:Tag Content=""只读标签 A"" />
    <pf:Tag Content=""只读标签 B"" />
</pf:TagContainer>

<!-- ItemsSource 数据绑定 — 关闭时自动从集合中移除数据项 -->
<pf:TagContainer ItemsSource=""{Binding TagItems}"" />";

        public const string XamlRate = @"<!-- Rate 基本 5 星评分，双向绑定 Value -->
<pf:Rate Count=""5"" Value=""{Binding RateValue, Mode=TwoWay}"" />

<!-- AllowHalf=""True"" — 支持半星（0.5 步进） -->
<pf:Rate Count=""5"" AllowHalf=""True""
         Value=""{Binding RateValueHalf, Mode=TwoWay}"" />

<!-- IsReadOnly=""True"" — 只读展示，DefaultValue 设初始值 -->
<pf:Rate Count=""5"" IsReadOnly=""True"" DefaultValue=""3.5"" />

<!-- Count=""10"" AllowClear=""False"" — 10 星且不可清零 -->
<pf:Rate Count=""10"" AllowClear=""False"" />

<!-- ItemWidth / ItemHeight — 调整图标大小（默认 20x20） -->
<pf:Rate Count=""5"" ItemWidth=""32"" ItemHeight=""32"" />";
    }
}
