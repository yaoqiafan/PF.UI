using System.Collections.ObjectModel;
using Prism.Mvvm;
using PF.UI.Controls;

namespace PF.UI.ViewModels.Demos
{
    public class CoverItem
    {
        public string Title { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
        public PackIconKind Icon { get; init; }
    }

    public class MediaDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "CoverView",    Title = "CoverView",    Sub = "封面分组视图" },
            new DemoTocItem { Anchor = "CoverFlow",    Title = "CoverFlow",    Sub = "3D 翻转流" },
            new DemoTocItem { Anchor = "ImageGallery", Title = "图片滚动视图", Sub = "横向图片画廊" },
        };

        public ObservableCollection<CoverItem> CoverItems { get; } = new()
        {
            new CoverItem { Title = "PLC 控制器", Subtitle = "工业自动化核心", Color = "#1565C0", Icon = PackIconKind.Chip },
            new CoverItem { Title = "伺服驱动器", Subtitle = "高精度运动控制", Color = "#2E7D32", Icon = PackIconKind.CogOutline },
            new CoverItem { Title = "触摸屏 HMI", Subtitle = "人机交互界面",   Color = "#E65100", Icon = PackIconKind.Monitor },
            new CoverItem { Title = "传感器套件", Subtitle = "数据采集",       Color = "#6A1B9A", Icon = PackIconKind.Radar },
            new CoverItem { Title = "工业相机",   Subtitle = "视觉检测",       Color = "#00838F", Icon = PackIconKind.Camera },
            new CoverItem { Title = "SCADA 软件", Subtitle = "监控与数据采集", Color = "#C62828", Icon = PackIconKind.Database },
            new CoverItem { Title = "气动夹爪",   Subtitle = "执行机构",       Color = "#558B2F", Icon = PackIconKind.Wrench },
            new CoverItem { Title = "导轨滑块",   Subtitle = "线性运动",       Color = "#283593", Icon = PackIconKind.Robot },
        };

        public ObservableCollection<CoverItem> GalleryItems { get; } = new()
        {
            new CoverItem { Title = "外观检测",  Subtitle = "AOI 视觉",   Color = "#1A237E", Icon = PackIconKind.Camera },
            new CoverItem { Title = "激光焊接",  Subtitle = "精密加工",   Color = "#1B5E20", Icon = PackIconKind.Chip },
            new CoverItem { Title = "六轴机器人", Subtitle = "柔性装配",  Color = "#B71C1C", Icon = PackIconKind.Robot },
            new CoverItem { Title = "输送线",    Subtitle = "物料传送",   Color = "#E65100", Icon = PackIconKind.CogOutline },
            new CoverItem { Title = "MES 调度",  Subtitle = "生产排程",   Color = "#4A148C", Icon = PackIconKind.Database },
            new CoverItem { Title = "AGV 小车",  Subtitle = "自动导引",   Color = "#006064", Icon = PackIconKind.Radar },
            new CoverItem { Title = "仓储管理",  Subtitle = "WMS 系统",   Color = "#33691E", Icon = PackIconKind.Monitor },
            new CoverItem { Title = "数据看板",  Subtitle = "实时可视化", Color = "#BF360C", Icon = PackIconKind.Chip },
            new CoverItem { Title = "设备维保",  Subtitle = "预测维护",   Color = "#0D47A1", Icon = PackIconKind.Wrench },
            new CoverItem { Title = "质量追溯",  Subtitle = "全链路追踪", Color = "#880E4F", Icon = PackIconKind.CogOutline },
        };

        public const string XamlCoverView = @"<!-- CoverView — 封面分组视图（RegularItemsControl 子类）-->
<pf:CoverView Groups=""4"" ItemWidth=""150"" ItemHeight=""180""
              ItemsSource=""{Binding CoverItems}""
              ItemContentHeight=""70"" ItemContentHeightFixed=""True"">
    <pf:CoverView.ItemHeaderTemplate>
        <DataTemplate>
            <!-- 封面图区：彩色背景 + 大图标 + 标题叠加 -->
            <Border Margin=""2"" CornerRadius=""6,6,0,0""
                    Background=""{Binding Color}"" ClipToBounds=""True"">
                <Grid>
                    <pf:PackIcon Kind=""{Binding Icon}""
                                 Width=""52"" Height=""52""
                                 Foreground=""White"" Opacity=""0.18""
                                 HorizontalAlignment=""Right""
                                 VerticalAlignment=""Bottom""
                                 Margin=""0,0,6,4"" />
                    <TextBlock Text=""{Binding Title}""
                               FontSize=""12"" FontWeight=""Bold""
                               Foreground=""White""
                               Margin=""10,0,8,8""
                               VerticalAlignment=""Bottom"" />
                </Grid>
            </Border>
        </DataTemplate>
    </pf:CoverView.ItemHeaderTemplate>
    <pf:CoverView.ItemTemplate>
        <DataTemplate>
            <Border Padding=""8,6"" Margin=""2,0,2,2""
                    Background=""{DynamicResource RegionBrush}""
                    CornerRadius=""0,0,6,6"">
                <TextBlock Text=""{Binding Subtitle}"" FontSize=""11""
                           Foreground=""{DynamicResource SecondaryTextBrush}""
                           TextWrapping=""Wrap"" />
            </Border>
        </DataTemplate>
    </pf:CoverView.ItemTemplate>
</pf:CoverView>

<!-- 关键属性：
     Groups             — 列数
     ItemWidth / ItemHeight   — 每项整体尺寸
     ItemContentHeight  — 底部内容区域高度
     ItemContentHeightFixed — 固定内容区高度 -->";

        public const string XamlCoverFlow = @"<!-- CoverFlow — 3D 翻转流（通过代码添加项目）-->

<!-- XAML 中放置控件 -->
<pf:CoverFlow x:Name=""MyCoverFlow"" Width=""480"" Height=""260"" />

<!-- 代码中初始化（Loaded 事件后） -->
// 添加 Uri（自动创建 BitmapImage）
MyCoverFlow.Add(new Uri(""pack://application:,,,/Assets/cover1.png""));

// 或批量添加任意 UIElement
MyCoverFlow.AddRange(cards); // IEnumerable<object>

// 页面切换
MyCoverFlow.Next();
MyCoverFlow.Prev();
MyCoverFlow.PageIndex = 3;  // 直接跳转";

        public const string XamlGallery = @"<!-- 图片滚动画廊 — 横向 ScrollViewer + ItemsControl -->
<ScrollViewer HorizontalScrollBarVisibility=""Auto""
              VerticalScrollBarVisibility=""Disabled"">
    <ItemsControl ItemsSource=""{Binding GalleryItems}"">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel Orientation=""Horizontal"" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <Border Width=""140"" Height=""180"" Margin=""0,0,10,0""
                        CornerRadius=""10"" Background=""{Binding Color}""
                        ClipToBounds=""True"">
                    <Grid>
                        <pf:PackIcon Kind=""{Binding Icon}""
                                     Width=""64"" Height=""64""
                                     Foreground=""White"" Opacity=""0.15""
                                     HorizontalAlignment=""Center""
                                     VerticalAlignment=""Center"" />
                        <StackPanel VerticalAlignment=""Bottom"" Margin=""10,0,10,10"">
                            <TextBlock Text=""{Binding Title}"" FontWeight=""Bold""
                                       FontSize=""12"" Foreground=""White"" />
                            <TextBlock Text=""{Binding Subtitle}"" FontSize=""10""
                                       Foreground=""White"" Opacity=""0.75"" Margin=""0,2,0,0"" />
                        </StackPanel>
                    </Grid>
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</ScrollViewer>";
    }
}
