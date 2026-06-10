using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using PF.UI.Controls;
using PF.UI.Shared.Data;

namespace PF.UI.ViewModels.Demos
{
    public class GrowlNotificationViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Growl",         Title = "Growl",         Sub = "消息弹窗 / Success / Info / Warning / Danger / Ask" },
            new DemoTocItem { Anchor = "Notification",  Title = "Notification",  Sub = "系统通知" },
            new DemoTocItem { Anchor = "Poptip",        Title = "Poptip",        Sub = "Hover 气泡提示" },
        };

        private string _lastResult = "点击按钮触发反馈...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public ICommand GrowlSuccessCommand { get; }
        public ICommand GrowlInfoCommand { get; }
        public ICommand GrowlWarningCommand { get; }
        public ICommand GrowlDangerCommand { get; }
        public ICommand GrowlAskCommand { get; }
        public ICommand ShowNotificationCommand { get; }

        public const string GrowlToken = "demo-growl";

        public GrowlNotificationViewModel()
        {
            GrowlSuccessCommand = new DelegateCommand(() =>
            {
                Growl.Success("操作成功完成！", GrowlToken);
                LastResult = $"Growl: Success  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlInfoCommand = new DelegateCommand(() =>
            {
                Growl.Info("这是一条信息提示。", GrowlToken);
                LastResult = $"Growl: Info  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlWarningCommand = new DelegateCommand(() =>
            {
                Growl.Warning("请注意，这可能是一个问题。", GrowlToken);
                LastResult = $"Growl: Warning  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlDangerCommand = new DelegateCommand(() =>
            {
                Growl.Error("发生了一个错误！", GrowlToken);
                LastResult = $"Growl: Error  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlAskCommand = new DelegateCommand(() =>
            {
                Growl.Ask("确认要删除这条记录吗？", _ => true, GrowlToken);
                LastResult = $"Growl: Ask  ({System.DateTime.Now:HH:mm:ss})";
            });
            ShowNotificationCommand = new DelegateCommand(() =>
            {
                var content = new System.Windows.Controls.Border
                {
                    Background = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#222831")),
                    CornerRadius = new System.Windows.CornerRadius(10),
                    Padding = new System.Windows.Thickness(16, 12, 16, 12),
                    Width = 280,
                    Child = new System.Windows.Controls.StackPanel
                    {
                        Children =
                        {
                            new System.Windows.Controls.TextBlock
                            {
                                Text = "PF.UI 演示通知",
                                FontSize = 13,
                                FontWeight = System.Windows.FontWeights.Bold,
                                Foreground = System.Windows.Media.Brushes.White,
                                Margin = new System.Windows.Thickness(0, 0, 0, 4),
                            },
                            new System.Windows.Controls.TextBlock
                            {
                                Text = "这是一条系统通知，右下角浮现后自动关闭。",
                                FontSize = 11,
                                Foreground = new System.Windows.Media.SolidColorBrush(
                                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#AAAAAA")),
                                TextWrapping = System.Windows.TextWrapping.Wrap,
                            },
                        }
                    }
                };
                Notification.Show(content, ShowAnimation.HorizontalMove, false);
                LastResult = $"Notification: 已发送  ({System.DateTime.Now:HH:mm:ss})";
            });
        }

        public const string XamlGrowl = @"<!-- Growl — 消息弹窗（静态方法，代码触发） -->
<!-- 需在 Window 或容器上附加 GrowlParent 属性 -->
<!-- <pf:Window pf:Growl.GrowlParent=""True""> -->

<!-- C# 调用方式 -->
<Button Content=""成功提示""
        Command=""{Binding GrowlSuccessCommand}"" />

<Button Content=""信息提示""
        Command=""{Binding GrowlInfoCommand}"" />

<Button Content=""警告提示""
        Command=""{Binding GrowlWarningCommand}"" />

<Button Content=""错误提示""
        Command=""{Binding GrowlDangerCommand}"" />

<Button Content=""确认弹窗""
        Command=""{Binding GrowlAskCommand}"" />

<!-- Growl 方法：Success / Info / Warning / Error / Fatal / Ask -->";

        public const string XamlNotification = @"<!-- Notification — 系统通知 -->
<!-- C#: Notification.Show(content, animation, staysOpen) -->
<Button Content=""显示通知""
        Command=""{Binding ShowNotificationCommand}"" />";

        public const string XamlPoptip = @"<!-- Poptip — HitMode 触发方式 -->
<Button pf:Poptip.HitMode=""Hover"" pf:Poptip.Content=""悬停触发"" />
<Button pf:Poptip.HitMode=""Click"" pf:Poptip.Content=""点击触发"" />
<Button pf:Poptip.HitMode=""Focus"" pf:Poptip.Content=""聚焦触发"" />
<!-- 手动控制（IsOpen 双向绑定）-->
<ToggleButton x:Name=""T""
              pf:Poptip.HitMode=""None""
              pf:Poptip.IsOpen=""{Binding IsChecked, ElementName=T}""
              pf:Poptip.Content=""手动控制"" />

<!-- PlacementType — 12 个方向 -->
<Button pf:Poptip.Placement=""Top"" pf:Poptip.Content=""上方"" />
<Button pf:Poptip.Placement=""BottomLeft"" pf:Poptip.Content=""左下"" />
<Button pf:Poptip.Placement=""RightTop"" pf:Poptip.Content=""右上"" />
<!-- 可选值：TopLeft/Top/TopRight/LeftTop/Left/LeftBottom
            RightTop/Right/RightBottom/BottomLeft/Bottom/BottomRight -->

<!-- Content 为 UIElement -->
<Button pf:Poptip.HitMode=""Hover"" pf:Poptip.Placement=""Top"">
    <pf:Poptip.Content>
        <StackPanel Orientation=""Horizontal"">
            <pf:PackIcon Kind=""CheckCircle"" Foreground=""{DynamicResource SuccessBrush}"" />
            <TextBlock Text=""UIElement 内容"" />
        </StackPanel>
    </pf:Poptip.Content>
</Button>

<!-- 偏移 -->
<Button pf:Poptip.HorizontalOffset=""40"" pf:Poptip.Content=""右偏 40px"" />";
    }
}
