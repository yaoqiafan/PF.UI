using PF.UI.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Media;

namespace PF.UI.ViewModels.Demos
{
    public class DialogDemoViewModel : BindableBase
    {
        private const string Token = "DemoDialog";

        private string _lastResult = "（尚未操作）";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ── 信息对话框 ───────────────────────────────────────────────
        public DelegateCommand ShowInfoDialogCommand => new(async () =>
        {
            var frame = BuildFrame("操作提示", PackIconKind.InformationOutline,
                new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                "当前操作已成功完成，所有数据已保存到数据库。",
                okOnly: true);

            await DialogOverlay.Show(frame, Token);
            LastResult = "信息对话框 → 确定";
        });

        // ── 确认对话框 ───────────────────────────────────────────────
        public DelegateCommand ShowConfirmDialogCommand => new(async () =>
        {
            var frame = BuildFrame("确认操作", PackIconKind.AlertOutline,
                new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                "此操作将重置所有参数至出厂默认值，确认继续？",
                okOnly: false);

            var result = await DialogOverlay.Show(frame, Token);
            LastResult = $"确认对话框 → {(result is true ? "确定" : "取消")}";
        });

        // ── 错误对话框 ───────────────────────────────────────────────
        public DelegateCommand ShowErrorDialogCommand => new(async () =>
        {
            var frame = BuildFrame("运行错误", PackIconKind.AlertCircleOutline,
                new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                "轴 X1 初始化失败：超时未收到回零完成信号。\n请检查驱动器电源与通讯连接后重试。",
                okOnly: true);

            await DialogOverlay.Show(frame, Token);
            LastResult = "错误对话框 → 关闭";
        });

        // ── 成功对话框 ───────────────────────────────────────────────
        public DelegateCommand ShowSuccessDialogCommand => new(async () =>
        {
            var frame = BuildFrame("保存成功", PackIconKind.CheckCircleOutline,
                new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                "配方参数已成功写入，下次启动将自动加载。",
                okOnly: true);

            await DialogOverlay.Show(frame, Token);
            LastResult = "成功对话框 → 确定";
        });

        // ── 等待对话框（自动关闭） ────────────────────────────────────
        public DelegateCommand ShowWaitDialogCommand => new(async () =>
        {
            var loading = new System.Windows.Controls.StackPanel { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            var line = new PF.UI.Controls.LoadingLine { IsRunning = true, Width = 200, Height = 4 };
            var txt = new System.Windows.Controls.TextBlock
            {
                Text = "正在初始化各轴，请稍候...",
                FontSize = 13,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new System.Windows.Thickness(0, 14, 0, 0)
            };
            txt.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, "SecondaryTextBrush");
            loading.Children.Add(line);
            loading.Children.Add(txt);

            var frame = new DialogFrame
            {
                Header = "正在初始化",
                IconKind = PackIconKind.TimerSandEmpty,
                IconBackground = new SolidColorBrush(Color.FromRgb(2, 173, 139)),
                ShowFooter = false,
                Padding = new System.Windows.Thickness(20, 20, 20, 28),
                MinWidth = 340,
                Content = loading
            };

            // 模拟 3 秒后自动关闭
            var task = DialogOverlay.Show(frame, Token);
            await Task.Delay(3000);
            DialogOverlay.Close("done", Token);
            await task;
            LastResult = "等待对话框 → 自动关闭（3秒）";
        });

        // ── 点击遮罩关闭 ─────────────────────────────────────────────
        public DelegateCommand ShowClickAwayDialogCommand => new(async () =>
        {
            var frame = BuildFrame("点击遮罩关闭", PackIconKind.CursorDefaultClickOutline,
                new SolidColorBrush(Color.FromRgb(103, 58, 183)),
                "点击对话框外的遮罩区域即可关闭此对话框。",
                okOnly: true);

            // 通过命令关闭，不影响 CloseOnClickAway 演示
            await DialogOverlay.Show(frame, Token);
            LastResult = "点击遮罩关闭演示 → 关闭";
        });

        // ── 工具方法 ────────────────────────────────────────────────
        private static DialogFrame BuildFrame(string header, PackIconKind icon,
            SolidColorBrush iconBg, string message, bool okOnly)
        {
            var msgBlock = new System.Windows.Controls.TextBlock
            {
                Text = message,
                FontSize = 13,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                LineHeight = 22,
                MaxWidth = 360
            };
            msgBlock.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, "PrimaryTextBrush");

            FrameworkElement footer;
            if (okOnly)
            {
                var btn = new System.Windows.Controls.Button
                {
                    Content = "确  定",
                    Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand,
                    CommandParameter = true
                };
                btn.SetResourceReference(System.Windows.Controls.Control.StyleProperty, "ButtonPrimary");
                footer = btn;
            }
            else
            {
                var panel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
                var cancelBtn = new System.Windows.Controls.Button
                {
                    Content = "取  消", Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand,
                    CommandParameter = false,
                    Margin = new System.Windows.Thickness(0, 0, 10, 0)
                };
                cancelBtn.SetResourceReference(System.Windows.Controls.Control.StyleProperty, "ButtonDefault");
                var okBtn = new System.Windows.Controls.Button
                {
                    Content = "确  定", Width = 88, Height = 34,
                    Command = DialogOverlay.CloseDialogCommand,
                    CommandParameter = true
                };
                okBtn.SetResourceReference(System.Windows.Controls.Control.StyleProperty, "ButtonPrimary");
                panel.Children.Add(cancelBtn);
                panel.Children.Add(okBtn);
                footer = panel;
            }

            return new DialogFrame
            {
                Header = header,
                IconKind = icon,
                IconBackground = iconBg,
                MinWidth = 360,
                Padding = new System.Windows.Thickness(20, 16, 20, 16),
                Content = msgBlock,
                FooterContent = footer
            };
        }
    }
}
