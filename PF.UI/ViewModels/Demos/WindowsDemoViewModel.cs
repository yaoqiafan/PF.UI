using PF.UI.Controls;
using PF.UI.Shared.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PF.UI.ViewModels.Demos
{
    public class WindowsDemoViewModel : BindableBase
    {
        private readonly Screenshot _screenshot = new();

        public WindowsDemoViewModel()
        {
            Screenshot.Snapped += OnSnapped;
        }

        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "PfWindow", Title = "pf:Window",     Sub = "自定义标题栏基础窗口" },
            new DemoTocItem { Anchor = "Effects",  Title = "特效窗口",      Sub = "BlurWindow · GlowWindow" },
            new DemoTocItem { Anchor = "Popup",    Title = "PopupWindow",   Sub = "静态弹窗 API" },
            new DemoTocItem { Anchor = "Tray",     Title = "NotifyIcon",    Sub = "系统托盘 + 截图" },
        };

        private string _lastResult = "点击按钮打开演示窗口...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        private bool _isBlinking;
        public bool IsBlinking
        {
            get => _isBlinking;
            set => SetProperty(ref _isBlinking, value);
        }

        // ─── pf:Window ────────────────────────────────────────────────────

        public DelegateCommand OpenPfWindowCommand => new(() =>
        {
            var content = BuildContent(
                "pf:Window — 自定义标题栏",
                "· 非客户区完全自定义：标题、图标、最小化/最大化/关闭按钮\n" +
                "· NonClientAreaContent 可向标题栏注入任意 UI 元素\n" +
                "· Loaded 时自动播放 CubicEase 弹出动画（Opacity + ScaleTransform）\n" +
                "· IsFullScreen 全屏切换不改变 WindowState 语义\n" +
                "· ShowNonClientArea=False 可隐藏整个标题栏（纯内容窗口）");
            var win = new PF.UI.Controls.Window
            {
                Title = "pf:Window 演示",
                Width = 520, Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = content
            };
            win.Show();
            LastResult = $"pf:Window 已打开  ({DateTime.Now:HH:mm:ss})";
        });

        // ─── BlurWindow ───────────────────────────────────────────────────

        public DelegateCommand OpenBlurWindowCommand => new(() =>
        {
            var content = BuildContent(
                "BlurWindow — 亚克力磨玻璃",
                "· Win10 1809+ → ACCENT_ENABLE_ACRYLICBLURBEHIND（亚克力）\n" +
                "· Win10 1903+ → 同上但加了 HwndSourceHook 优化\n" +
                "· 移动/缩放窗口时（WM_ENTERSIZEMOVE）自动切为普通模糊\n" +
                "· 需将 Background 设置为半透明才能透出磨玻璃效果\n" +
                "· 继承 pf:Window 所有特性（标题栏、动画等）");
            var win = new BlurWindow
            {
                Title = "BlurWindow — 亚克力磨玻璃",
                Background = new SolidColorBrush(Color.FromArgb(0x28, 0x10, 0x10, 0x10)),
                Width = 520, Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = content
            };
            win.Show();
            LastResult = $"BlurWindow 已打开  ({DateTime.Now:HH:mm:ss})";
        });

        // ─── GlowWindow ───────────────────────────────────────────────────

        public DelegateCommand OpenGlowWindowCommand => new(() =>
        {
            var content = BuildContent(
                "GlowWindow — 彩色外发光边框",
                "· ActiveGlowColor：窗口聚焦时四边发光颜色\n" +
                "· InactiveGlowColor：失焦时的发光颜色（通常为灰色）\n" +
                "· 发光通过 4 个独立的 GlowEdge 辅助窗口实现，不影响主窗口渲染\n" +
                "· 继承 pf:Window 所有特性");
            var win = new GlowWindow
            {
                Title = "GlowWindow — 外发光边框",
                ActiveGlowColor   = Color.FromRgb(33, 150, 243),
                InactiveGlowColor = Color.FromRgb(90, 90, 90),
                Width = 520, Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = content
            };
            win.Show();
            LastResult = $"GlowWindow 已打开（蓝色激活发光）  ({DateTime.Now:HH:mm:ss})";
        });

        // ─── PopupWindow ──────────────────────────────────────────────────

        public DelegateCommand ShowPopupMsgCommand => new(() =>
        {
            PopupWindow.Show("操作已完成，配方参数已成功写入设备。");
            LastResult = $"PopupWindow.Show(msg) 已调用  ({DateTime.Now:HH:mm:ss})";
        });

        public DelegateCommand ShowPopupDialogCommand => new(() =>
        {
            var result = PopupWindow.ShowDialog(
                "此操作将清空所有运行日志，确认继续？",
                "清空确认",
                showCancel: true);
            LastResult = result == true
                ? $"PopupWindow.ShowDialog → 确定  ({DateTime.Now:HH:mm:ss})"
                : $"PopupWindow.ShowDialog → 取消  ({DateTime.Now:HH:mm:ss})";
        });

        // ─── Screenshot ───────────────────────────────────────────────────

        public DelegateCommand StartScreenshotCommand => new(() =>
        {
            _screenshot.Start();
            LastResult = $"ScreenshotWindow 已打开，框选区域后释放鼠标...";
        });

        private void OnSnapped(object sender, FunctionEventArgs<ImageSource> e)
        {
            Application.Current.Dispatcher.Invoke(() =>
                LastResult = $"截图完成：{(e.Info as BitmapSource)?.PixelWidth ?? 0} × {(e.Info as BitmapSource)?.PixelHeight ?? 0} px  ({DateTime.Now:HH:mm:ss})");
        }

        // ─── 工具方法 ──────────────────────────────────────────────────────

        private static UIElement BuildContent(string title, string desc)
        {
            var panel = new StackPanel { Margin = new Thickness(24, 20, 24, 20) };

            var titleBlock = new TextBlock
            {
                Text = title, FontSize = 15, FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 12)
            };
            titleBlock.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryTextBrush");

            var descBlock = new TextBlock
            {
                Text = desc, FontSize = 12, TextWrapping = TextWrapping.Wrap, LineHeight = 22
            };
            descBlock.SetResourceReference(TextBlock.ForegroundProperty, "SecondaryTextBrush");

            panel.Children.Add(titleBlock);
            panel.Children.Add(descBlock);
            return panel;
        }

        // ─── 代码示例 ──────────────────────────────────────────────────────

        public const string XamlPfWindow = @"<!-- XAML：继承 pf:Window，获得自定义标题栏 + 动画 -->
<pf:Window x:Class=""MyApp.Views.MainWindow""
           xmlns:pf=""clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls""
           Title=""我的窗口"" Width=""800"" Height=""600"">

    <!-- 向标题栏右侧注入自定义内容（如时钟、用户头像） -->
    <pf:Window.NonClientAreaContent>
        <StackPanel Orientation=""Horizontal"">
            <TextBlock Text=""v1.0.0"" FontSize=""11"" VerticalAlignment=""Center"" />
        </StackPanel>
    </pf:Window.NonClientAreaContent>

    <!-- 页面内容 -->
    <Grid />
</pf:Window>

<!-- 关键属性
     NonClientAreaHeight  — 标题栏高度（默认 32）
     ShowNonClientArea    — 是否显示标题栏（False = 纯内容窗口）
     ShowTitle / ShowIcon — 标题文字 / 图标是否显示
     IsFullScreen         — 切换全屏（保存/还原 WindowState）
     UIIcon               — 独立 UI 图标（与 Icon 属性区分）-->";

        public const string XamlBlurGlow = @"<!-- BlurWindow：亚克力磨玻璃效果窗口 -->
<pf:BlurWindow x:Class=""MyApp.Views.BlurDemoWindow""
               xmlns:pf=""clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls""
               Title=""磨玻璃窗口""
               Background=""#28101010"">  <!-- 半透明背景才能透出效果 -->
</pf:BlurWindow>

<!-- 版本适配（自动检测，无需手动处理）
     Win10 1809+ → ACCENT_ENABLE_ACRYLICBLURBEHIND（亚克力）
     Win10 1903+ → 同上 + WM_ENTERSIZEMOVE 拖动时降级优化
     Win7/8      → ACCENT_ENABLE_TRANSPARENTGRADIENT（半透明渐变）-->

<!-- GlowWindow：彩色外发光边框 -->
<pf:GlowWindow x:Class=""MyApp.Views.GlowDemoWindow""
               xmlns:pf=""clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls""
               Title=""发光窗口""
               ActiveGlowColor=""#2196F3""
               InactiveGlowColor=""#505050"">
</pf:GlowWindow>

<!-- C# 代码打开 -->
var win = new GlowWindow
{
    ActiveGlowColor   = Color.FromRgb(33, 150, 243),  // 聚焦时蓝色发光
    InactiveGlowColor = Color.FromRgb(90, 90, 90),    // 失焦时灰色
};
win.Show();";

        public const string XamlPopup = @"<!-- PopupWindow 静态 API（无需实例化）-->

// 1. 纯消息弹窗（非模态，不阻塞调用线程）
PopupWindow.Show(""操作已完成，数据已写入设备。"");

// 2. 模态确认弹窗（返回 bool?：true=确定 false=取消 null=关闭）
bool? result = PopupWindow.ShowDialog(
    ""此操作不可撤销，确认继续？"",
    title: ""操作确认"",
    showCancel: true);

// 3. 锚定到 UI 元素旁边（弹出层，无标题栏/背景）
var popup = new PopupWindow { PopupElement = myUserControl };
popup.Show(anchorElement, showBackground: false);

// 4. 指定坐标打开
popup.Show(ownerWindow, new Point(100, 200));

<!-- 关键属性
     ContentStr  — 字符串消息内容
     ShowTitle   — 是否显示标题栏（默认 True）
     ShowCancel  — 是否显示取消按钮
     ShowBorder  — 是否显示边框（模态模式默认 True）
     PopupElement — 自定义 FrameworkElement 内容 -->";

        public const string XamlTray = @"<!-- NotifyIcon：系统托盘图标，放入 Window/UserControl 视觉树即自动注册 -->
<pf:NotifyIcon Token=""AppTray""
               Text=""PF.UI 应用（右键查看菜单）""
               Icon=""{StaticResource TrayDrawingImage}""
               IsBlink=""{Binding IsAlarm}"">
    <!-- 右键菜单内容（任意 UIElement）-->
    <pf:NotifyIcon.ContextContent>
        <StackPanel Margin=""8"">
            <Button Content=""前置主窗口""
                    Command=""{x:Static pf:ControlCommands.PushMainWindow2Top}"" />
            <Separator />
            <Button Content=""退出程序""
                    Command=""{x:Static pf:ControlCommands.ShutdownApp}"" />
        </StackPanel>
    </pf:NotifyIcon.ContextContent>
</pf:NotifyIcon>

<!-- Screenshot：框选截图 -->
// 启动截图（打开全屏透明 ScreenshotWindow，框选后触发 Snapped 事件）
var ss = new Screenshot();
Screenshot.Snapped += (s, e) =>
{
    // e.Info 是 ImageSource（BitmapSource），可保存或展示
    var bmp = e.Info as BitmapSource;
};
ss.Start();

// 或通过内置命令触发（需在 CommandBindings 中注册）
Command=""{x:Static pf:ControlCommands.StartScreenshot}""";
    }
}
