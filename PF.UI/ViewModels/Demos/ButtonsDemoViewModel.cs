using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ButtonsDemoViewModel : BindableBase
    {
        // ===== TOC 目录 =====
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "基础按钮", Title = "基础按钮样式", Sub = "实心 / 小号 / 虚线" },
            new DemoTocItem { Anchor = "图标按钮", Title = "图标按钮", Sub = "ButtonIcon / Circular" },
            new DemoTocItem { Anchor = "切换按钮", Title = "切换按钮", Sub = "Toggle / Repeat / Progress" },
            new DemoTocItem { Anchor = "复合按钮", Title = "复合按钮", Sub = "Split / ContextMenu / Shield / Group" },
        };

        private DemoTocItem? _activeTocItem;
        public DemoTocItem? ActiveTocItem
        {
            get => _activeTocItem;
            set => SetProperty(ref _activeTocItem, value);
        }

        // ===== 绑定属性 =====
        private bool _toggleChecked;
        public bool ToggleChecked
        {
            get => _toggleChecked;
            set
            {
                if (SetProperty(ref _toggleChecked, value))
                    ToggleLabel = value ? "已选中 ✓" : "未选中";
            }
        }

        private string _toggleLabel = "未选中";
        public string ToggleLabel
        {
            get => _toggleLabel;
            set => SetProperty(ref _toggleLabel, value);
        }

        private double _progressValue = 65;
        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ICommand StartProgressCommand { get; }
        public ICommand ButtonClickCommand { get; }
        public ICommand SplitButtonActionCommand { get; }

        private string _lastClickResult = "点击上面任意按钮查看交互结果...";
        public string LastClickResult
        {
            get => _lastClickResult;
            set => SetProperty(ref _lastClickResult, value);
        }

        public ButtonsDemoViewModel()
        {
            StartProgressCommand = new DelegateCommand(OnStartProgress);
            ButtonClickCommand = new DelegateCommand<string>(OnButtonClick);
            SplitButtonActionCommand = new DelegateCommand<string>(OnSplitAction);
        }

        private async void OnStartProgress()
        {
            if (IsBusy) return;
            IsBusy = true;
            for (int i = 0; i <= 100; i += 2)
            {
                ProgressValue = i;
                await System.Threading.Tasks.Task.Delay(40);
            }
            IsBusy = false;
            ProgressValue = 100;
        }

        private void OnButtonClick(string? tag)
        {
            LastClickResult = $"点击了: {tag}  (时间: {System.DateTime.Now:HH:mm:ss})";
        }

        private void OnSplitAction(string? tag)
        {
            LastClickResult = $"SplitButton: {tag}  (时间: {System.DateTime.Now:HH:mm:ss})";
        }

        // ===== 代码示例字符串 =====

        public const string XamlButtonStyles = @"<!-- 实心按钮 -->
<Button Style=""{StaticResource ButtonDefault}"" Content=""默认"" />
<Button Style=""{StaticResource ButtonPrimary}"" Content=""主要"" />
<Button Style=""{StaticResource ButtonSuccess}"" Content=""成功"" />
<Button Style=""{StaticResource ButtonInfo}"" Content=""信息"" />
<Button Style=""{StaticResource ButtonWarning}"" Content=""警告"" />
<Button Style=""{StaticResource ButtonDanger}"" Content=""危险"" />

<!-- 小号按钮 (Small) -->
<Button Style=""{StaticResource ButtonPrimary.Small}"" Content=""主要"" />

<!-- 虚线按钮 (Dashed) -->
<Button Style=""{StaticResource ButtonDashedSuccess}"" Content=""成功"" />
<Button Style=""{StaticResource ButtonDashedDanger}"" Content=""危险"" />";

        public const string XamlIconButtons = @"<!-- 方形图标按钮 -->
<Button Style=""{StaticResource ButtonIcon}"">
    <pf:PackIcon Kind=""Heart"" Width=""16"" Height=""16"" />
</Button>

<!-- 圆形图标按钮 -->
<Button Style=""{StaticResource ButtonIconCircular}""
        Width=""40"" Height=""40""
        Content=""{pf:PackIcon Kind=Heart, Width=18, Height=18}"" />";

        public const string XamlToggleButton = @"<!-- ToggleButton：双向绑定到 IsChecked -->
<ToggleButton Content=""切换""
              IsChecked=""{Binding ToggleChecked}"" />

<!-- RepeatButton：长按持续触发 Click -->
<RepeatButton Content=""+ 长按递增""
              Delay=""500"" Interval=""100"" />";

        public const string XamlProgressButton = @"<pf:ProgressButton Content=""上传文件""
                   IsChecked=""{Binding IsBusy}""
                   Progress=""{Binding ProgressValue}""
                   Command=""{Binding StartProgressCommand}"" />";

        public const string XamlSplitButton = @"<pf:SplitButton Content=""操作""
                Style=""{StaticResource SplitButtonPrimary}""
                Command=""{Binding SplitButtonActionCommand}"">
    <pf:SplitButton.DropDownContent>
        <Border Padding=""8"" MinWidth=""130"">
            <StackPanel>
                <Button Content=""编辑"" />
                <Button Content=""删除"" />
            </StackPanel>
        </Border>
    </pf:SplitButton.DropDownContent>
</pf:SplitButton>";

        public const string XamlButtonGroup = @"<!-- 横向按钮组 -->
<pf:ButtonGroup Orientation=""Horizontal"">
    <Button Content=""剪切"" />
    <Button Content=""复制"" />
    <Button Content=""粘贴"" />
</pf:ButtonGroup>

<!-- 纵向按钮组 -->
<pf:ButtonGroup Orientation=""Vertical"">
    <ToggleButton Content=""左对齐"" IsChecked=""True"" />
    <ToggleButton Content=""居中"" />
</pf:ButtonGroup>";

        public const string XamlShield = @"<pf:Shield Subject=""设备"" Status=""在线""
           Color=""{DynamicResource DarkSuccessBrush}"" />
<pf:Shield Subject=""告警"" Status=""3条""
           Color=""{DynamicResource DarkWarningBrush}"" />";

        public const string XamlContextMenu = @"<pf:ContextMenuButton Content=""更多操作"">
    <pf:ContextMenuButton.Menu>
        <ContextMenu>
            <MenuItem Header=""新建"" />
            <MenuItem Header=""导入"" />
        </ContextMenu>
    </pf:ContextMenuButton.Menu>
</pf:ContextMenuButton>";
    }

    public class DemoTocItem : BindableBase
    {
        public string Anchor { get; init; } = "";
        public string Title { get; init; } = "";
        public string Sub { get; init; } = "";

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}
