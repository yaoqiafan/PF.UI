using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class StatusIndicatorViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Badge",    Title = "Badge",    Sub = "文字 / 数字 / 状态圆点" },
            new DemoTocItem { Anchor = "Card",     Title = "Card",     Sub = "Header / Content / Footer" },
            new DemoTocItem { Anchor = "Divider",  Title = "Divider",  Sub = "线 / 文字 / 垂直 / 虚线" },
            new DemoTocItem { Anchor = "Empty",    Title = "Empty",    Sub = "空状态占位" },
        };

        private string _lastResult = "";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        private bool _showBadge = true;
        public bool ShowBadge
        {
            get => _showBadge;
            set
            {
                if (SetProperty(ref _showBadge, value))
                    LastResult = $"Badge 显示: {value}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }

        public ICommand LogCommand { get; }

        public StatusIndicatorViewModel()
        {
            LogCommand = new DelegateCommand<string>(msg =>
                LastResult = $"{msg}  ({System.DateTime.Now:HH:mm:ss})");
        }

        public const string XamlBadge = @"<!-- Badge — 角标容器 -->
<!-- 文字角标 -->
<pf:Badge Text=""新"" ShowBadge=""True"">
    <Button Content=""消息"" />
</pf:Badge>

<!-- 数字角标（Max=99 超过显示 99+） -->
<pf:Badge Value=""5"" Maximum=""99"" ShowBadge=""True"">
    <Button Content=""通知"" />
</pf:Badge>

<pf:Badge Value=""100"" Maximum=""99"" ShowBadge=""True"">
    <pf:PackIcon Kind=""Bell"" Width=""20"" Height=""20"" />
</pf:Badge>

<!-- 状态圆点 -->
<pf:Badge Status=""Success"" ShowBadge=""True"" />
<pf:Badge Status=""Danger"" ShowBadge=""True"" />";

        public const string XamlCard = @"<!-- Card — 容器卡片 -->
<pf:Card Header=""Card Header"" Footer=""Footer"">
    <TextBlock Text=""卡片内容区域"" />
</pf:Card>

<pf:Card>
    <StackPanel>
        <TextBlock FontWeight=""Bold"" Text=""标题"" />
        <TextBlock Text=""内容描述..."" />
    </StackPanel>
</pf:Card>";

        public const string XamlDivider = @"<!-- Divider — 分割线 -->
<!-- 纯线 -->
<pf:Divider />

<!-- 带文字 -->
<pf:Divider Content=""分割文字"" />

<!-- 垂直 -->
<StackPanel Orientation=""Horizontal"" Height=""40"">
    <TextBlock Text=""左"" VerticalAlignment=""Center"" />
    <pf:Divider Orientation=""Vertical"" />
    <TextBlock Text=""右"" VerticalAlignment=""Center"" />
</StackPanel>

<!-- 虚线 -->
<pf:Divider Content=""虚线"" LineStrokeDashArray=""4,2"" />";

        public const string XamlEmpty = @"<!-- Empty — 空状态 -->
<pf:Empty Description=""暂无数据"">
    <Button Content=""重新加载"" />
</pf:Empty>

<!-- 使用 Logo 自定义图标 -->
<pf:Empty Description=""没有找到匹配结果""
          Logo=""{StaticResource SearchGeometry}"">
    <Button Content=""清除筛选"" />
</pf:Empty>

<!-- ShowEmpty 附加属性动态切换 -->
<Grid>
    <pf:Empty Description=""数据为空""
              pf:Empty.ShowEmpty=""True"" />
    <TextBlock Text=""有数据时显示此内容""
               pf:Empty.ShowEmpty=""False"" />
</Grid>";
    }
}
