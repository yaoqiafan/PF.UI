using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class AnimationDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "TransitioningContent", Title = "TransitioningContent", Sub = "内容切换过渡动画" },
            new DemoTocItem { Anchor = "AnimationPath",       Title = "AnimationPath",       Sub = "几何路径描边动画" },
            new DemoTocItem { Anchor = "RunningBlock",        Title = "RunningBlock",        Sub = "跑马灯滚动内容" },
            new DemoTocItem { Anchor = "ToggleBlock",         Title = "ToggleBlock",         Sub = "切换显示内容块" },
        };

        private string _lastResult = "与下方控件交互查看动画效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ===== TransitioningContentControl =====
        private bool _contentToggle;
        public bool ContentToggle
        {
            get => _contentToggle;
            set
            {
                if (SetProperty(ref _contentToggle, value))
                {
                    LastResult = $"过渡动画切换: {(value ? "视图 B" : "视图 A")}  ({System.DateTime.Now:HH:mm:ss})";
                    RaisePropertyChanged(nameof(ContentA));
                }
            }
        }
        public string ContentA => ContentToggle ? "这是新的视图内容  B" : "这是初始视图内容  A";
        private int _modeIndex;
        public string CurrentModeName { get; private set; } = "Right2Left";
        public ICommand NextModeCommand { get; }

        // ===== AnimationPath =====
        private bool _isPathPlaying;
        public bool IsPathPlaying
        {
            get => _isPathPlaying;
            set
            {
                if (SetProperty(ref _isPathPlaying, value))
                    LastResult = $"AnimationPath: {(value ? "播放" : "暂停")}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }
        public ICommand TogglePathCommand { get; }

        // ===== RunningBlock =====
        private bool _isBlockRunning = true;
        public bool IsBlockRunning
        {
            get => _isBlockRunning;
            set
            {
                if (SetProperty(ref _isBlockRunning, value))
                    LastResult = $"RunningBlock: {(value ? "运行" : "停止")}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }
        public ICommand ToggleBlockRunCommand { get; }

        // ===== ToggleBlock =====
        private bool? _isToggled;
        public bool? IsToggled
        {
            get => _isToggled;
            set
            {
                if (SetProperty(ref _isToggled, value))
                    LastResult = $"ToggleBlock: {(value == true ? "Checked" : "Unchecked")}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }

        public AnimationDemoViewModel()
        {
            NextModeCommand = new DelegateCommand(() => ContentToggle = !ContentToggle);
            TogglePathCommand = new DelegateCommand(() => IsPathPlaying = !IsPathPlaying);
            ToggleBlockRunCommand = new DelegateCommand(() => IsBlockRunning = !IsBlockRunning);
        }

        public const string XamlTransitioningContent = @"<!-- TransitioningContentControl — 内容切换过渡 -->
<pf:TransitioningContentControl TransitionMode=""Right2Left""
                                Content=""{Binding ContentA}"" />

<!-- 支持以下 TransitionMode： -->
<!-- Right2Left / Left2Right / Bottom2Top / Top2Bottom -->
<!-- Right2LeftWithFade / Left2RightWithFade -->
<!-- Bottom2TopWithFade / Top2BottomWithFade / Fade / Custom -->";

        public const string XamlAnimationPath = @"<!-- AnimationPath — 路径描边动画 -->
<pf:AnimationPath Data=""{StaticResource SearchGeometry}""
                  Duration=""0:0:2""
                  IsPlaying=""True""
                  Stroke=""{DynamicResource PrimaryBrush}""
                  StrokeThickness=""2""
                  RepeatBehavior=""Forever"" />";

        public const string XamlRunningBlock = @"<!-- RunningBlock — 跑马灯滚动 -->
<pf:RunningBlock IsRunning=""True"" Speed=""60""
                 Orientation=""Horizontal""
                 RunningDirection=""LeftToRight"">
    <TextBlock Text=""这是一条滚动通知消息..."" />
</pf:RunningBlock>

<pf:RunningBlock IsRunning=""True"" Speed=""40""
                 Orientation=""Vertical""
                 RunningDirection=""BottomToTop"">
    <TextBlock Text=""垂直滚动行"" />
</pf:RunningBlock>";

        public const string XamlToggleBlock = @"<!-- ToggleBlock — 切换块 -->
<pf:ToggleBlock IsChecked=""False""
                CheckedContent=""已开启""
                UnCheckedContent=""已关闭""
                IndeterminateContent=""不确定"" />";
    }
}
