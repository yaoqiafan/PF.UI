# PF.UI

企业级 WPF UI 组件库，面向 .NET 8 (net8.0-windows)。提供 90+ 自定义控件、完整深色/浅色主题、700+ Material Design 图标、思源黑体七字重嵌入字体，以及配套的值转换器、附加属性、交互行为工具集。

## 项目结构

```
PF.UI.slnx
├── PF.UI                   → 演示应用（Prism 9 Bootstrapper + 26 个演示页）
├── PF.UI.Infrastructure    → Prism 基础设施（行为、导航、对话框基类）
├── PF.UI.Controls          → 自定义控件库（NuGet 打包）
├── PF.UI.Resources         → 主题资源字典（NuGet 打包）
└── PF.UI.Shared            → 共享工具（转换器、帮助类、Interop，NuGet 打包）
```

依赖链（严格单向）：

```
PF.UI
  └── PF.UI.Infrastructure
        └── PF.UI.Resources
              └── PF.UI.Controls
                    └── PF.UI.Shared
```

## 技术栈

| 依赖 | 版本 |
|------|------|
| .NET | 8.0 (net8.0-windows) |
| WPF | 内置 |
| Prism.DryIoc | 9.0.537 |
| DryIoc | 6.x |
| XAMLTools.MSBuild | 1.0.0-alpha（主题合并）|

NuGet 版本统一管理：`Directory.Packages.props`

## 快速开始

### 安装 NuGet 包

```powershell
dotnet add package PF.UI.Controls
dotnet add package PF.UI.Resources
dotnet add package PF.UI.Shared
```

### 引入主题（App.xaml）

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- 控件模板 + 所有附加属性/转换器预注册 -->
            <ResourceDictionary Source="/PF.UI.Resources;component/Themes/Default.xaml"/>
            <!-- 浅色配色（替换为 Colors/Dark.xaml 启用深色主题）-->
            <ResourceDictionary Source="/PF.UI.Resources;component/Colors/Default.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 使用控件

```xml
<Window xmlns:pf="clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls">
    <StackPanel>
        <pf:PackIcon Kind="Heart" Width="32" Height="32"
                     Foreground="{DynamicResource PrimaryBrush}" />
        <pf:Growl.GrowlPanel />
        <pf:Badge Content="99+" Status="Error" />
        <pf:Chip Content="可删除" IsDeletable="True" />
        <pf:RangeSlider ValueStart="20" ValueEnd="80" />
    </StackPanel>
</Window>
```

## 运行演示应用

```powershell
cd source\repos\PF.UI
dotnet restore
dotnet run --project PF.UI
```

演示应用包含 **26 个页面**，覆盖全部控件、附加属性、转换器和交互行为。

## 控件清单

### 自定义控件（90+）

| 分类 | 控件 |
|------|------|
| **按钮** | `ProgressButton` `SplitButton` `ButtonGroup` `ContextMenuButton` `Shield` |
| **文本输入** | `WatermarkTextBox` `SearchBar` `PasswordBox` `PinBox` |
| **选择器** | `AutoCompleteTextBox` `CheckComboBox` `SearchComboBox` `NumericUpDown` `DateTimePicker` `TimePicker` `ColorPicker` `ImageSelector` |
| **布局面板** | `FlexPanel` `HoneycombPanel` `CirclePanel` `UniformSpacingPanel` `WaterfallPanel` `ClipGrid` `Col` `Row` `ElementGroup` |
| **导航** | `SideMenu` `StepBar` `Drawer` `DrawerContainer` `Pagination` `Ribbon` `SlidingTabContainer` |
| **反馈** | `Badge` `Card` `Growl` `Notification` `Poptip` `Divider` `Empty` `Magnifier` `ConfettiCannon` |
| **对话框** | `DialogOverlay` `DialogFrame` |
| **加载/进度** | `LoadingCircle` `LoadingLine` `CircleProgressBar` `WaveProgressBar` |
| **图标** | `PackIcon`（700+ Material Design 图标）|
| **动画** | `Ripple` `TransitioningContentControl` `AnimationPath` `Sprite` `RunningBlock` `ToggleBlock` |
| **标签** | `Tag` `TagContainer` `Chip` |
| **评分** | `Rate` `RateItem` |
| **滑块** | `PreviewSlider` `RangeSlider` `CompareSlider` |
| **时间** | `FlipClock` `Clock` `ListClock` `CalendarWithClock` `DateTimeSelector` |
| **属性网格** | `PropertyGrid`（12 种 Editor）|
| **窗口** | `Window` `BlurWindow` `GlowWindow` `PopupWindow` `NotifyIcon` |
| **截图** | `Screenshot` `ScreenshotWindow` |
| **文本** | `HighlightTextBlock` `OutlineText` `SimpleText` |
| **其他** | `DashedBorder` `BlendEffectBox` `GotoTop` `Watermark` `ChatBubble` |

### 原生控件样式重写（32 个）

Button、ToggleButton、RepeatButton、CheckBox、RadioButton、TextBox、PasswordBox、ComboBox、ListBox、ListView、DataGrid、TreeView、Menu、ContextMenu、ToolBar、StatusBar、TabControl、Expander、GroupBox、Label、TextBlock、Slider、ProgressBar、ScrollViewer、ToolTip、Separator、DatePicker、Calendar、RichTextBox、GridSplitter、Frame

### 附加属性（28 个类）

| 最常用 | 功能 |
|--------|------|
| `InfoElement` / `TitleElement` | 为输入控件附加标题、占位符、必填标记（支持继承）|
| `BorderElement` | `CornerRadius` 继承链、`Circular` 自动正圆 |
| `WindowAttach` | `IsDragElement` 拖窗、`IgnoreAltF4`、`HideWhenClosing` |
| `ScrollViewerAttach` | `AutoHide`、`Orientation` 横向滚轮、`IsDisabled` 防穿透 |
| `IconElement` | 向控件模板注入自定义 Geometry 图标 |
| `RippleAssist` | Ripple 涟漪行为控制 |

### 值转换器（33 个）

`Boolean2BooleanReConverter` · `Boolean2StringConverter` · `Boolean2VisibilityReConverter` · `BooleanArr2BooleanConverter` · `BooleanArr2VisibilityConverter` · `NullableToVisibilityConverter` · `Object2BooleanConverter` · `Object2BooleanReConverter` · `Object2VisibilityConverter` · `Object2VisibilityReConverter` · `Object2StringConverter` · `String2VisibilityConverter` · `String2VisibilityReConverter` · `Int2StringConverter` · `Long2FileSizeConverter` · `Number2PercentageConverter` · `DoubleMinConverter` · `Double2GridLengthConverter` · `DateTimeToStringConverter` · `Color2HexStringConverter` · `Color2ChannelAConverter` · `HsbToColorConverter` · `HsbLinearGradientConverter` · `BrushRoundConverter` 等

引入主题后已全部以 `StaticResource` 预注册，无需重复声明。

### 交互行为

| 类 | 功能 |
|----|------|
| `EventToCommand` | 任意 CLR 事件 → ViewModel 命令（支持 `PassEventArgsToCommand`、`MustToggleIsEnabled`）|
| `RoutedEventTrigger` | 支持冒泡/隧道 RoutedEvent 的触发器 |
| `MouseDragElementBehavior` | 鼠标拖拽任意 FrameworkElement（支持限制在父容器范围）|
| `FluidMoveBehavior` | Panel 子元素位移时平滑动画（独立 X/Y 缓动函数）|
| `ControlCommands` | 30+ 预置 RoutedCommand + 5 个全局 ICommand（CloseWindow / ShutdownApp / OpenLink 等）|

## 主题系统

```
PF.UI.Resources/
├── Themes/
│   ├── Basic/
│   │   ├── 01_Brushes.xaml
│   │   ├── 02_Converters.xaml     ← 转换器预注册
│   │   ├── 03_Effects.xaml
│   │   ├── 04_Fonts.xaml          ← 思源黑体 SC 7字重
│   │   ├── 05_Geometries.xaml
│   │   ├── 09_Sizes.xaml
│   │   ├── 10_Behaviors.xaml
│   │   └── 11_ControlBaseStyle.xaml
│   └── Default.xaml               ← XAMLTools 构建时自动合并所有 Themes/**/*.xaml
└── Colors/
    ├── Default.xaml               ← 浅色主题配色
    └── Dark.xaml                  ← 深色主题配色
```

> **注意**：不要直接编辑 `Themes/Default.xaml`，该文件由 `XAMLTools.MSBuild` 在构建时自动生成。请编辑 `Themes/` 子目录下的各分类文件。

## Prism 对话框集成

PF.UI 提供两种对话框模式：

```csharp
// 模式 1：DialogOverlay 页内遮罩（同页面视觉树，await 等待结果）
var frame = new DialogFrame { Header = "确认", Content = ... };
var result = await DialogOverlay.Show(frame, "MyToken");

// 模式 2：Prism IDialogService 独立窗口（继承 pf:Window）
// App.xaml.cs 注册：
containerRegistry.RegisterDialogWindow<PFDialogWindow>();
containerRegistry.RegisterDialog<MyDialogView, MyDialogViewModel>("MyDialog");

// ViewModel 注入使用：
_dialogService.ShowDialog("MyDialog", parameters, result => { ... });
```

## NuGet 打包

`Release` 构建自动生成 NuGet 包（三个可打包项目）：

```powershell
dotnet build -c Release
# 输出：
# PF.UI.Controls/bin/Release/*.nupkg
# PF.UI.Resources/bin/Release/*.nupkg
# PF.UI.Shared/bin/Release/*.nupkg
```

## 演示页面列表

| 分组 | 页面 | 覆盖控件 |
|------|------|---------|
| 概览 | OverviewDemoView | 快速开始、项目架构、控件统计 |
| 按钮 | ButtonsDemoView | Button 全样式、ProgressButton、SplitButton、ButtonGroup、Shield |
| 输入 | TextInputDemoView | TextBox、WatermarkTextBox、SearchBar、PasswordBox、PinBox |
| 输入 | SelectorsDemoView | ComboBox、CheckComboBox、SearchComboBox、AutoCompleteTextBox、NumericUpDown |
| 数据 | DataDemoView | DataGrid、ListBox、ListView、TreeView、PropertyGrid、Pagination |
| 布局 | PanelsDemoView | FlexPanel、HoneycombPanel、CirclePanel、WaterfallPanel、ClipGrid、Col/Row |
| 导航 | NavigationDemoView | SideMenu、StepBar、TabControl、SlidingTabContainer、Drawer |
| 反馈 | GrowlNotificationView | Growl、Notification、Badge、Card、Poptip、ChatBubble |
| 反馈 | StatusIndicatorView | Empty、Divider、Shield、Magnifier |
| 反馈 | ProgressLoadingView | LoadingCircle、LoadingLine、CircleProgressBar、WaveProgressBar、ConfettiCannon |
| 动画 | AnimationDemoView | Ripple、TransitioningContentControl、AnimationPath、Sprite、RunningBlock |
| 对话框 | DialogDemoView | DialogOverlay（6种）+ Prism IDialogService |
| 图标 | PackIconDemoView | PackIcon 700+ 图标搜索 |
| 涟漪 | RippleDemoView | RippleAssist 全属性 |
| 颜色 | ColorPickerDemoView | ColorPicker |
| 颜色 | ColorManipulationDemoView | 颜色操作工具 |
| 颜色 | ColorPaletteDemoView | Material Design 19×14 调色板 |
| 标签 | ChipDemoView | Chip 全样式 |
| 标签评分 | TagsRateDemoView | Tag、TagContainer、Rate |
| 滑块 | SlidersDemoView | Slider、RangeSlider、PreviewSlider、CompareSlider |
| 时间 | TimeDemoView | FlipClock、Clock、ListClock、TimePicker、DateTimePicker、DateTimeSelector、CalendarWithClock |
| 附加属性 | AttachedDemoView | InfoElement/TitleElement、IconElement、BorderElement、WindowAttach、ScrollViewerAttach |
| 转换器 | ConvertersDemoView | 全部 33 个转换器说明与用法 |
| 交互 | InteractivityDemoView | EventToCommand、ControlCommands、MouseDragElementBehavior、FluidMoveBehavior |
| 窗口 | WindowsDemoView | pf:Window、BlurWindow、GlowWindow、PopupWindow、NotifyIcon、Screenshot |
