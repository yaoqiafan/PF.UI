using Prism.Mvvm;
using PF.UI.Controls;

namespace PF.UI.Models
{
    public class NavItem : BindableBase
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string ViewName { get; init; } = string.Empty;
        public PackIconKind Icon { get; init; }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
    }
}
