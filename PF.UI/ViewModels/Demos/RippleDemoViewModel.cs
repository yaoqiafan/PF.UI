using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class RippleDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Basic",             Title = "基本用法",       Sub = "点击处扩散涟漪" },
            new DemoTocItem { Anchor = "Centered",          Title = "居中涟漪",       Sub = "IsCentered=True" },
            new DemoTocItem { Anchor = "ListItem",          Title = "列表项",         Sub = "行级点击反馈" },
            new DemoTocItem { Anchor = "ButtonIntegration", Title = "Button 内层集成", Sub = "IsEnabled 一键开启" },
            new DemoTocItem { Anchor = "Inherit",           Title = "继承传递",       Sub = "容器统一开关" },
            new DemoTocItem { Anchor = "Options",           Title = "附加属性",       Sub = "IsDisabled · SizeMultiplier" },
        };

        private string _lastResult = "点击下方控件查看涟漪动画效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public const string XamlBasic = @"<!-- Ripple 基本用法：外层 Border 设 ClipToBounds=""True""，内容包裹在 pf:Ripple 中 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        Background=""{DynamicResource PrimaryBrush}"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center""
               VerticalContentAlignment=""Center""
               pf:RippleAssist.Feedback=""White"">
        <TextBlock Text=""实心按钮"" Foreground=""White"" />
    </pf:Ripple>
</Border>

<!-- 轮廓按钮：不设 Feedback 时使用主题默认涟漪色 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        BorderBrush=""{DynamicResource PrimaryBrush}"" BorderThickness=""1.5"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center""
               VerticalContentAlignment=""Center"">
        <TextBlock Text=""轮廓按钮"" Foreground=""{DynamicResource PrimaryBrush}"" />
    </pf:Ripple>
</Border>";

        public const string XamlCentered = @"<!-- IsCentered=""True"" — 涟漪始终从控件中心扩散（适合圆形图标按钮） -->
<Border CornerRadius=""24"" Width=""48"" Height=""48""
        Background=""{DynamicResource PrimaryBrush}"" ClipToBounds=""True"">
    <pf:Ripple pf:RippleAssist.IsCentered=""True""
               pf:RippleAssist.Feedback=""White"">
        <pf:PackIcon Kind=""Heart"" Width=""24"" Height=""24""
                     Foreground=""White"" HorizontalAlignment=""Center"" />
    </pf:Ripple>
</Border>

<!-- 默认 IsCentered=""False"" — 涟漪从鼠标点击位置扩散 -->
<Border CornerRadius=""24"" Width=""48"" Height=""48""
        Background=""{DynamicResource DangerBrush}"" ClipToBounds=""True"">
    <pf:Ripple pf:RippleAssist.IsCentered=""False""
               pf:RippleAssist.Feedback=""White"">
        <pf:PackIcon Kind=""Delete"" Width=""24"" Height=""24""
                     Foreground=""White"" HorizontalAlignment=""Center"" />
    </pf:Ripple>
</Border>";

        public const string XamlListItem = @"<!-- 列表项涟漪：每个行的外层 Border 设 ClipToBounds=""True"" -->
<Border CornerRadius=""8"" ClipToBounds=""True""
        BorderBrush=""{DynamicResource BorderBrush}"" BorderThickness=""1"">
    <StackPanel>
        <Border ClipToBounds=""True""
                BorderBrush=""{DynamicResource BorderBrush}"" BorderThickness=""0,0,0,1"">
            <pf:Ripple Padding=""16,12"">
                <StackPanel Orientation=""Horizontal"">
                    <pf:PackIcon Kind=""AccountOutline"" Width=""18"" Height=""18""
                                 VerticalAlignment=""Center"" Margin=""0,0,10,0"" />
                    <TextBlock Text=""用户管理"" VerticalAlignment=""Center"" />
                </StackPanel>
            </pf:Ripple>
        </Border>
        <Border ClipToBounds=""True"">
            <pf:Ripple Padding=""16,12"">
                <StackPanel Orientation=""Horizontal"">
                    <pf:PackIcon Kind=""CogOutline"" Width=""18"" Height=""18""
                                 VerticalAlignment=""Center"" Margin=""0,0,10,0"" />
                    <TextBlock Text=""系统设置"" VerticalAlignment=""Center"" />
                </StackPanel>
            </pf:Ripple>
        </Border>
    </StackPanel>
</Border>";

        public const string XamlButtonIntegration = @"<!-- 单个按钮开启内层涟漪：只需设置 IsRippleEnabled=""True"" -->
<!-- Feedback 颜色已按按钮类型预设，无需手动指定 -->
<Button Content=""Primary"" Style=""{StaticResource ButtonPrimary}""
        pf:RippleAssist.IsRippleEnabled=""True"" />

<Button Content=""Success"" Style=""{StaticResource ButtonSuccess}""
        pf:RippleAssist.IsRippleEnabled=""True"" />

<!-- 图标按钮：IsCentered=""True"" 已在样式中预设，涟漪从中心扩散 -->
<Button Style=""{StaticResource ButtonIconCircular}""
        Background=""{DynamicResource PrimaryBrush}""
        pf:RippleAssist.IsRippleEnabled=""True""
        pf:RippleAssist.Feedback=""White""
        pf:IconElement.Geometry=""{StaticResource SearchGeometry}"" />

<!-- 覆盖默认 Feedback 颜色 -->
<Button Style=""{StaticResource ButtonIcon}""
        pf:RippleAssist.IsRippleEnabled=""True""
        pf:RippleAssist.Feedback=""{DynamicResource DangerBrush}""
        pf:IconElement.Geometry=""{StaticResource DeleteGeometry}""
        Foreground=""{DynamicResource DangerBrush}"" />";

        public const string XamlInherit = @"<!-- IsEnabled 是可继承属性 — 在父级容器设置，内部所有子控件自动生效 -->
<StackPanel pf:RippleAssist.IsRippleEnabled=""True"">
    <Button Content=""Default"" Style=""{StaticResource ButtonDefault}"" />
    <Button Content=""Primary"" Style=""{StaticResource ButtonPrimary}"" />
    <Button Content=""Success"" Style=""{StaticResource ButtonSuccess}"" />
    <!-- ButtonDashed 同样生效，无需额外设置 -->
    <Button Style=""{StaticResource ButtonDashed}"" Content=""虚线按钮"" />
    <!-- 子级可通过 IsDisabled=""True"" 单独关闭涟漪 -->
    <Button Content=""此按钮无涟漪"" Style=""{StaticResource ButtonDefault}""
            pf:RippleAssist.IsDisabled=""True"" />
</StackPanel>

<!-- DataGrid 整体开启行涟漪（Phase 3 实现后） -->
<!-- <DataGrid pf:RippleAssist.IsRippleEnabled=""True"" /> -->";

        public const string XamlOptions = @"<!-- IsDisabled=""True"" — 禁用涟漪 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        Background=""{DynamicResource SecondaryRegionBrush}"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center"" VerticalContentAlignment=""Center""
               pf:RippleAssist.IsDisabled=""True"">
        <TextBlock Text=""无涟漪效果"" Foreground=""{DynamicResource ThirdlyTextBrush}"" />
    </pf:Ripple>
</Border>

<!-- RippleSizeMultiplier=""2"" — 涟漪半径放大 2 倍 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        Background=""{DynamicResource PrimaryBrush}"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center"" VerticalContentAlignment=""Center""
               pf:RippleAssist.Feedback=""White""
               pf:RippleAssist.RippleSizeMultiplier=""2"">
        <TextBlock Text=""超大涟漪"" Foreground=""White"" />
    </pf:Ripple>
</Border>

<!-- RippleOnTop=""True"" — 涟漪层渲染在内容之上 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        Background=""{DynamicResource DarkSuccessBrush}"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center"" VerticalContentAlignment=""Center""
               pf:RippleAssist.Feedback=""White""
               pf:RippleAssist.RippleOnTop=""True"">
        <TextBlock Text=""涟漪置顶"" Foreground=""White"" />
    </pf:Ripple>
</Border>

<!-- 自定义 Feedback 颜色 -->
<Border CornerRadius=""6"" Height=""40"" ClipToBounds=""True""
        Background=""#1A000000"" BorderBrush=""#FF4081"" BorderThickness=""1.5"">
    <pf:Ripple Padding=""20,0""
               HorizontalContentAlignment=""Center"" VerticalContentAlignment=""Center""
               pf:RippleAssist.Feedback=""#FF4081"">
        <TextBlock Text=""自定义颜色"" Foreground=""#FF4081"" />
    </pf:Ripple>
</Border>";
    }
}
