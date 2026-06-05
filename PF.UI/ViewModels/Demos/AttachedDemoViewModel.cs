using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PF.UI.ViewModels.Demos
{
    public class AttachedDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "InfoTitle", Title = "InfoElement",      Sub = "表单装饰 · 标题占位符" },
            new DemoTocItem { Anchor = "Icon",      Title = "IconElement",      Sub = "图标注入 · BorderElement" },
            new DemoTocItem { Anchor = "Window",    Title = "WindowAttach",     Sub = "窗口行为附加属性" },
            new DemoTocItem { Anchor = "Scroll",    Title = "ScrollViewerAttach", Sub = "滚动条行为" },
            new DemoTocItem { Anchor = "Reference", Title = "速查参考",         Sub = "其余 19 个附加属性" },
        };

        private string _lastResult = "与下方控件交互查看效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ─── 代码示例 ──────────────────────────────────────────────────────

        public const string XamlInfoTitle = @"<!-- InfoElement 继承自 TitleElement，为输入控件添加标题/占位符/必填标记 -->

<!-- TitlePlacement=Top（默认）：标题在控件上方 -->
<TextBox pf:InfoElement.Title=""用户名""
         pf:InfoElement.Placeholder=""请输入用户名""
         pf:InfoElement.Necessary=""True""
         pf:InfoElement.ShowClearButton=""True""
         pf:InfoElement.ContentHeight=""34"" />

<!-- TitlePlacement=Left：标题在控件左侧，TitleWidth 控制列宽 -->
<StackPanel pf:TitleElement.TitlePlacement=""Left""
            pf:TitleElement.TitleWidth=""80"">
    <TextBox pf:InfoElement.Title=""账号"" pf:InfoElement.Placeholder=""输入账号"" />
    <TextBox pf:InfoElement.Title=""密码"" pf:InfoElement.Placeholder=""输入密码"" />
    <ComboBox pf:InfoElement.Title=""角色"" />
</StackPanel>

<!-- 继承属性：父容器设置后，子控件自动继承 -->
<StackPanel pf:InfoElement.Necessary=""True""
            pf:InfoElement.ContentHeight=""36"">
    <TextBox pf:InfoElement.Title=""姓名"" />   <!-- 自动继承 Necessary=True -->
    <TextBox pf:InfoElement.Title=""工号"" />
</StackPanel>";

        public const string XamlIcon = @"<!-- IconElement：向控件注入 Path 图标（Geometry / Width / Height） -->
<Button Style=""{StaticResource ButtonPrimary}""
        pf:IconElement.Geometry=""{StaticResource SearchGeometry}""
        pf:IconElement.Width=""16""
        pf:IconElement.Height=""16""
        Content=""搜索"" />

<!-- IconElement 与 PackIcon 对比 -->
<!-- PackIcon：Material Design 内置图标（Kind 枚举） -->
<pf:PackIcon Kind=""Magnify"" Width=""20"" Height=""20"" />
<!-- IconElement：自定义 Geometry Path，嵌入任意控件模板 -->
<Button pf:IconElement.Geometry=""{StaticResource StarGeometry}"" />";

        public const string XamlBorder = @"<!-- BorderElement.CornerRadius：可继承，子树统一圆角 -->
<StackPanel pf:BorderElement.CornerRadius=""8"">
    <Button Content=""Button"" />    <!-- 自动获得 CornerRadius=8 -->
    <TextBox />
    <ComboBox />
</StackPanel>

<!-- BorderElement.Circular：自动绑定 ActualWidth/Height 变正圆 -->
<Border Width=""60"" Height=""60"" Background=""{DynamicResource PrimaryBrush}""
        pf:BorderElement.Circular=""True"">
    <TextBlock Text=""A"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""
               Foreground=""White"" FontSize=""22"" FontWeight=""Bold"" />
</Border>";

        public const string XamlWindow = @"<!-- WindowAttach：通过附加属性为窗口添加行为，无需子类化 -->

<!-- IsDragElement：任意元素拖动所在窗口 -->
<Border Background=""Transparent""
        pf:WindowAttach.IsDragElement=""True""
        Height=""32"" />

<!-- IgnoreAltF4：阻止 Alt+F4 关闭窗口（用于强制操作流程） -->
<Window pf:WindowAttach.IgnoreAltF4=""True"" />

<!-- HideWhenClosing：关闭窗口时改为隐藏，用于系统托盘场景 -->
<Window pf:WindowAttach.HideWhenClosing=""True"" />

<!-- ShowInTaskManager：控制是否在任务管理器中显示 -->
<Window pf:WindowAttach.ShowInTaskManager=""False"" />";

        public const string XamlScroll = @"<!-- ScrollViewerAttach：控制滚动条行为 -->

<!-- AutoHide=False：始终显示滚动条（默认 True=悬停才显示）-->
<ScrollViewer pf:ScrollViewerAttach.AutoHide=""False"" />

<!-- Orientation=Horizontal：鼠标滚轮触发横向滚动 -->
<ScrollViewer HorizontalScrollBarVisibility=""Auto""
              pf:ScrollViewerAttach.Orientation=""Horizontal"">
    <StackPanel Orientation=""Horizontal""><!-- 横向内容 --></StackPanel>
</ScrollViewer>

<!-- IsDisabled：阻止内层 ListBox 的滚动事件穿透到外层 ScrollViewer -->
<ScrollViewer>
    <StackPanel>
        <ListBox pf:ScrollViewerAttach.IsDisabled=""True"" />
    </StackPanel>
</ScrollViewer>

<!-- IsHoverResizingEnabled=False：禁止鼠标悬停时改变滚动条宽度 -->
<ScrollViewer pf:ScrollViewerAttach.IsHoverResizingEnabled=""False"" />";
    }
}
