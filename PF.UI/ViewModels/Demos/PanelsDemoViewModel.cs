using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class PanelsDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "FlexPanel",           Title = "FlexPanel",           Sub = "Flexbox 弹性布局" },
            new DemoTocItem { Anchor = "UniformSpacingPanel", Title = "UniformSpacingPanel", Sub = "等间距排列 / 自动换行" },
            new DemoTocItem { Anchor = "WaterfallPanel",      Title = "WaterfallPanel",      Sub = "瀑布流多列" },
            new DemoTocItem { Anchor = "CirclePanel",         Title = "CirclePanel",         Sub = "圆形排列" },
            new DemoTocItem { Anchor = "HoneycombPanel",      Title = "HoneycombPanel",      Sub = "蜂巢六边形" },
            new DemoTocItem { Anchor = "SimpleStackPanel",    Title = "SimpleStackPanel",    Sub = "轻量 StackPanel" },
            new DemoTocItem { Anchor = "ElementGroup",        Title = "ElementGroup",        Sub = "Stack / Uniform 分组" },
            new DemoTocItem { Anchor = "RowCol",              Title = "Row / Col",           Sub = "24 列网格 / ClipGrid" },
        };

        // ===== 代码示例 =====

        public const string XamlFlexPanel = @"<!-- FlexPanel — CSS Flexbox 风格弹性布局 -->

<!-- 横向排列（默认） -->
<pf:FlexPanel FlexDirection=""Row"" FlexWrap=""Wrap""
              JustifyContent=""SpaceBetween"" AlignItems=""Center"">
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
</pf:FlexPanel>

<!-- 纵向 + FlexGrow 拉伸 -->
<pf:FlexPanel FlexDirection=""Column"" Height=""200"">
    <Border Height=""40"" />
    <Border pf:FlexPanel.FlexGrow=""1"" />  <!-- 占剩余空间 -->
    <Border Height=""40"" />
</pf:FlexPanel>

<!-- 换行 + SpaceAround -->
<pf:FlexPanel FlexWrap=""Wrap"" JustifyContent=""SpaceAround"">
    <Border Width=""60"" Height=""60"" />
    <Border Width=""60"" Height=""60"" />
    <Border Width=""60"" Height=""60"" />
    <Border Width=""60"" Height=""60"" />
</pf:FlexPanel>";

        public const string XamlUniformSpacingPanel = @"<!-- UniformSpacingPanel — 统一间距排列 -->

<!-- 水平等间距 -->
<pf:UniformSpacingPanel Orientation=""Horizontal"" Spacing=""12"">
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
</pf:UniformSpacingPanel>

<!-- 自动换行 (ChildWrapping=Wrap) + 独立 H/V 间距 -->
<pf:UniformSpacingPanel ChildWrapping=""Wrap""
                         HorizontalSpacing=""12""
                         VerticalSpacing=""8"">
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
    <Border Width=""80"" Height=""40"" />
</pf:UniformSpacingPanel>";

        public const string XamlWaterfallPanel = @"<!-- WaterfallPanel — 瀑布流布局 -->
<!-- Groups 指定列数，子元素填入最短列 -->
<pf:WaterfallPanel Groups=""3"">
    <Border Height=""80"" />
    <Border Height=""120"" />
    <Border Height=""60"" />
    <Border Height=""100"" />
    <Border Height=""80"" />
    <Border Height=""90"" />
</pf:WaterfallPanel>

<!-- 纵向 Groups=2 -->
<pf:WaterfallPanel Groups=""2"" Orientation=""Vertical"" Height=""160"">
    <Border Width=""80"" />
    <Border Width=""120"" />
    <Border Width=""60"" />
    <Border Width=""100"" />
</pf:WaterfallPanel>";

        public const string XamlCirclePanel = @"<!-- CirclePanel — 将子元素均匀排列在圆形轨迹上 -->
<pf:CirclePanel Diameter=""180"" OffsetAngle=""-90"">
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
    <Border Width=""36"" Height=""36"" CornerRadius=""18"" />
</pf:CirclePanel>

<!-- KeepVertical=True：子元素保持垂直不随圆旋转 -->
<pf:CirclePanel Diameter=""160"" KeepVertical=""True"" OffsetAngle=""0"">
    <pf:PackIcon Kind=""Home"" Width=""20"" Height=""20"" />
    <pf:PackIcon Kind=""Star"" Width=""20"" Height=""20"" />
    <pf:PackIcon Kind=""Heart"" Width=""20"" Height=""20"" />
    <pf:PackIcon Kind=""Delete"" Width=""20"" Height=""20"" />
</pf:CirclePanel>";

        public const string XamlHoneycombPanel = @"<!-- HoneycombPanel — 蜂巢六边形排列 -->
<!-- 子元素将按蜂巢规律自动分布，无需额外属性 -->
<pf:HoneycombPanel>
    <Border />
    <Border />
    <Border />
    <Border />
    <Border />
    <Border />
    <Border />
</pf:HoneycombPanel>";

        public const string XamlSimpleStackPanel = @"<!-- SimpleStackPanel — 轻量 StackPanel，无 Margin/Padding 开销 -->
<pf:SimpleStackPanel Orientation=""Horizontal"">
    <Border Width=""60"" Height=""40"" />
    <Border Width=""60"" Height=""40"" />
    <Border Width=""60"" Height=""40"" />
</pf:SimpleStackPanel>

<pf:SimpleStackPanel Orientation=""Vertical"">
    <Border Width=""200"" Height=""36"" />
    <Border Width=""200"" Height=""36"" />
    <Border Width=""200"" Height=""36"" />
</pf:SimpleStackPanel>";

        public const string XamlElementGroup = @"<!-- ElementGroup — 统一管理一组元素，Stack/Uniform 两种布局 -->

<!-- Stack 布局：子项紧排 -->
<pf:ElementGroup Orientation=""Horizontal"" Layout=""Stack"">
    <Button Content=""剪切"" />
    <Button Content=""复制"" />
    <Button Content=""粘贴"" />
</pf:ElementGroup>

<!-- Uniform 布局：子项等宽填满 -->
<pf:ElementGroup Orientation=""Horizontal"" Layout=""Uniform"">
    <Button Content=""左对齐"" />
    <Button Content=""居中"" />
    <Button Content=""右对齐"" />
</pf:ElementGroup>";

        public const string XamlRowCol = @"<!-- Row / Col — 24 列响应式网格（类 Bootstrap） -->
<pf:Row Gutter=""16"">
    <pf:Col Span=""8""><Border Height=""40"" /></pf:Col>
    <pf:Col Span=""8""><Border Height=""40"" /></pf:Col>
    <pf:Col Span=""8""><Border Height=""40"" /></pf:Col>
</pf:Row>

<pf:Row Gutter=""8"">
    <pf:Col Span=""6""><Border Height=""40"" /></pf:Col>
    <pf:Col Span=""12""><Border Height=""40"" /></pf:Col>
    <pf:Col Span=""6""><Border Height=""40"" /></pf:Col>
</pf:Row>

<!-- ClipGrid — 裁剪溢出内容的 Grid -->
<pf:ClipGrid IsClipEnabled=""True"">
    <Image Source=""..."" />
</pf:ClipGrid>";
    }
}
