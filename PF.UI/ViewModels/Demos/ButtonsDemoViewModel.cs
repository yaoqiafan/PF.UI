using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ButtonsDemoViewModel : DemoViewModelBase
    {
        // ===== TOC 目录 =====
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "基础按钮", Title = "基础按钮", Sub = "Solid / Small / Dashed" },
            new DemoTocItem { Anchor = "图标按钮", Title = "图标按钮", Sub = "ButtonIcon / Circular" },
            new DemoTocItem { Anchor = "切换按钮", Title = "ToggleButton", Sub = "Solid / Icon / Switch / Loading" },
            new DemoTocItem { Anchor = "单选按钮", Title = "RadioButton", Sub = "SameAsButton / Icon / Group" },
            new DemoTocItem { Anchor = "长按重复", Title = "RepeatButton", Sub = "Solid / Dashed / 长按计数" },
            new DemoTocItem { Anchor = "进度按钮", Title = "ProgressButton", Sub = "带进度条的按钮" },
            new DemoTocItem { Anchor = "分割按钮", Title = "SplitButton", Sub = "主操作 + 下拉菜单" },
            new DemoTocItem { Anchor = "复合按钮", Title = "复合按钮", Sub = "ContextMenu / Shield / Group" },
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

        private int _repeatCount;
        public int RepeatCount
        {
            get => _repeatCount;
            set => SetProperty(ref _repeatCount, value);
        }

        private string _repeatLabel = "按住按钮递增计数器";
        public string RepeatLabel
        {
            get => _repeatLabel;
            set => SetProperty(ref _repeatLabel, value);
        }

        private string _selectedRadio = "无";
        public string SelectedRadio
        {
            get => _selectedRadio;
            set => SetProperty(ref _selectedRadio, value);
        }

        public ICommand StartProgressCommand { get; }
        public ICommand ButtonClickCommand { get; }
        public ICommand IncrementRepeatCommand { get; }
        public ICommand RadioCheckedCommand { get; }
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
            IncrementRepeatCommand = new DelegateCommand(OnIncrementRepeat);
            RadioCheckedCommand = new DelegateCommand<string>(OnRadioChecked);
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

        private void OnIncrementRepeat()
        {
            RepeatCount++;
            RepeatLabel = $"长按计数: {RepeatCount}";
            LastClickResult = $"RepeatButton 触发 #{RepeatCount}  (时间: {System.DateTime.Now:HH:mm:ss})";
        }

        private void OnRadioChecked(string? tag)
        {
            SelectedRadio = tag ?? "未知";
            LastClickResult = $"RadioButton 选中: {tag}  (时间: {System.DateTime.Now:HH:mm:ss})";
        }

        private void OnSplitAction(string? tag)
        {
            LastClickResult = $"SplitButton: {tag}  (时间: {System.DateTime.Now:HH:mm:ss})";
        }

        // ===== 代码示例字符串 =====

        public const string XamlButtonSolid = @"<!-- 实心按钮 6 色 -->
<Button Style=""{StaticResource ButtonDefault}""  Content=""默认"" />
<Button Style=""{StaticResource ButtonPrimary}""  Content=""主要"" />
<Button Style=""{StaticResource ButtonSuccess}""  Content=""成功"" />
<Button Style=""{StaticResource ButtonInfo}""     Content=""信息"" />
<Button Style=""{StaticResource ButtonWarning}""  Content=""警告"" />
<Button Style=""{StaticResource ButtonDanger}""   Content=""危险"" />

<!-- 小号变体 (.Small) -->
<Button Style=""{StaticResource ButtonDefault.Small}""  Content=""默认"" />
<Button Style=""{StaticResource ButtonPrimary.Small}""  Content=""主要"" />
<Button Style=""{StaticResource ButtonSuccess.Small}""  Content=""成功"" />
<Button Style=""{StaticResource ButtonInfo.Small}""     Content=""信息"" />
<Button Style=""{StaticResource ButtonWarning.Small}""  Content=""警告"" />
<Button Style=""{StaticResource ButtonDanger.Small}""   Content=""危险"" />";

        public const string XamlButtonDashed = @"<!-- 虚线按钮 6 色 -->
<Button Style=""{StaticResource ButtonDashed}""        Content=""默认"" />
<Button Style=""{StaticResource ButtonDashedPrimary}""  Content=""主要"" />
<Button Style=""{StaticResource ButtonDashedSuccess}""  Content=""成功"" />
<Button Style=""{StaticResource ButtonDashedInfo}""     Content=""信息"" />
<Button Style=""{StaticResource ButtonDashedWarning}""  Content=""警告"" />
<Button Style=""{StaticResource ButtonDashedDanger}""   Content=""危险"" />

<!-- 虚线小号变体 (.Small) -->
<Button Style=""{StaticResource ButtonDashedDanger.Small}"" Content=""危险"" />";

        public const string XamlIconButtons = @"<!-- 方形图标按钮 — ButtonIcon -->
<Button Style=""{StaticResource ButtonIcon}"">
    <pf:PackIcon Kind=""Heart"" Width=""16"" Height=""16"" />
</Button>

<!-- 圆形图标按钮 — ButtonIconCircular -->
<Button Style=""{StaticResource ButtonIconCircular}""
        Width=""40"" Height=""40"">
    <pf:PackIcon Kind=""Play"" Width=""18"" Height=""18"" />
</Button>

<!-- 小号变体 -->
<Button Style=""{StaticResource ButtonIcon.Small}"">
    <pf:PackIcon Kind=""Heart"" Width=""12"" Height=""12"" />
</Button>
<Button Style=""{StaticResource ButtonIconCircular.Small}""
        Width=""20"" Height=""20"">
    <pf:PackIcon Kind=""Play"" Width=""12"" Height=""12"" />
</Button>";

        public const string XamlToggleSolid = @"<!-- ToggleButton 实心 6 色 -->
<ToggleButton Style=""{StaticResource ToggleButtonDefault}""  Content=""默认"" />
<ToggleButton Style=""{StaticResource ToggleButtonPrimary}""  Content=""主要"" />
<ToggleButton Style=""{StaticResource ToggleButtonSuccess}""  Content=""成功"" />
<ToggleButton Style=""{StaticResource ToggleButtonInfo}""     Content=""信息"" />
<ToggleButton Style=""{StaticResource ToggleButtonWarning}""  Content=""警告"" />
<ToggleButton Style=""{StaticResource ToggleButtonDanger}""   Content=""危险"" />

<!-- 小号变体 (.Small) -->
<ToggleButton Style=""{StaticResource ToggleButtonDefault.Small}""  Content=""默认"" />
<!-- ToggleButtonPrimary.Small / Success / Info / Warning / Danger.Small 同理 -->";

        public const string XamlToggleIcon = @"<!-- ToggleButtonIcon 通过 IconSwitchElement 指定图标 -->
<!-- Geometry = 未选中图标, GeometrySelected = 选中图标（切换显示）-->
<ToggleButton Style=""{StaticResource ToggleButtonIcon}""
              pf:IconSwitchElement.Geometry=""{StaticResource EyeOpenGeometry}""
              pf:IconSwitchElement.GeometrySelected=""{StaticResource EyeCloseGeometry}"" />

<ToggleButton Style=""{StaticResource ToggleButtonIconPrimary}""
              pf:IconSwitchElement.Geometry=""{StaticResource FullScreenGeometry}""
              pf:IconSwitchElement.GeometrySelected=""{StaticResource FullScreenReturnGeometry}"" />

<!-- ToggleButtonIconSuccess / Info / Warning / Danger 同理 -->

<!-- 透明背景，无边框 -->
<ToggleButton Style=""{StaticResource ToggleButtonIconTransparent}""
              pf:IconSwitchElement.Geometry=""{StaticResource LeftGeometry}""
              pf:IconSwitchElement.GeometrySelected=""{StaticResource RightGeometry}"" />";

        public const string XamlToggleSwitch = @"<!-- 开关样式 ToggleButton -->
<ToggleButton Style=""{StaticResource ToggleButtonSwitch}""
              Content=""开关"" />
<ToggleButton Style=""{StaticResource ToggleButtonSwitch.Small}""
              Content=""小开关"" />";

        public const string XamlToggleLoading = @"<!-- 加载中 ToggleButton 6 色 -->
<ToggleButton Style=""{StaticResource ToggleButtonLoading}""
              IsChecked=""{Binding IsBusy}""
              Content=""加载中"" />
<ToggleButton Style=""{StaticResource ToggleButtonLoadingPrimary}""
              Content=""主要加载"" />
<!-- ToggleButtonLoadingSuccess / Info / Warning / Danger 同理 -->";

        public const string XamlRadioSameAsButton = @"<!-- RadioButton 伪装成普通按钮，但具有互斥行为 -->
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonDefault}""
             Content=""选项 A"" GroupName=""Demo"" />
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonPrimary}""
             Content=""选项 B"" GroupName=""Demo"" />
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonSuccess}""
             Content=""选项 C"" GroupName=""Demo"" />
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonInfo}""
             Content=""选项 D"" GroupName=""Demo"" />
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonWarning}""
             Content=""选项 E"" GroupName=""Demo"" />
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonDanger}""
             Content=""选项 F"" GroupName=""Demo"" />

<!-- 小号变体 (.Small) -->
<RadioButton Style=""{StaticResource RadioButtonSameAsButtonDefault.Small}""
             Content=""小选项"" GroupName=""DemoSmall"" />";

        public const string XamlRadioIcon = @"<!-- RadioButton 图标单选 -->
<RadioButton Style=""{StaticResource RadioButtonIcon}""
             GroupName=""IconDemo"">
    <pf:PackIcon Kind=""FormatAlignLeft"" Width=""16"" Height=""16"" />
</RadioButton>
<RadioButton Style=""{StaticResource RadioButtonIcon}""
             GroupName=""IconDemo"">
    <pf:PackIcon Kind=""FormatAlignCenter"" Width=""16"" Height=""16"" />
</RadioButton>
<!-- RadioButtonIcon.Small 同理 -->";

        public const string XamlRepeat = @"<!-- RepeatButton 实心 6 色 -->
<RepeatButton Style=""{StaticResource RepeatButtonDefault}""  Content=""默认"" />
<RepeatButton Style=""{StaticResource RepeatButtonPrimary}""  Content=""主要"" />
<RepeatButton Style=""{StaticResource RepeatButtonSuccess}""  Content=""成功"" />
<RepeatButton Style=""{StaticResource RepeatButtonInfo}""     Content=""信息"" />
<RepeatButton Style=""{StaticResource RepeatButtonWarning}""  Content=""警告"" />
<RepeatButton Style=""{StaticResource RepeatButtonDanger}""   Content=""危险"" />

<!-- 虚线 6 色 -->
<RepeatButton Style=""{StaticResource RepeatButtonDashed}""        Content=""默认"" />
<RepeatButton Style=""{StaticResource RepeatButtonDashedPrimary}""  Content=""主要"" />
<RepeatButton Style=""{StaticResource RepeatButtonDashedSuccess}""  Content=""成功"" />
<RepeatButton Style=""{StaticResource RepeatButtonDashedInfo}""     Content=""信息"" />
<RepeatButton Style=""{StaticResource RepeatButtonDashedWarning}""  Content=""警告"" />
<RepeatButton Style=""{StaticResource RepeatButtonDashedDanger}""   Content=""危险"" />

<!-- 长按计数 -->
<RepeatButton Content=""+ 长按递增""
              Delay=""500"" Interval=""100""
              Command=""{Binding IncrementRepeatCommand}"" />";

        public const string XamlProgressButton = @"<pf:ProgressButton Content=""上传文件""
                   IsChecked=""{Binding IsBusy}""
                   Progress=""{Binding ProgressValue}""
                   Command=""{Binding StartProgressCommand}"" />";

        public const string XamlSplitButton = @"<!-- SplitButton 6 色 -->
<pf:SplitButton Content=""默认""
                Style=""{StaticResource SplitButtonDefault}"" />
<pf:SplitButton Content=""主要""
                Style=""{StaticResource SplitButtonPrimary}"" />
<pf:SplitButton Content=""成功""
                Style=""{StaticResource SplitButtonSuccess}"" />
<pf:SplitButton Content=""信息""
                Style=""{StaticResource SplitButtonInfo}"" />
<pf:SplitButton Content=""警告""
                Style=""{StaticResource SplitButtonWarning}"" />
<pf:SplitButton Content=""危险""
                Style=""{StaticResource SplitButtonDanger}"" />

<!-- 带下拉菜单 -->
<pf:SplitButton Content=""保存""
                Style=""{StaticResource SplitButtonPrimary}""
                Command=""{Binding SplitButtonActionCommand}""
                CommandParameter=""保存文件"">
    <pf:SplitButton.DropDownContent>
        <Border Padding=""8"" MinWidth=""130"">
            <StackPanel>
                <Button Content=""保存"" />
                <Button Content=""另存为..."" />
                <Separator />
                <Button Content=""全部保存"" />
            </StackPanel>
        </Border>
    </pf:SplitButton.DropDownContent>
</pf:SplitButton>";

        public const string XamlButtonGroup = @"<!-- 横向按钮组 (Button) -->
<pf:ButtonGroup Orientation=""Horizontal"">
    <Button Content=""剪切"" />
    <Button Content=""复制"" />
    <Button Content=""粘贴"" />
</pf:ButtonGroup>

<!-- 纵向按钮组 (RadioButton 互斥选择) -->
<pf:ButtonGroup Orientation=""Vertical"">
    <RadioButton Content=""左对齐"" IsChecked=""True"" GroupName=""AlignV"" />
    <RadioButton Content=""居中"" GroupName=""AlignV"" />
    <RadioButton Content=""右对齐"" GroupName=""AlignV"" />
    <RadioButton Content=""两端对齐"" GroupName=""AlignV"" />
</pf:ButtonGroup>";

        public const string XamlShield = @"<pf:Shield Subject=""设备"" Status=""在线""
           Color=""{DynamicResource DarkSuccessBrush}"" />
<pf:Shield Subject=""告警"" Status=""3条""
           Color=""{DynamicResource DarkWarningBrush}"" />
<pf:Shield Subject=""错误"" Status=""5""
           Color=""{DynamicResource DangerBrush}"" />
<pf:Shield Subject=""状态"" Status=""离线""
           Color=""{DynamicResource SecondaryTextBrush}"" />";

        public const string XamlContextMenu = @"<pf:ContextMenuButton Content=""更多操作"">
    <pf:ContextMenuButton.Menu>
        <ContextMenu>
            <MenuItem Header=""新建项目"" />
            <MenuItem Header=""导入文件"" />
            <Separator />
            <MenuItem Header=""设置"" />
        </ContextMenu>
    </pf:ContextMenuButton.Menu>
</pf:ContextMenuButton>";

        public const string XamlContextMenuToggle = @"<!-- ContextMenuToggleButton — ToggleButton + 右键菜单 -->
<!-- IsChecked=True 时展开菜单，False 时关闭菜单 -->
<pf:ContextMenuToggleButton Content=""切换菜单 ▼"">
    <pf:ContextMenuToggleButton.Menu>
        <ContextMenu>
            <MenuItem Header=""操作 A"" />
            <MenuItem Header=""操作 B"" />
            <Separator />
            <MenuItem Header=""取消"" />
        </ContextMenu>
    </pf:ContextMenuToggleButton.Menu>
</pf:ContextMenuToggleButton>";

        public const string XamlRepeatIcon = @"<!-- RepeatButtonIcon — 图标长按（支持 Content 或 IconElement.Geometry）-->
<RepeatButton Style=""{StaticResource RepeatButtonIcon}""
              Delay=""500"" Interval=""100""
              Command=""{Binding IncrementRepeatCommand}"">
    <pf:PackIcon Kind=""Plus"" Width=""14"" Height=""14"" />
</RepeatButton>

<!-- RepeatButtonIconCircular — 圆形图标长按 (用 IconElement.Geometry) -->
<RepeatButton Style=""{StaticResource RepeatButtonIconCircular}""
              Width=""36"" Height=""36""
              pf:IconElement.Geometry=""{StaticResource UpGeometry}""
              Delay=""500"" Interval=""100""
              Command=""{Binding IncrementRepeatCommand}"" />
<RepeatButton Style=""{StaticResource RepeatButtonIconCircular}""
              Width=""36"" Height=""36""
              pf:IconElement.Geometry=""{StaticResource DownGeometry}""
              Delay=""500"" Interval=""100""
              Command=""{Binding IncrementRepeatCommand}"" />";
    }

    public class DemoTocItem : DemoViewModelBase
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
