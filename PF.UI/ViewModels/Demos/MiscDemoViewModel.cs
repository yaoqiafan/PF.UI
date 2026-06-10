using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PF.UI.ViewModels.Demos
{
    public class MiscDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Watermark",     Title = "Watermark",      Sub = "水印覆盖层" },
            new DemoTocItem { Anchor = "DashedBorder",  Title = "DashedBorder",   Sub = "虚线边框" },
            new DemoTocItem { Anchor = "BlendEffect",   Title = "BlendEffectBox", Sub = "混合效果" },
            new DemoTocItem { Anchor = "GotoTop",       Title = "GotoTop",        Sub = "返回顶部" },
            new DemoTocItem { Anchor = "ChatBubble",    Title = "ChatBubble",     Sub = "聊天气泡" },
            new DemoTocItem { Anchor = "Magnifier",     Title = "Magnifier",      Sub = "鼠标放大镜" },
        };

        // ─── 代码示例 ──────────────────────────────────────────────────

        public const string XamlWatermark = @"<!-- Watermark — 水印覆盖层（覆盖整个容器区域）-->
<!-- Mark 为水印文本/图片，Angle 旋转角度，MarkBrush 水印色 -->
<pf:Watermark Mark=""PF.UI DEMO"" Angle=""45""
              MarkBrush=""{DynamicResource DangerBrush}""
              MarkMargin=""10"">
    <Border Background=""{DynamicResource RegionBrush}"" Width=""300"" Height=""200"">
        <TextBlock Text=""被水印覆盖的内容区"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"" />
    </Border>
</pf:Watermark>

<!-- 纯图片水印 -->
<pf:Watermark>
    <pf:Watermark.Content>
        <pf:PackIcon Kind=""LockOutline"" Width=""80"" Height=""80"" Opacity=""0.15"" />
    </pf:Watermark.Content>
    <!-- 水印包装的内容 -->
    <Border Width=""300"" Height=""200"" Background=""{DynamicResource RegionBrush}"" />
</pf:Watermark>";

        public const string XamlDashedBorder = @"<!-- DashedBorder — 虚线边框 Decorator -->
<pf:DashedBorder BorderBrush=""{DynamicResource PrimaryBrush}""
                 BorderThickness=""2""
                 BorderDashArray=""4,2""
                 CornerRadius=""8""
                 Padding=""16,8"">
    <TextBlock Text=""虚线边框内容"" />
</pf:DashedBorder>

<!-- 不同虚线样式 -->
<pf:DashedBorder BorderDashArray=""8,4"" CornerRadius=""12""   BorderDashCap=""Round"">
    <TextBlock Text=""圆头虚线 8,4"" />
</pf:DashedBorder>
<pf:DashedBorder BorderDashArray=""2,4"" CornerRadius=""4""    BorderDashCap=""Square"">
    <TextBlock Text=""方头虚线 2,4"" />
</pf:DashedBorder>
<!-- 实线（等效 Border） -->
<pf:DashedBorder BorderDashArray=""1,0"">
    <TextBlock Text=""BorderDashArray=1,0 等效实线"" />
</pf:DashedBorder>";

        public const string XamlBlendEffect = @"<!-- BlendEffectBox — 多层 WPF Effect 叠加容器 -->
<!-- Effects 集合从外到内依次叠加，Content 是底层原始内容 -->
<pf:BlendEffectBox>
    <pf:BlendEffectBox.Effects>
        <DropShadowEffect Color=""#1565C0"" BlurRadius=""20"" ShadowDepth=""0"" Opacity=""0.6"" />
        <BlurEffect Radius=""0"" />
    </pf:BlendEffectBox.Effects>
    <pf:BlendEffectBox.Content>
        <Border Width=""200"" Height=""80"" CornerRadius=""10""
                Background=""{DynamicResource PrimaryBrush}"">
            <TextBlock Text=""BlendEffectBox 内容""
                       Foreground=""White"" FontWeight=""Bold""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center"" />
        </Border>
    </pf:BlendEffectBox.Content>
</pf:BlendEffectBox>";

        public const string XamlMagnifier = @"<!-- Magnifier — 鼠标放大镜（AdornerLayer 叠层） -->
<!-- Target 绑定要被放大的目标元素，Scale 为放大倍数（默认 5.0）-->
<Grid>
    <Border x:Name=""MagnifyTarget"" ... />
    <pf:Magnifier Target=""{Binding ElementName=MagnifyTarget}""
                  Scale=""3"" />
</Grid>

<!-- 也可通过附加属性一行绑定 -->
<Border pf:AdornerElement.Instance=""{StaticResource MyMagnifier}"" ... />";

        public const string XamlGotoTop = @"<!-- GotoTop — 返回顶部按钮 -->
<!-- 用 SimplePanel 叠放 ScrollViewer + GotoTop -->
<!-- Target 直接绑定 ScrollViewer 自身 -->
<pf:SimplePanel Height=""300"">
    <ScrollViewer x:Name=""MyScroll""
                  IsInertiaEnabled=""True""
                  VerticalScrollBarVisibility=""Auto"">
        <Border Height=""1200"" /> <!-- 滚动内容 -->
    </ScrollViewer>
    <pf:GotoTop Target=""{Binding ElementName=MyScroll}""
                AutoHiding=""True""
                Animated=""True""
                AnimationTime=""400""
                HidingHeight=""60""
                HorizontalAlignment=""Right""
                VerticalAlignment=""Bottom""
                Margin=""0,0,16,16"" />
</pf:SimplePanel>";

        public const string XamlChatBubble = @"<!-- ChatBubble — 聊天气泡 -->
<!-- Role: Sender（发送者/右）| Receiver（接收者/左）-->
<!-- Type: String | Image | Audio | Custom -->
<!-- IsRead: 已读/未读标记 -->
<pf:ChatBubble Role=""Receiver"" Type=""String"" IsRead=""True"" Content=""这是接收到的消息"" />
<pf:ChatBubble Role=""Sender""   Type=""String"" Content=""这是发送的消息"" />
<pf:ChatBubble Role=""Receiver"" Type=""Image""  Content=""图片消息类型"" />
<pf:ChatBubble Role=""Sender""   Type=""Audio""  Content=""语音消息"" />";
    }
}
