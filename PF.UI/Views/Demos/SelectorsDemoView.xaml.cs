using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class SelectorsDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        public SelectorsDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["ComboBox基础"]   = Section_ComboBox基础,
                ["ComboBox扩展"]   = Section_ComboBox扩展,
                ["CheckComboBox"]  = Section_CheckComboBox,
                ["SearchComboBox"] = Section_SearchComboBox,
                ["AutoComplete"]   = Section_AutoComplete,
                ["NumericUpDown"]  = Section_NumericUpDown,
                ["ImageSelector"]  = Section_ImageSelector,
            };

            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is SelectorsDemoViewModel vm)
            {
                foreach (var item in vm.TocItems)
                    _tocMap[item.Anchor] = item;
                if (vm.TocItems.Count > 0)
                    vm.TocItems[0].IsActive = true;
            }

            UpdateActiveToc();
        }

        private void TocItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is DemoTocItem toc
                && _anchors != null && _anchors.TryGetValue(toc.Anchor, out var target))
            {
                _navigating = true;
                target.BringIntoView();
                SetActiveToc(toc.Anchor);

                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ContentScroll.ScrollToVerticalOffset(ContentScroll.VerticalOffset - 8);
                    _navigating = false;
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void ContentScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_navigating || _anchors == null || DataContext is not SelectorsDemoViewModel)
                return;
            UpdateActiveToc();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not SelectorsDemoViewModel vm) return;
            if (sender is ComboBox cb && cb.SelectedItem is string selected)
                vm.LogCommand.Execute($"ComboBox 选中: {selected}");
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not SelectorsDemoViewModel vm)
                return;

            var scrollCenter = ContentScroll.VerticalOffset + ContentScroll.ViewportHeight * 0.3;
            string? bestAnchor = null;

            foreach (var kv in _anchors)
            {
                var transform = kv.Value.TransformToVisual(ContentScroll.Content as UIElement);
                var relY = transform.Transform(new Point(0, 0)).Y;
                if (relY <= scrollCenter)
                    bestAnchor = kv.Key;
            }

            if (bestAnchor != null)
                SetActiveToc(bestAnchor);
        }

        private void SetActiveToc(string anchor)
        {
            if (_tocMap == null) return;
            foreach (var kv in _tocMap)
                kv.Value.IsActive = false;
            if (_tocMap.TryGetValue(anchor, out var active))
                active.IsActive = true;
        }
    }
}
