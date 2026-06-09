using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class DataDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        public DataDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["DataGrid"]   = Section_DataGrid,
                ["ListBox"]    = Section_ListBox,
                ["ListView"]   = Section_ListView,
                ["TreeView"]   = Section_TreeView,
                ["Pagination"]    = Section_Pagination,
                ["PropertyGrid"]  = Section_PropertyGrid,
            };

            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is DataDemoViewModel vm)
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
            if (_navigating || _anchors == null || DataContext is not DataDemoViewModel)
                return;
            UpdateActiveToc();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not DataDemoViewModel vm) return;
            if (sender is DataGrid dg && dg.SelectedItem is ProductItem p)
                vm.LogCommand.Execute($"DataGrid 选中: {p.Name} — ¥{p.Price:N0}");
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not DataDemoViewModel vm) return;
            if (sender is ListBox lb && lb.SelectedItem is string s)
                vm.LogCommand.Execute($"ListBox 选中: {s}");
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not DataDemoViewModel vm) return;
            if (sender is ListView lv && lv.SelectedItem is ProductItem p)
                vm.LogCommand.Execute($"ListView 选中: {p.Name}");
        }

        private void ListView_Plain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not DataDemoViewModel vm) return;
            if (sender is ListView lv && lv.SelectedItem is string s)
                vm.LogCommand.Execute($"ListView (纯列表) 选中: {s}");
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is not DataDemoViewModel vm) return;
            if (e.NewValue is TreeNodeItem node)
                vm.LogCommand.Execute($"TreeView 选中: {node.Name}");
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not DataDemoViewModel vm)
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
