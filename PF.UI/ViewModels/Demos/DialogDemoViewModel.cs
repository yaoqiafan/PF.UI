using PF.UI.Controls;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PF.UI.ViewModels.Demos
{
    public class DialogDemoViewModel : BindableBase
    {
        private const string Token = "DemoDialog";
        private readonly IDialogService _dialogService;

        public DialogDemoViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Overlay", Title = "DialogOverlay", Sub = "页内遮罩对话框" },
            new DemoTocItem { Anchor = "Prism",   Title = "IDialogService", Sub = "Prism 独立窗口对话框" },
        };

        private string _lastResult = "（尚未操作）";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ─── DialogOverlay 命令 ────────────────────────────────────────────

        public DelegateCommand ShowInfoDialogCommand => new(async () =>
        {
            var frame = BuildFrame("操作提示", PackIconKind.InformationOutline,
                new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                "当前操作已成功完成，所有数据已保存到数据库。", okOnly: true);
            await DialogOverlay.Show(frame, Token);
            LastResult = "信息对话框 → 确定";
        });

        public DelegateCommand ShowConfirmDialogCommand => new(async () =>
        {
            var frame = BuildFrame("确认操作", PackIconKind.AlertOutline,
                new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                "此操作将重置所有参数至出厂默认值，确认继续？", okOnly: false);
            var result = await DialogOverlay.Show(frame, Token);
            LastResult = $"确认对话框 → {(result is true ? "确定" : "取消")}";
        });

        public DelegateCommand ShowSuccessDialogCommand => new(async () =>
        {
            var frame = BuildFrame("保存成功", PackIconKind.CheckCircleOutline,
                new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                "配方参数已成功写入，下次启动将自动加载。", okOnly: true);
            await DialogOverlay.Show(frame, Token);
            LastResult = "成功对话框 → 确定";
        });

        public DelegateCommand ShowErrorDialogCommand => new(async () =>
        {
            var frame = BuildFrame("运行错误", PackIconKind.AlertCircleOutline,
                new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                "轴 X1 初始化失败：超时未收到回零完成信号。\n请检查驱动器电源与通讯连接后重试。", okOnly: true);
            await DialogOverlay.Show(frame, Token);
            LastResult = "错误对话框 → 关闭";
        });

        public DelegateCommand ShowWaitDialogCommand => new(async () =>
        {
            var loading = new System.Windows.Controls.StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var line = new LoadingLine { IsRunning = true, Width = 200, Height = 4 };
            var txt = new System.Windows.Controls.TextBlock
            {
                Text = "正在初始化各轴，请稍候...",
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 14, 0, 0)
            };
            txt.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, "SecondaryTextBrush");
            loading.Children.Add(line);
            loading.Children.Add(txt);

            var frame = new DialogFrame
            {
                Header         = "正在初始化",
                IconKind       = PackIconKind.TimerSandEmpty,
                IconBackground = new SolidColorBrush(Color.FromRgb(2, 173, 139)),
                ShowFooter     = false,
                Padding        = new Thickness(20, 20, 20, 28),
                MinWidth       = 340,
                Content        = loading
            };
            var task = DialogOverlay.Show(frame, Token);
            await Task.Delay(3000);
            DialogOverlay.Close("done", Token);
            await task;
            LastResult = "等待对话框 → 自动关闭（3秒）";
        });

        public DelegateCommand ShowClickAwayDialogCommand => new(async () =>
        {
            var frame = BuildFrame("点击遮罩关闭", PackIconKind.CursorDefaultClickOutline,
                new SolidColorBrush(Color.FromRgb(103, 58, 183)),
                "点击对话框外的遮罩区域即可关闭此对话框。", okOnly: true);
            await DialogOverlay.Show(frame, Token);
            LastResult = "点击遮罩关闭演示 → 关闭";
        });

        // ─── Prism IDialogService 命令 ─────────────────────────────────────

        public DelegateCommand ShowPrismConfirmCommand => new(() =>
        {
            _dialogService.ShowDialog("ConfirmDialog",
                new DialogParameters
                {
                    { "title",   "确认操作" },
                    { "message", "此操作将重置所有参数至出厂默认值，确认继续？" },
                    { "icon",    PackIconKind.AlertOutline },
                    { "color",   "#FF9800" }
                },
                r => LastResult = r.Result == ButtonResult.OK
                    ? "Prism 确认 → 确定" : "Prism 确认 → 取消");
        });

        public DelegateCommand ShowPrismCustomCommand => new(() =>
        {
            _dialogService.ShowDialog("ConfirmDialog",
                new DialogParameters
                {
                    { "title",   "删除确认" },
                    { "message", "即将永久删除选中的 5 条日志记录，\n此操作不可恢复，确认删除？" },
                    { "icon",    PackIconKind.TrashCanOutline },
                    { "color",   "#F44336" }
                },
                r => LastResult = r.Result == ButtonResult.OK
                    ? "Prism 删除 → 确定" : "Prism 删除 → 取消");
        });

        // ─── 工具方法 ──────────────────────────────────────────────────────

        private static DialogFrame BuildFrame(string header, PackIconKind icon,
            SolidColorBrush iconBg, string message, bool okOnly)
        {
            var msgBlock = new System.Windows.Controls.TextBlock
            {
                Text        = message,
                FontSize    = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight  = 22,
                MaxWidth    = 360
            };
            msgBlock.SetResourceReference(
                System.Windows.Controls.TextBlock.ForegroundProperty, "PrimaryTextBrush");

            FrameworkElement footer;
            if (okOnly)
            {
                var btn = new System.Windows.Controls.Button
                {
                    Content = "确  定", Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand, CommandParameter = true
                };
                btn.SetResourceReference(System.Windows.Controls.Control.StyleProperty, "ButtonPrimary");
                footer = btn;
            }
            else
            {
                var panel = new System.Windows.Controls.StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal
                };
                var cancelBtn = new System.Windows.Controls.Button
                {
                    Content = "取  消", Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand, CommandParameter = false,
                    Margin  = new Thickness(0, 0, 10, 0)
                };
                cancelBtn.SetResourceReference(
                    System.Windows.Controls.Control.StyleProperty, "ButtonDefault");
                var okBtn = new System.Windows.Controls.Button
                {
                    Content = "确  定", Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand, CommandParameter = true
                };
                okBtn.SetResourceReference(
                    System.Windows.Controls.Control.StyleProperty, "ButtonPrimary");
                panel.Children.Add(cancelBtn);
                panel.Children.Add(okBtn);
                footer = panel;
            }

            return new DialogFrame
            {
                Header        = header,
                IconKind      = icon,
                IconBackground = iconBg,
                MinWidth      = 360,
                Padding       = new Thickness(20, 16, 20, 16),
                Content       = msgBlock,
                FooterContent = footer
            };
        }

        // ─── 代码示例常量 ──────────────────────────────────────────────────

        public const string XamlOverlayDecl = @"<!-- 1. 在 View 根部包裹 DialogOverlay，Token 作为作用域 ID -->
<pf:DialogOverlay Token=""DemoDialog"" CloseOnClickAway=""True"">
    <Grid><!-- 正常页面内容 --></Grid>
</pf:DialogOverlay>

<!-- 2. DialogFrame：Header / Icon / Content / FooterContent 四区布局 -->
<pf:DialogFrame Header=""确认操作"" IconKind=""AlertOutline""
                IconBackground=""#FF9800"" MinWidth=""360"">
    <pf:DialogFrame.Content>
        <TextBlock Text=""确认继续此操作？"" />
    </pf:DialogFrame.Content>
    <pf:DialogFrame.FooterContent>
        <!-- CloseDialogCommand 路由命令，CommandParameter 即为 Show() 的返回值 -->
        <Button Command=""{x:Static pf:DialogOverlay.CloseDialogCommand}""
                CommandParameter=""True"" Content=""确  定""
                Style=""{StaticResource ButtonPrimary}"" />
    </pf:DialogFrame.FooterContent>
</pf:DialogFrame>";

        public const string XamlOverlayCs = @"// 打开对话框，await 等待用户交互
var frame = BuildFrame(/* header, icon, message, okOnly */);
var result = await DialogOverlay.Show(frame, ""DemoDialog"");
// result: true=确定  false=取消  null=ClickAway / 其它

// 自动关闭（等待弹窗）
var task = DialogOverlay.Show(frame, ""DemoDialog"");
await Task.Delay(3000);
DialogOverlay.Close(""done"", ""DemoDialog"");
await task;

// Token 规则
// · Token 与 XAML 中 DialogOverlay.Token 保持一致
// · 多个 DialogOverlay 共存时 Token 用于精确定位目标";

        public const string XamlPrismReg = @"// App.xaml.cs — RegisterTypes
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    // 注册 IDialogWindow 实现（继承 pf:Window，自带标题栏 + CubicEase 弹出动画）
    containerRegistry.RegisterDialogWindow<PFDialogWindow>();

    // 注册对话框 View + ViewModel
    // ""ConfirmDialog"" 即 ShowDialog() 的第一个参数（名称）
    containerRegistry.RegisterDialog<ConfirmDialogView, ConfirmDialogViewModel>(""ConfirmDialog"");
}";

        public const string XamlPrismCall = @"// ViewModel 通过构造函数注入 IDialogService
public class MyViewModel
{
    private readonly IDialogService _dialogService;
    public MyViewModel(IDialogService dialogService) =>
        _dialogService = dialogService;

    public void OpenConfirm()
    {
        _dialogService.ShowDialog(""ConfirmDialog"",
            new DialogParameters
            {
                { ""title"",   ""确认操作"" },
                { ""message"", ""此操作不可撤销，确认继续？"" },
                { ""icon"",    PackIconKind.AlertOutline },
                { ""color"",   ""#FF9800"" }
            },
            result =>
            {
                // 对话框关闭后在 UI 线程回调
                if (result.Result == ButtonResult.OK)
                    LastResult = ""用户点击确定"";
                else
                    LastResult = ""用户取消"";
            });
    }
}";

        public const string XamlPrismVm = @"// ConfirmDialogViewModel — 实现 Prism 9 IDialogAware
public class ConfirmDialogViewModel : BindableBase, IDialogAware
{
    // Prism 9：RequestClose 是 DialogCloseListener 属性（非 event）
    public DialogCloseListener RequestClose { get; } = new();

    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        // 从 ShowDialog 传入的 DialogParameters 读取初始值
        if (parameters.TryGetValue(""title"",   out string t)) DialogTitle = t;
        if (parameters.TryGetValue(""message"", out string m)) Message     = m;
        if (parameters.TryGetValue(""icon"",    out PackIconKind i)) IconKind = i;
    }

    // 调用 RequestClose.Invoke 通知 Prism 对话框操作已完成
    public DelegateCommand OkCommand =>
        new(() => RequestClose.Invoke(ButtonResult.OK));
    public DelegateCommand CancelCommand =>
        new(() => RequestClose.Invoke(ButtonResult.Cancel));
}";
    }
}
