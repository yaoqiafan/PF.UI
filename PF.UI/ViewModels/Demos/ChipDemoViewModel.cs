using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ChipDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Filled",    Title = "Filled",    Sub = "实色填充变体" },
            new DemoTocItem { Anchor = "Outlined",  Title = "Outlined",  Sub = "描边轮廓变体" },
            new DemoTocItem { Anchor = "Icon",      Title = "带图标",    Sub = "Icon 属性" },
            new DemoTocItem { Anchor = "Deletable", Title = "可删除",    Sub = "IsDeletable + 标签云" },
        };

        private string _lastResult = "与下方 Chip 控件交互...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public const string XamlFilled = @"<!-- Chip Filled 变体 — 直接指定命名样式即可 -->
<pf:Chip Content=""默认"" />
<pf:Chip Content=""主色"" Style=""{StaticResource ChipPrimary}"" />
<pf:Chip Content=""成功"" Style=""{StaticResource ChipSuccess}"" />
<pf:Chip Content=""危险"" Style=""{StaticResource ChipDanger}"" />
<pf:Chip Content=""警告"" Style=""{StaticResource ChipWarning}"" />";

        public const string XamlOutlined = @"<!-- Chip Outlined 描边变体 -->
<pf:Chip Content=""描边默认"" Style=""{StaticResource ChipOutlined}"" />
<pf:Chip Content=""描边主色"" Style=""{StaticResource ChipOutlinedPrimary}"" />";

        public const string XamlIcon = @"<!-- Icon 属性 — 接受任意 UIElement，显示在文字左侧 -->
<pf:Chip Content=""用户"">
    <pf:Chip.Icon>
        <pf:PackIcon Kind=""Account"" Width=""18"" Height=""18"" Foreground=""White"" />
    </pf:Chip.Icon>
</pf:Chip>

<!-- IconBackground / IconForeground 可单独控制图标区颜色 -->
<pf:Chip Content=""收藏"" Style=""{StaticResource ChipPrimary}"">
    <pf:Chip.Icon>
        <pf:PackIcon Kind=""Star"" Width=""18"" Height=""18"" Foreground=""White"" />
    </pf:Chip.Icon>
</pf:Chip>";

        public const string XamlDeletable = @"<!-- IsDeletable=""True"" — 显示右侧删除按钮 -->
<pf:Chip Content=""可删除"" IsDeletable=""True"" />
<pf:Chip Content=""主色可删除"" IsDeletable=""True""
         Style=""{StaticResource ChipPrimary}"" />

<!-- DeleteCommand 绑定命令，DeleteToolTip 设提示 -->
<pf:Chip Content=""带命令"" IsDeletable=""True""
         DeleteToolTip=""点击删除此标签""
         DeleteCommand=""{Binding RemoveTagCommand}"" />

<!-- 标签云场景：在 WrapPanel 中排列 -->
<WrapPanel>
    <pf:Chip Content=""WPF"" IsDeletable=""True"" />
    <pf:Chip Content="".NET 8"" IsDeletable=""True"" Style=""{StaticResource ChipPrimary}"" />
    <pf:Chip Content=""MVVM"" IsDeletable=""True"" Style=""{StaticResource ChipOutlinedPrimary}"" />
</WrapPanel>";
    }
}
