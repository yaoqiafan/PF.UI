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
        private const int DefaultPageSize = 300;

        private readonly List<PackIconItem> _allIcons;

        public ObservableCollection<PackIconItem> Icons { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); ApplyFilter(); }
        }

        private int _iconSize = 32;
        public int IconSize
        {
            get => _iconSize;
            set => SetProperty(ref _iconSize, value);
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

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

        private void ApplyFilter()
        {
            Icons.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var item in _allIcons.Take(DefaultPageSize))
                    Icons.Add(item);
                StatusText = $"显示前 {Icons.Count} 个（共 {_allIcons.Count} 个）— 输入名称搜索全部";
            }
            else
            {
                foreach (var item in _allIcons.Where(i =>
                    i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    Icons.Add(item);
                StatusText = $"搜索 \"{SearchText}\"：找到 {Icons.Count} / {_allIcons.Count} 个图标";
            }
        }
    }
}
