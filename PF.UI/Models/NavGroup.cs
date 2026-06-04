using System.Collections.ObjectModel;
using PF.UI.Controls;
using Prism.Mvvm;

namespace PF.UI.Models
{
    /// <summary>
    /// 侧边栏导航分组
    /// </summary>
    public class NavGroup : BindableBase
    {
        public string Title { get; init; } = string.Empty;
        public PackIconKind Icon { get; init; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public ObservableCollection<NavItem> Children { get; init; } = new();
    }
}
