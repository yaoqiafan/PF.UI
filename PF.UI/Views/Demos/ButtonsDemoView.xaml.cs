using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class ButtonsDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        public ButtonsDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["基础按钮"] = Section_基础按钮,
                ["图标按钮"] = Section_图标按钮,
                ["切换按钮"] = Section_切换按钮,
                ["复合按钮"] = Section_复合按钮,
            };

            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is ButtonsDemoViewModel vm)
            {
                foreach (var item in vm.TocItems)
                    _tocMap[item.Anchor] = item;
                // 默认高亮第一条
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

                // 额外偏移一点让它不要紧贴顶部
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ContentScroll.ScrollToVerticalOffset(ContentScroll.VerticalOffset - 8);
                    _navigating = false;
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void ContentScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_navigating || _anchors == null || DataContext is not ButtonsDemoViewModel vm)
                return;

            UpdateActiveToc();
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not ButtonsDemoViewModel vm)
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

            // 清除所有高亮
            foreach (var kv in _tocMap)
                kv.Value.IsActive = false;

            // 设置当前
            if (_tocMap.TryGetValue(anchor, out var active))
                active.IsActive = true;
        }
    }
}
