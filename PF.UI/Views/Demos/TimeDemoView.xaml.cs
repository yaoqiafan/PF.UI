using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class TimeDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        public TimeDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["FlipClock"]     = Section_FlipClock,
                ["Clock"]         = Section_Clock,
                ["Pickers"]       = Section_Pickers,
                ["DateTimeRange"] = Section_DateTimeRange,
                ["ListClock"]     = Section_ListClock,
            };
            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is TimeDemoViewModel vm)
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
                { ContentScroll.ScrollToVerticalOffset(ContentScroll.VerticalOffset - 8); _navigating = false; }),
                    System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void ContentScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_navigating || _anchors == null || DataContext is not TimeDemoViewModel) return;
            UpdateActiveToc();
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not TimeDemoViewModel) return;
            var scrollCenter = ContentScroll.VerticalOffset + ContentScroll.ViewportHeight * 0.3;
            string? bestAnchor = null;
            foreach (var kv in _anchors)
            {
                var t = kv.Value.TransformToVisual(ContentScroll.Content as UIElement);
                if (t.Transform(new Point(0, 0)).Y <= scrollCenter)
                    bestAnchor = kv.Key;
            }
            if (bestAnchor != null) SetActiveToc(bestAnchor);
        }

        private void SetActiveToc(string anchor)
        {
            if (_tocMap == null) return;
            foreach (var kv in _tocMap)
                kv.Value.IsActive = false;
            if (_tocMap.TryGetValue(anchor, out var a))
                a.IsActive = true;
        }
    }
}
