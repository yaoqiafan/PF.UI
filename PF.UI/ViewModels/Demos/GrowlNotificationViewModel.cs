using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using PF.UI.Controls;
using PF.UI.Shared.Data;

namespace PF.UI.ViewModels.Demos
{
    public class GrowlNotificationViewModel : BindableBase
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

        public GrowlNotificationViewModel()
        {
            GrowlSuccessCommand = new DelegateCommand(() =>
            {
                Growl.Success("操作成功完成！");
                LastResult = $"Growl: Success  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlInfoCommand = new DelegateCommand(() =>
            {
                Growl.Info("这是一条信息提示。");
                LastResult = $"Growl: Info  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlWarningCommand = new DelegateCommand(() =>
            {
                Growl.Warning("请注意，这可能是一个问题。");
                LastResult = $"Growl: Warning  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlDangerCommand = new DelegateCommand(() =>
            {
                Growl.Error("发生了一个错误！");
                LastResult = $"Growl: Error  ({System.DateTime.Now:HH:mm:ss})";
            });
            GrowlAskCommand = new DelegateCommand(() =>
            {
                Growl.Ask("确认要删除这条记录吗？", _ => true);
                LastResult = $"Growl: Ask  ({System.DateTime.Now:HH:mm:ss})";
            });
            ShowNotificationCommand = new DelegateCommand(() =>
            {
                Notification.Show("这是一条系统通知", ShowAnimation.HorizontalMove, false);
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

        public const string XamlPoptip = @"<!-- Poptip — 气泡提示（附加属性） -->
<Button Content=""悬停查看提示""
        pf:Poptip.HitMode=""Hover""
        pf:Poptip.Content=""这是气泡提示内容"" />";
    }
}
