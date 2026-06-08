using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using PF.UI.Shared.Data;

namespace PF.UI.ViewModels.Demos
{
    public class SplashDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Basic",     Title = "基础用法",    Sub = "LoadingAction 回调，2.5 秒后自动关闭" },
            new DemoTocItem { Anchor = "Steps",     Title = "分步初始化",  Sub = "UpdateMessage 逐步更新状态文字与颜色" },
            new DemoTocItem { Anchor = "Integrate", Title = "集成说明",    Sub = "在 App.xaml.cs 中替换启动流程" },
        };

        private string _lastResult = "点击按钮打开启动屏...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public DelegateCommand OpenBasicSplashCommand => new(() =>
        {
            var splash = new PF.UI.Resources.Splash
            {
                WelcomeText       = "PF.UI 工业自动化平台",
                VersionNumber     = "v2.5.0",
                WelcomeText_small = "PF AutoFramework — Industrial Control System",
                LoadingAction     = async () =>
                {
                    await Task.Delay(2500);
                    return true;
                }
            };
            splash.ShowDialog();
            LastResult = $"基础启动屏已关闭  ({DateTime.Now:HH:mm:ss})";
        });

        public DelegateCommand OpenStepsSplashCommand => new(() =>
        {
            PF.UI.Resources.Splash? splash = null;
            splash = new PF.UI.Resources.Splash
            {
                WelcomeText       = "PF.UI 工业自动化平台",
                VersionNumber     = "v2.5.0",
                WelcomeText_small = "正在初始化系统，请稍候...",
                LoadingAction     = async () =>
                {
                    var steps = new (string Msg, MsgType Type, int DelayMs)[]
                    {
                        ("正在连接数据库...",           MsgType.Info,    600),
                        ("正在加载系统参数...",         MsgType.Info,    700),
                        ("正在初始化运动控制卡...",     MsgType.Info,    800),
                        ("正在连接 IO 设备...",         MsgType.Info,    600),
                        ("正在启动 SECS/GEM 服务...",   MsgType.Info,    700),
                        ("初始化完成",                  MsgType.Success, 600),
                    };
                    foreach (var (msg, type, delay) in steps)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            splash!.UpdateMessage(msg, type));
                        await Task.Delay(delay);
                    }
                    return true;
                }
            };
            splash.ShowDialog();
            LastResult = $"分步启动屏已关闭  ({DateTime.Now:HH:mm:ss})";
        });

        public DelegateCommand OpenFailSplashCommand => new(() =>
        {
            PF.UI.Resources.Splash? splash = null;
            splash = new PF.UI.Resources.Splash
            {
                WelcomeText       = "PF.UI 工业自动化平台",
                VersionNumber     = "v2.5.0",
                WelcomeText_small = "正在初始化系统，请稍候...",
                LoadingAction     = async () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        splash!.UpdateMessage("正在连接数据库...", MsgType.Info));
                    await Task.Delay(700);
                    Application.Current.Dispatcher.Invoke(() =>
                        splash!.UpdateMessage("连接运动控制卡失败！", MsgType.Error));
                    await Task.Delay(1200);
                    return false;
                }
            };
            var ok = splash.ShowDialog();
            LastResult = ok == true
                ? $"启动成功  ({DateTime.Now:HH:mm:ss})"
                : $"启动失败（DialogResult=false），可在此处中止启动  ({DateTime.Now:HH:mm:ss})";
        });

        public const string CsBasic = @"// 最简用法：传入 LoadingAction，完成后自动关闭
var splash = new Splash
{
    WelcomeText       = ""PF.UI 工业自动化平台"",
    VersionNumber     = ""v2.5.0"",
    WelcomeText_small = ""PF AutoFramework — Industrial Control System"",
    LoadingAction = async () =>
    {
        // 执行真实初始化（如 DI 容器构建、硬件连接等）
        await Task.Delay(2000);
        return true; // false = 初始化失败，可据此中止主窗口启动
    }
};
bool? ok = splash.ShowDialog();";

        public const string CsSteps = @"// 分步初始化：调用 UpdateMessage 更新状态文字与颜色
Splash? splash = null;
splash = new Splash
{
    WelcomeText   = ""PF.UI 工业自动化平台"",
    VersionNumber = ""v2.5.0"",
    LoadingAction = async () =>
    {
        splash!.UpdateMessage(""正在连接数据库..."",         MsgType.Info);
        await ConnectDatabaseAsync();

        splash!.UpdateMessage(""正在加载系统参数..."",       MsgType.Info);
        await LoadParametersAsync();

        splash!.UpdateMessage(""正在初始化运动控制卡..."",   MsgType.Info);
        bool ok = await InitMotionCardAsync();
        if (!ok)
        {
            splash!.UpdateMessage(""运动控制卡初始化失败！"", MsgType.Error);
            await Task.Delay(1500);
            return false; // 返回 false 令 DialogResult = false
        }

        splash!.UpdateMessage(""初始化完成"", MsgType.Success);
        await Task.Delay(500);
        return true;
    }
};
splash.ShowDialog();";

        public const string CsIntegrate = @"// App.xaml.cs — 在 OnStartup 中接入启动屏，替代默认 StartupUri
protected override async void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    Splash? splash = null;
    splash = new Splash
    {
        WelcomeText   = ""我的工业系统"",
        VersionNumber = $""v{Assembly.GetEntryAssembly()!.GetName().Version}"",
        LoadingAction = async () =>
        {
            // 1. 构建 DI 容器 / Prism 模块
            splash!.UpdateMessage(""正在加载模块..."", MsgType.Info);
            await Task.Run(BuildContainer);

            // 2. 初始化硬件
            splash!.UpdateMessage(""正在初始化硬件..."", MsgType.Info);
            bool hw = await _hardwareManager.InitAsync();
            if (!hw)
            {
                splash!.UpdateMessage(""硬件初始化失败"", MsgType.Error);
                await Task.Delay(1500);
                return false;
            }

            splash!.UpdateMessage(""就绪"", MsgType.Success);
            await Task.Delay(400);
            return true;
        }
    };

    bool? ok = splash.ShowDialog();
    if (ok != true)
    {
        // 初始化失败 → 退出程序
        Shutdown(1);
        return;
    }

    // 显示主窗口
    var mainWindow = Container.Resolve<MainWindow>();
    mainWindow.Show();
    MainWindow = mainWindow;
}

// App.xaml 中移除 StartupUri，改为在 App.xaml.cs 手动 Show()
// <Application ... >  <!-- 不设置 StartupUri -->
//     <Application.Resources> ... </Application.Resources>
// </Application>";
    }
}
