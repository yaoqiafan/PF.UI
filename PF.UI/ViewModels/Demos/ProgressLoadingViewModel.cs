using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ProgressLoadingViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "LoadingCircle", Title = "LoadingCircle", Sub = "圆形加载动画" },
            new DemoTocItem { Anchor = "LoadingLine",   Title = "LoadingLine",   Sub = "线性加载动画" },
            new DemoTocItem { Anchor = "ProgressBar",   Title = "ProgressBar",   Sub = "原生 + Flat 样式" },
            new DemoTocItem { Anchor = "CircleProgress", Title = "CircleProgress", Sub = "圆形百分比" },
            new DemoTocItem { Anchor = "WaveProgress",  Title = "WaveProgress",  Sub = "波浪百分比" },
            new DemoTocItem { Anchor = "Confetti",      Title = "ConfettiCannon", Sub = "彩纸抛洒" },
        };

        private string _lastResult = "";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        private bool _isRunning = true;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (SetProperty(ref _isRunning, value))
                    LastResult = $"Loading: {(value ? "运行" : "停止")}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }

        private double _progressValue = 65;
        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public ICommand ToggleRunningCommand { get; }
        public ICommand FireCommand { get; }

        public ProgressLoadingViewModel()
        {
            ToggleRunningCommand = new DelegateCommand(() => IsRunning = !IsRunning);
            FireCommand = new DelegateCommand(() =>
                LastResult = $"🎊 彩纸发射  ({System.DateTime.Now:HH:mm:ss})");
        }

        public const string XamlLoadingCircle = @"<!-- LoadingCircle — 圆形加载 -->
<pf:LoadingCircle IsRunning=""{Binding IsRunning}"" />

<!-- 颜色变体 -->
<pf:LoadingCircle IsRunning=""True"" Foreground=""{DynamicResource SuccessBrush}"" />
<pf:LoadingCircle IsRunning=""True"" Foreground=""{DynamicResource DangerBrush}"" />";

        public const string XamlLoadingLine = @"<!-- LoadingLine — 线性加载 -->
<pf:LoadingLine IsRunning=""{Binding IsRunning}"" />";

        public const string XamlProgressBar = @"<!-- ProgressBar — 原生进度条 -->
<ProgressBar Value=""{Binding ProgressValue}"" Height=""8"" />

<!-- Flat 样式（无 3D 效果） -->
<ProgressBar Style=""{StaticResource ProgressBarFlat}""
             Value=""{Binding ProgressValue}"" Height=""8"" />";

        public const string XamlCircleProgress = @"<!-- CircleProgressBar — 圆形百分比进度 -->
<pf:CircleProgressBar Value=""{Binding ProgressValue}"" />";

        public const string XamlWaveProgress = @"<!-- WaveProgressBar — 波浪百分比进度 -->
<pf:WaveProgressBar Value=""{Binding ProgressValue}"" />";

        public const string XamlConfetti = @"<!-- ConfettiCannon — 彩纸抛洒 -->
<!-- 需包裹在 ConfettiCannonContainer 中 -->
<pf:ConfettiCannonContainer>
    <Button Content=""发射彩纸"" Command=""{Binding FireCommand}"" />
</pf:ConfettiCannonContainer>";
    }
}
