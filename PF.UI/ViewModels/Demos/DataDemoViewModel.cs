using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class DataDemoViewModel : BindableBase
    {
        // ===== TOC =====
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "DataGrid",   Title = "DataGrid",   Sub = "多列表格 / 排序 / 选中" },
            new DemoTocItem { Anchor = "ListBox",    Title = "ListBox",    Sub = "单选/多选 / 自定义模板" },
            new DemoTocItem { Anchor = "ListView",   Title = "ListView",   Sub = "GridView 多列 / 纯列表" },
            new DemoTocItem { Anchor = "TreeView",   Title = "TreeView",   Sub = "递归树形节点" },
            new DemoTocItem { Anchor = "Pagination",    Title = "Pagination",    Sub = "分页 / 跳页" },
            new DemoTocItem { Anchor = "PropertyGrid",  Title = "PropertyGrid",  Sub = "属性网格 / 类型 / 搜索" },
        };

        // ===== 交互日志 =====
        private string _lastResult = "与下方控件交互，这里将显示操作结果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        public ICommand LogCommand { get; }

        // ===== DataGrid 数据源 =====
        public ObservableCollection<ProductItem> Products { get; } = new()
        {
            new ProductItem(1,  "PLC 控制器",   "硬件", 4880m,  true),
            new ProductItem(2,  "伺服驱动器",   "硬件", 3200m,  true),
            new ProductItem(3,  "触摸屏 HMI",   "硬件", 6500m,  false),
            new ProductItem(4,  "传感器套件",   "配件", 980m,   true),
            new ProductItem(5,  "工业相机",     "视觉", 12800m, true),
            new ProductItem(6,  "SCADA 软件",   "软件", 28000m, false),
            new ProductItem(7,  "气动夹爪",     "执行", 2100m,  true),
            new ProductItem(8,  "导轨滑块",     "机械", 560m,   true),
        };

        // ===== ListBox / ListView 数据源 =====
        public ObservableCollection<string> Stations { get; } = new()
        {
            "装配站 01", "检测站 02", "分拣站 03",
            "包装站 04", "标签站 05", "出料站 06",
        };

        // ===== TreeView 数据源 =====
        public ObservableCollection<TreeNodeItem> TreeRoots { get; }

        // ===== Pagination =====
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (SetProperty(ref _pageIndex, value))
                    LastResult = $"翻至第 {value} 页  ({System.DateTime.Now:HH:mm:ss})";
            }
        }

        public int TotalCount { get; } = 256;
        public int PageSize  { get; } = 20;

        public DataDemoViewModel()
        {
            LogCommand = new DelegateCommand<string>(msg =>
                LastResult = $"{msg}  ({System.DateTime.Now:HH:mm:ss})");

            TreeRoots = new ObservableCollection<TreeNodeItem>
            {
                new TreeNodeItem("产线 A")
                {
                    Children =
                    {
                        new TreeNodeItem("装配区")
                        {
                            Children =
                            {
                                new TreeNodeItem("工位 A-01"),
                                new TreeNodeItem("工位 A-02"),
                                new TreeNodeItem("工位 A-03"),
                            }
                        },
                        new TreeNodeItem("检测区")
                        {
                            Children =
                            {
                                new TreeNodeItem("视觉检测"),
                                new TreeNodeItem("尺寸检测"),
                            }
                        },
                    }
                },
                new TreeNodeItem("产线 B")
                {
                    Children =
                    {
                        new TreeNodeItem("包装区")
                        {
                            Children =
                            {
                                new TreeNodeItem("工位 B-01"),
                                new TreeNodeItem("工位 B-02"),
                            }
                        },
                        new TreeNodeItem("出料区"),
                    }
                },
                new TreeNodeItem("公共设施")
                {
                    Children =
                    {
                        new TreeNodeItem("压缩空气站"),
                        new TreeNodeItem("配电室"),
                        new TreeNodeItem("工控机房"),
                    }
                },
            };
        }

        // ===== 代码示例 =====

        public const string XamlDataGrid = @"<DataGrid ItemsSource=""{Binding Products}""
          AutoGenerateColumns=""False""
          CanUserSortColumns=""True""
          GridLinesVisibility=""Horizontal""
          SelectionMode=""Single"">
    <DataGrid.Columns>
        <DataGridTextColumn Header=""ID""     Binding=""{Binding Id}""       Width=""50"" />
        <DataGridTextColumn Header=""名称""   Binding=""{Binding Name}""     Width=""*"" />
        <DataGridTextColumn Header=""分类""   Binding=""{Binding Category}"" Width=""80"" />
        <DataGridTextColumn Header=""单价""   Binding=""{Binding Price, StringFormat=¥\{0:N0\}}"" Width=""100"" />
        <DataGridCheckBoxColumn Header=""库存"" Binding=""{Binding InStock}"" Width=""60"" />
    </DataGrid.Columns>
</DataGrid>";

        public const string XamlListBox = @"<!-- 单选 ListBox（默认） -->
<ListBox ItemsSource=""{Binding Stations}"" />

<!-- 多选 -->
<ListBox ItemsSource=""{Binding Stations}""
         SelectionMode=""Multiple"" />

<!-- 自定义 ItemTemplate -->
<ListBox ItemsSource=""{Binding Stations}"">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation=""Horizontal"" Margin=""4,2"">
                <pf:PackIcon Kind=""FactoryIcon"" Width=""14"" Height=""14""
                             Foreground=""{DynamicResource PrimaryBrush}""
                             VerticalAlignment=""Center"" Margin=""0,0,8,0"" />
                <TextBlock Text=""{Binding}"" VerticalAlignment=""Center"" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>";

        public const string XamlListView = @"<ListView ItemsSource=""{Binding Products}"">
    <ListView.View>
        <GridView>
            <GridViewColumn Header=""名称""   DisplayMemberBinding=""{Binding Name}""     Width=""160"" />
            <GridViewColumn Header=""分类""   DisplayMemberBinding=""{Binding Category}"" Width=""80"" />
            <GridViewColumn Header=""单价""   DisplayMemberBinding=""{Binding Price, StringFormat=¥\{0:N0\}}"" Width=""100"" />
        </GridView>
    </ListView.View>
</ListView>";

        public const string XamlListViewSimple = @"<!-- ListView 不设置 View 时退化为纯列表，使用 ListViewItem 容器 -->
<ListView ItemsSource=""{Binding Stations}"" />

<!-- 自定义 ItemTemplate -->
<ListView ItemsSource=""{Binding Stations}"">
    <ListView.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation=""Horizontal"" Margin=""4,2"">
                <pf:PackIcon Kind=""Factory"" Width=""14"" Height=""14""
                             Foreground=""{DynamicResource PrimaryBrush}""
                             VerticalAlignment=""Center"" Margin=""0,0,8,0"" />
                <TextBlock Text=""{Binding}"" VerticalAlignment=""Center"" />
            </StackPanel>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>";

        public const string XamlTreeView = @"<!-- HierarchicalDataTemplate 递归绑定树形数据 -->
<TreeView ItemsSource=""{Binding TreeRoots}"">
    <TreeView.ItemTemplate>
        <HierarchicalDataTemplate ItemsSource=""{Binding Children}"">
            <TextBlock Text=""{Binding Name}"" />
        </HierarchicalDataTemplate>
    </TreeView.ItemTemplate>
</TreeView>";

        public const string XamlPagination = @"<!-- Pagination：MaxPageCount 由 DataCount ÷ PageSize 决定 -->
<!-- Height=""30"" 约束内部跳页 NumericUpDown 高度 -->
<pf:Pagination MaxPageCount=""{Binding TotalCount}""
               DataCountPerPage=""{Binding PageSize}""
               PageIndex=""{Binding PageIndex, Mode=TwoWay}""
               IsJumpEnabled=""True""
               Height=""30"" />";

        public const string XamlPropertyGrid = @"<!-- PropertyGrid — 绑定任意 POCO 对象，自动反射所有可浏览属性 -->
<pf:PropertyGrid SelectedObject=""{Binding DemoObject}""
                 ShowSortButton=""True"" />

<!-- 控制显示的属性：使用标准 .NET Attribute -->
[Category(""基本信息"")]
[DisplayName(""产品名称"")]
[Description(""产品的显示名称"")]
public string Name { get; set; }

[Browsable(false)]   // 隐藏此属性
public string InternalId { get; set; }

[ReadOnly(true)]     // 只读显示
public DateTime CreateTime { get; set; }

<!-- PropertyGrid 内置 Editor 类型：
     string       → TextBox
     int/double   → NumericUpDown
     bool         → CheckBox / ToggleButton
     Enum         → ComboBox（自动枚举所有值）
     Color        → ColorPicker
     DateTime     → DateTimePicker -->";

        // PropertyGrid 演示对象（公开属性供绑定）
        public DemoPropertyObject DemoObject { get; } = new();
    }

    public class ProductItem
    {
        public int     Id       { get; }
        public string  Name     { get; }
        public string  Category { get; }
        public decimal Price    { get; }
        public bool    InStock  { get; }

        public ProductItem(int id, string name, string category, decimal price, bool inStock)
        {
            Id = id; Name = name; Category = category; Price = price; InStock = inStock;
        }
    }

    public class TreeNodeItem
    {
        public string Name { get; }
        public ObservableCollection<TreeNodeItem> Children { get; } = new();
        public TreeNodeItem(string name) => Name = name;
    }

    public enum DemoStatus { Active, Paused, Stopped, Error }
    public enum DemoAlignment { Left, Center, Right, Stretch }

    [System.ComponentModel.DisplayName("设备配置演示对象")]
    public class DemoPropertyObject
    {
        // ─── 基本信息 ───────────────────────────────────────────────
        [System.ComponentModel.Category("基本信息")]
        [System.ComponentModel.DisplayName("设备名称")]
        [System.ComponentModel.Description("设备的显示名称")]
        public string Name { get; set; } = "传感器 A1";

        [System.ComponentModel.Category("基本信息")]
        [System.ComponentModel.DisplayName("设备编号")]
        [System.ComponentModel.Description("工厂内唯一编号")]
        public string DeviceId { get; set; } = "SN-2024-001";

        [System.ComponentModel.Category("基本信息")]
        [System.ComponentModel.DisplayName("启用")]
        public bool IsEnabled { get; set; } = true;

        [System.ComponentModel.Category("基本信息")]
        [System.ComponentModel.DisplayName("运行状态")]
        public DemoStatus Status { get; set; } = DemoStatus.Active;

        // ─── 数值参数 ───────────────────────────────────────────────
        [System.ComponentModel.Category("数值参数")]
        [System.ComponentModel.DisplayName("采样间隔 (ms)")]
        [System.ComponentModel.Description("数据采集周期，单位毫秒")]
        public int SampleInterval { get; set; } = 200;

        [System.ComponentModel.Category("数值参数")]
        [System.ComponentModel.DisplayName("报警阈值")]
        public double AlarmThreshold { get; set; } = 85.5;

        [System.ComponentModel.Category("数值参数")]
        [System.ComponentModel.DisplayName("最大重试次数")]
        public int MaxRetries { get; set; } = 3;

        // ─── 显示设置 ───────────────────────────────────────────────
        [System.ComponentModel.Category("显示设置")]
        [System.ComponentModel.DisplayName("对齐方式")]
        public DemoAlignment Alignment { get; set; } = DemoAlignment.Center;

        [System.ComponentModel.Category("显示设置")]
        [System.ComponentModel.DisplayName("描述文字")]
        public string Description { get; set; } = "一楼生产线左侧温度传感器";

        // ─── 只读/隐藏 ──────────────────────────────────────────────
        [System.ComponentModel.Category("只读信息")]
        [System.ComponentModel.DisplayName("创建时间")]
        [System.ComponentModel.ReadOnly(true)]
        public string CreateTime { get; set; } = "2024-01-15 09:30";

        [System.ComponentModel.Browsable(false)]
        public string InternalKey { get; set; } = "HIDDEN";
    }
}
