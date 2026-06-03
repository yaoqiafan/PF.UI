using PF.UI.Controls;

namespace PF.UI.Models
{
    public class NavItem
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string ViewName { get; init; } = string.Empty;
        public PackIconKind Icon { get; init; }
    }
}
