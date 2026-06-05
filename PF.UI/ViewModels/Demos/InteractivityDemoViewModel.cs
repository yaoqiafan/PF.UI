using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PF.UI.ViewModels.Demos
{
    public class InteractivityDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "EventToCommand", Title = "EventToCommand",      Sub = "事件 → 命令桥接" },
            new DemoTocItem { Anchor = "Commands",       Title = "内置命令",            Sub = "ControlCommands 速查" },
            new DemoTocItem { Anchor = "DragBehavior",   Title = "拖拽行为",            Sub = "MouseDragElementBehavior" },
            new DemoTocItem { Anchor = "FluidMove",      Title = "流体动画",            Sub = "FluidMoveBehavior" },
        };

        private string _lastResult = "与下方控件交互查看效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ─── EventToCommand 演示 ───────────────────────────────────────────

        public DelegateCommand LogMouseEnterCommand => new(() =>
            LastResult = $"Border.MouseEnter → EventToCommand 触发  ({DateTime.Now:HH:mm:ss})");

        public DelegateCommand LogLostFocusCommand => new(() =>
            LastResult = $"TextBox.LostFocus → EventToCommand 触发  ({DateTime.Now:HH:mm:ss})");

        public DelegateCommand LogSelectionChangedCommand => new(() =>
            LastResult = $"ListBox.SelectionChanged → EventToCommand 触发  ({DateTime.Now:HH:mm:ss})");

        public DelegateCommand LogPreviewClickCommand => new(() =>
            LastResult = $"RoutedEventTrigger.PreviewMouseDown 触发  ({DateTime.Now:HH:mm:ss})");

        // MustToggleIsEnabled 演示
        private bool _isCommandEnabled = true;
        public bool IsCommandEnabled
        {
            get => _isCommandEnabled;
            set
            {
                if (SetProperty(ref _isCommandEnabled, value))
                    SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand? _submitCommand;
        public DelegateCommand SubmitCommand => _submitCommand ??= new DelegateCommand(
            () => LastResult = $"SubmitCommand 执行（CanExecute=True）  ({DateTime.Now:HH:mm:ss})",
            () => IsCommandEnabled);

        // ─── FluidMoveBehavior 演示 ────────────────────────────────────────

        private int _fluidCounter = 5;
        public ObservableCollection<string> FluidItems { get; } = new()
            { "WPF", ".NET 8", "MVVM", "Prism" };

        public DelegateCommand AddFluidItemCommand => new(() =>
        {
            var tag = $"Item {_fluidCounter++}";
            var idx = new Random().Next(FluidItems.Count + 1);
            FluidItems.Insert(idx, tag);
            LastResult = $"插入 \"{tag}\" at [{idx}]  ({DateTime.Now:HH:mm:ss})";
        });

        public DelegateCommand RemoveLastCommand => new(
            () =>
            {
                var tag = FluidItems[^1];
                FluidItems.RemoveAt(FluidItems.Count - 1);
                LastResult = $"移除 \"{tag}\"  ({DateTime.Now:HH:mm:ss})";
            },
            () => FluidItems.Count > 0);

        public DelegateCommand ShuffleCommand => new(() =>
        {
            var rng = new Random();
            var shuffled = FluidItems.OrderBy(_ => rng.Next()).ToList();
            for (int i = 0; i < shuffled.Count; i++)
            {
                var from = FluidItems.IndexOf(shuffled[i]);
                if (from != i) FluidItems.Move(from, i);
            }
            LastResult = $"乱序重排  ({DateTime.Now:HH:mm:ss})";
        });

        // ─── 代码示例 ──────────────────────────────────────────────────────

        public const string XamlEventToCommand = @"<!-- 命名空间：EventToCommand / EventTrigger / Interaction 均在 PF.UI.Controls -->
xmlns:pf=""clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls""

<!-- 基础：将任意事件绑定到 ViewModel 命令（无 Command 属性的控件场景）-->
<Border Background=""Transparent"">
    <pf:Interaction.Triggers>
        <pf:EventTrigger EventName=""MouseEnter"">
            <pf:EventToCommand Command=""{Binding LogMouseEnterCommand}"" />
        </pf:EventTrigger>
    </pf:Interaction.Triggers>
</Border>

<!-- PassEventArgsToCommand：原始 EventArgs 作为命令参数传入 ViewModel -->
<Slider>
    <pf:Interaction.Triggers>
        <pf:EventTrigger EventName=""ValueChanged"">
            <pf:EventToCommand Command=""{Binding OnValueChangedCommand}""
                               PassEventArgsToCommand=""True"" />
        </pf:EventTrigger>
    </pf:Interaction.Triggers>
</Slider>
// ViewModel 侧（避免 UI 引用可用 IEventArgsConverter 解耦）：
public DelegateCommand<object> OnValueChangedCommand => new(args =>
{
    if (args is RoutedPropertyChangedEventArgs<double> e)
        LastResult = $""NewValue: {e.NewValue:F1}"";
});

<!-- MustToggleIsEnabled：CanExecute=false 时自动禁用宿主控件 -->
<Button>
    <pf:Interaction.Triggers>
        <pf:EventTrigger EventName=""Click"">
            <pf:EventToCommand Command=""{Binding SubmitCommand}""
                               MustToggleIsEnabled=""True"" />
        </pf:EventTrigger>
    </pf:Interaction.Triggers>
</Button>

<!-- RoutedEventTrigger：支持冒泡/隧道 RoutedEvent，普通 EventTrigger 不支持 -->
<StackPanel>
    <pf:Interaction.Triggers>
        <pf:RoutedEventTrigger RoutedEvent=""{x:Static UIElement.PreviewMouseDownEvent}"">
            <pf:EventToCommand Command=""{Binding LogPreviewClickCommand}"" />
        </pf:RoutedEventTrigger>
    </pf:Interaction.Triggers>
</StackPanel>";

        public const string XamlCommands = @"<!-- 内置 ICommand：直接绑定，无需 CommandBinding -->
<!-- CommandParameter 传入 DependencyObject，CloseWindowCommand 向上查找 Window.Close() -->
<Button Command=""{x:Static pf:ControlCommands.CloseWindow}""
        CommandParameter=""{Binding RelativeSource={RelativeSource Self}}""
        Content=""关闭窗口"" />

<Button Command=""{x:Static pf:ControlCommands.ShutdownApp}""
        Content=""退出程序"" />

<!-- OpenLinkCommand：CommandParameter 为 URL 字符串 -->
<Button Command=""{x:Static pf:ControlCommands.OpenLink}""
        CommandParameter=""https://github.com""
        Content=""打开链接"" />

<!-- RoutedCommand：需在视觉树祖先节点的 CommandBindings 中注册处理逻辑 -->
<Window>
    <Window.CommandBindings>
        <CommandBinding Command=""{x:Static pf:ControlCommands.Search}""
                        Executed=""OnSearch_Executed"" />
    </Window.CommandBindings>
    <Button Command=""{x:Static pf:ControlCommands.Search}"" Content=""搜索"" />
</Window>
// 或在 ViewModel（Prism）中通过 CompositeCommand 注册：
containerRegistry.RegisterForNavigation<SearchView>();";

        public const string XamlDragBehavior = @"<!-- MouseDragElementBehavior：鼠标拖拽任意 FrameworkElement -->
<!-- 通过修改元素 RenderTransform（TranslateTransform）实现移动 -->

<Canvas Height=""180"" Background=""{DynamicResource SecondaryRegionBrush}"">
    <!-- 默认：可拖出父容器范围 -->
    <Border Width=""80"" Height=""80"" Canvas.Left=""20"" Canvas.Top=""50""
            Background=""{DynamicResource PrimaryBrush}"" CornerRadius=""8"">
        <pf:Interaction.Behaviors>
            <pf:MouseDragElementBehavior />
        </pf:Interaction.Behaviors>
    </Border>

    <!-- ConstrainToParentBounds=True：拖拽限制在父容器范围内 -->
    <Border Width=""80"" Height=""80"" Canvas.Left=""140"" Canvas.Top=""50""
            Background=""{DynamicResource AccentBrush}"" CornerRadius=""8"">
        <pf:Interaction.Behaviors>
            <pf:MouseDragElementBehavior ConstrainToParentBounds=""True"" />
        </pf:Interaction.Behaviors>
    </Border>
</Canvas>

<!-- 注意：元素必须有 RenderTransform 空间（Canvas 或 TranslateTransform）
     X / Y 属性可双向绑定获取当前拖拽位置 -->
<pf:MouseDragElementBehavior X=""{Binding PosX, Mode=TwoWay}""
                              Y=""{Binding PosY, Mode=TwoWay}"" />";

        public const string XamlFluidMove = @"<!-- FluidMoveBehavior：Panel 子元素位置变化时平滑动画过渡 -->
<!-- 附加到 ItemsControl 的 ItemsPanel（WrapPanel），实现集合增删重排动画 -->

<ItemsControl ItemsSource=""{Binding FluidItems}"">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel>
                <pf:Interaction.Behaviors>
                    <pf:FluidMoveBehavior Duration=""0:0:0.4"" FloatAbove=""True"">
                        <pf:FluidMoveBehavior.EaseX>
                            <CubicEase EasingMode=""EaseInOut"" />
                        </pf:FluidMoveBehavior.EaseX>
                        <pf:FluidMoveBehavior.EaseY>
                            <CubicEase EasingMode=""EaseInOut"" />
                        </pf:FluidMoveBehavior.EaseY>
                    </pf:FluidMoveBehavior>
                </pf:Interaction.Behaviors>
            </WrapPanel>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border Margin=""4"" Padding=""12,6"" CornerRadius=""4""
                    Background=""{DynamicResource PrimaryBrush}"">
                <TextBlock Text=""{Binding}"" Foreground=""White"" />
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>

<!-- 关键属性
     Duration   — 动画时长（默认 1s，建议 0.3s–0.6s）
     FloatAbove — 移动中的元素是否浮在其他元素上方（默认 True）
     EaseX/EaseY — X/Y 方向各自的缓动函数（可独立配置）

     触发条件：ObservableCollection 的 Add / Remove / Move 操作
     Move（乱序）比 Clear+Add 动画更连贯 -->";
    }
}
