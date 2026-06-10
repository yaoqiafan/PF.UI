using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using PF.UI.Controls;

namespace PF.UI.ViewModels.Demos
{
    public class ConfettiPreset
    {
        public string Name { get; init; } = string.Empty;
        public ConfettiCannon.Options Options { get; init; } = new();
    }

    public class ProgressLoadingViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "LoadingCircle",  Title = "LoadingCircle",  Sub = "圆形加载动画" },
            new DemoTocItem { Anchor = "LoadingLine",    Title = "LoadingLine",    Sub = "线性加载动画" },
            new DemoTocItem { Anchor = "ProgressBar",    Title = "ProgressBar",    Sub = "原生 + Flat 样式" },
            new DemoTocItem { Anchor = "CircleProgress", Title = "CircleProgress", Sub = "圆形百分比" },
            new DemoTocItem { Anchor = "WaveProgress",   Title = "WaveProgress",   Sub = "波浪百分比" },
            new DemoTocItem { Anchor = "Confetti",       Title = "ConfettiCannon", Sub = "彩纸抛洒" },
        };

        public List<ConfettiPreset> ConfettiPresets { get; } = new()
        {
            new ConfettiPreset
            {
                Name = "默认彩纸",
                Options = new ConfettiCannon.Options
                {
                    ParticleCount = 80, Angle = 90, Spread = 60,
                    Origin = new System.Windows.Point(0.5, 1.0),
                }
            },
            new ConfettiPreset
            {
                Name = "礼花绽放",
                Options = new ConfettiCannon.Options
                {
                    ParticleCount = 120, Angle = 90, Spread = 360,
                    StartVelocity = 35, Decay = 0.92, Gravity = 0.8,
                    Origin = new System.Windows.Point(0.5, 0.5),
                    Colors = new List<string> { "#FF5E7E", "#FFCC00", "#26CCFF", "#A25AFD", "#00E676" },
                    Shapes = new List<string> { "circle", "star" },
                }
            },
            new ConfettiPreset
            {
                Name = "左右齐射",
                Options = new ConfettiCannon.Options
                {
                    ParticleCount = 50, Spread = 55, Angle = 60,
                    Origin = new System.Windows.Point(0.0, 0.6),
                    Colors = new List<string> { "#1565C0", "#0097A7", "#00695C" },
                }
            },
            new ConfettiPreset
            {
                Name = "星形飘落",
                Options = new ConfettiCannon.Options
                {
                    ParticleCount = 60, Angle = 90, Spread = 80,
                    Gravity = 1.2, StartVelocity = 30, Scalar = 1.5,
                    Origin = new System.Windows.Point(0.5, 0.0),
                    Shapes = new List<string> { "star" },
                    Colors = new List<string> { "#FFD700", "#FFA500", "#FF8C00" },
                }
            },
            new ConfettiPreset
            {
                Name = "彩色圆点",
                Options = new ConfettiCannon.Options
                {
                    ParticleCount = 100, Angle = 90, Spread = 70,
                    Ticks = 250, Decay = 0.88,
                    Shapes = new List<string> { "circle" },
                    Colors = new List<string> { "#E91E63", "#9C27B0", "#3F51B5", "#00BCD4", "#4CAF50", "#FF9800" },
                }
            },
        };

        private int _selectedPresetIndex;
        public int SelectedPresetIndex
        {
            get => _selectedPresetIndex;
            set => SetProperty(ref _selectedPresetIndex, value);
        }

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
            {
                var preset = SelectedPresetIndex >= 0 && SelectedPresetIndex < ConfettiPresets.Count
                    ? ConfettiPresets[SelectedPresetIndex]
                    : ConfettiPresets[0];
                ConfettiCannon.Fire(preset.Options);
                LastResult = $"🎊 {preset.Name} 发射！  ({System.DateTime.Now:HH:mm:ss})";
            });
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

        public const string XamlCircleProgress = @"<!-- CircleProgressBar — 状态色 Style -->
<pf:CircleProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarSuccessCircle}"" />
<pf:CircleProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarInfoCircle}"" />
<pf:CircleProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarWarningCircle}"" />
<pf:CircleProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarDangerCircle}"" />

<!-- 自定义属性 -->
<pf:CircleProgressBar IsIndeterminate=""True"" />
<pf:CircleProgressBar Value=""{Binding V}"" ArcThickness=""12"" />
<pf:CircleProgressBar Value=""{Binding V}"" ShowText=""False"" />
<pf:CircleProgressBar Value=""{Binding V}"" Text=""{Binding V, StringFormat={}{0:F0}%}"" />";

        public const string XamlWaveProgress = @"<!-- WaveProgressBar — 状态色 Style -->
<pf:WaveProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarSuccessWave}"" />
<pf:WaveProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarWarningWave}"" />
<pf:WaveProgressBar Value=""{Binding V}"" Style=""{StaticResource ProgressBarDangerWave}"" />

<!-- 自定义属性 -->
<pf:WaveProgressBar WaveThickness=""6"" />
<pf:WaveProgressBar WaveStroke=""{DynamicResource DangerBrush}"" />
<pf:WaveProgressBar ShowText=""False"" />
<pf:WaveProgressBar>
    <pf:WaveProgressBar.WaveFill>
        <LinearGradientBrush StartPoint=""0,0"" EndPoint=""1,1"">
            <GradientStop Color=""{DynamicResource PrimaryColor}"" Offset=""0"" />
            <GradientStop Color=""{DynamicResource InfoColor}"" Offset=""1"" />
        </LinearGradientBrush>
    </pf:WaveProgressBar.WaveFill>
</pf:WaveProgressBar>";

        public const string XamlConfetti = @"<!-- ConfettiCannon — 彩纸抛洒 -->
<!-- 需包裹在 ConfettiCannonContainer 中 -->
<pf:ConfettiCannonContainer>
    <Button Content=""发射彩纸"" Command=""{Binding FireCommand}"" />
</pf:ConfettiCannonContainer>";
    }
}
