using System.Collections.ObjectModel;
using PF.UI.Controls;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class PackIconItem
    {
        public PackIconKind Kind { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public class PackIconDemoViewModel : BindableBase
    {
        private const int PageSize = 120;

        private readonly List<PackIconItem> _allIcons;
        private List<PackIconItem> _filteredIcons = [];

        public ObservableCollection<PackIconItem> Icons { get; } = new();

        // ─── 搜索文本 ─────────────────────────────────────────────────────

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); ApplyFilter(); }
        }

        // ─── 图标大小 ─────────────────────────────────────────────────────

        private int _iconSize = 32;
        public int IconSize
        {
            get => _iconSize;
            set => SetProperty(ref _iconSize, value);
        }

        // ─── 分页 ─────────────────────────────────────────────────────────

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                    RefreshCurrentPage();
            }
        }

        // MaxPageCount = 总页数（Pagination 用它夹住 PageIndex 上限）
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)_filteredIcons.Count / PageSize));

        public int ItemsPerPage => PageSize;

        // ─── 状态文字 ─────────────────────────────────────────────────────

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        // ─── 构造 ─────────────────────────────────────────────────────────

        public PackIconDemoViewModel()
        {
            var seen = new HashSet<int>();
            _allIcons = Enum.GetValues<PackIconKind>()
                .Where(k => seen.Add((int)k))
                .OrderBy(k => k.ToString())
                .Select(k => new PackIconItem { Kind = k, Name = k.ToString() })
                .ToList();

            ApplyFilter();
        }

        // ─── 过滤 & 分页逻辑 ──────────────────────────────────────────────

        private void ApplyFilter()
        {
            _filteredIcons = string.IsNullOrWhiteSpace(SearchText)
                ? _allIcons.ToList()
                : _allIcons.Where(i => i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            // 直接赋字段，避免 setter 触发 RefreshCurrentPage（后面统一刷新）
            _currentPage = 1;
            RaisePropertyChanged(nameof(CurrentPage));
            RaisePropertyChanged(nameof(TotalPages));
            RefreshCurrentPage();
        }

        private void RefreshCurrentPage()
        {
            Icons.Clear();
            var page = _filteredIcons
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize);
            foreach (var item in page)
                Icons.Add(item);

            UpdateStatusText();
        }

        private void UpdateStatusText()
        {
            var total = _filteredIcons.Count;
            var start = (_currentPage - 1) * PageSize + 1;
            var end = Math.Min(_currentPage * PageSize, total);
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)total / PageSize));

            StatusText = string.IsNullOrWhiteSpace(SearchText)
                ? $"显示 {start}–{end}  共 {total} 个图标  第 {_currentPage} / {totalPages} 页"
                : $"搜索 \"{SearchText}\"：{total} 个结果  显示 {start}–{end}  第 {_currentPage} / {totalPages} 页";
        }

        // ─── 代码示例 ──────────────────────────────────────────────────────

        public const string XamlUsage = @"<!-- XAML 使用 PackIcon -->
xmlns:pf=""clr-namespace:PF.UI.Controls;assembly=PF.UI.Controls""

<!-- 基本用法：Kind 枚举 + 任意颜色/尺寸 -->
<pf:PackIcon Kind=""Heart"" Width=""24"" Height=""24""
             Foreground=""{DynamicResource PrimaryBrush}"" />

<!-- 通过 x:Static 直接引用枚举（减少字符串匹配）-->
<pf:PackIcon Kind=""{x:Static pf:PackIconKind.Magnify}"" Width=""20"" Height=""20"" />

<!-- ViewModel 属性绑定（属性类型 PackIconKind）-->
<pf:PackIcon Kind=""{Binding CurrentIconKind}"" />

<!-- 嵌入 Button 使用 IconElement 附加属性（Button 模板支持）-->
<Button Content=""搜索""
        pf:IconElement.Geometry=""{StaticResource SearchGeometry}""
        pf:IconElement.Width=""16"" pf:IconElement.Height=""16"" />

<!-- C# 代码中按名称获取 Kind -->
var kind = Enum.Parse&lt;PackIconKind&gt;(""Heart"");";
    }
}
