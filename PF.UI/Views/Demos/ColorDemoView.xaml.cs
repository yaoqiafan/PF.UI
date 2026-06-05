using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class ColorDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        public ColorDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["ThemeBrushes"] = Section_ThemeBrushes,
                ["ColorPicker"]  = Section_ColorPicker,
                ["HSB"]          = Section_HSB,
                ["Palette"]      = Section_Palette,
            };
            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is ColorDemoViewModel vm)
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
            if (_navigating || _anchors == null || DataContext is not ColorDemoViewModel) return;
            UpdateActiveToc();
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not ColorDemoViewModel) return;
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

        // HueSliderPosition RadioButton 事件（保持 code-behind 模式，避免 Command 干扰）
        private ColorDemoViewModel? VM => DataContext as ColorDemoViewModel;
        private void BottomChecked(object sender, RoutedEventArgs e) { if (VM != null) VM.HueSliderPosition = Dock.Bottom; }
        private void TopChecked(object sender, RoutedEventArgs e)    { if (VM != null) VM.HueSliderPosition = Dock.Top; }
        private void LeftChecked(object sender, RoutedEventArgs e)   { if (VM != null) VM.HueSliderPosition = Dock.Left; }
        private void RightChecked(object sender, RoutedEventArgs e)  { if (VM != null) VM.HueSliderPosition = Dock.Right; }
    }
}
