using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PF.UI.Controls;
using PF.UI.ViewModels.Demos;

namespace PF.UI.Views.Demos
{
    public partial class MediaDemoView : UserControl
    {
        private Dictionary<string, FrameworkElement>? _anchors;
        private Dictionary<string, DemoTocItem>? _tocMap;
        private bool _navigating;

        private static readonly (string Hex, string Title, PackIconKind Icon)[] _coverFlowData =
        {
            ("#1565C0", "PLC 控制器",  PackIconKind.Chip),
            ("#2E7D32", "伺服驱动器",  PackIconKind.CogOutline),
            ("#E65100", "触摸屏 HMI",  PackIconKind.Monitor),
            ("#6A1B9A", "传感器套件",  PackIconKind.Radar),
            ("#00838F", "工业相机",    PackIconKind.Camera),
            ("#C62828", "SCADA 软件",  PackIconKind.Database),
            ("#558B2F", "气动夹爪",    PackIconKind.Wrench),
        };

        public MediaDemoView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _anchors = new Dictionary<string, FrameworkElement>
            {
                ["CoverView"]    = Section_CoverView,
                ["CoverFlow"]    = Section_CoverFlow,
                ["ImageGallery"] = Section_ImageGallery,
            };
            _tocMap = new Dictionary<string, DemoTocItem>();
            if (DataContext is MediaDemoViewModel vm)
            {
                foreach (var item in vm.TocItems)
                    _tocMap[item.Anchor] = item;
                if (vm.TocItems.Count > 0)
                    vm.TocItems[0].IsActive = true;
            }

            InitCoverFlow();
            UpdateActiveToc();
        }

        private void InitCoverFlow()
        {
            var converter = new BrushConverter();
            var cards = new List<object>();

            foreach (var (hex, title, icon) in _coverFlowData)
            {
                var bg = (Brush)(converter.ConvertFromString(hex) ?? Brushes.Gray);
                var card = new Border
                {
                    Width = 220,
                    Height = 160,
                    Background = bg,
                    CornerRadius = new CornerRadius(6),
                    ClipToBounds = true,
                };
                var grid = new Grid();
                var ico = new PackIcon
                {
                    Kind = icon,
                    Width = 72,
                    Height = 72,
                    Foreground = Brushes.White,
                    Opacity = 0.18,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 8, 4),
                };
                var txt = new TextBlock
                {
                    Text = title,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(12, 0, 8, 10),
                };
                grid.Children.Add(ico);
                grid.Children.Add(txt);
                card.Child = grid;
                cards.Add(card);
            }

            DemoCoverFlow.AddRange(cards);
            UpdateCoverIndexText();
        }

        private void UpdateCoverIndexText()
        {
            TxtCoverIndex.Text = $"{DemoCoverFlow.PageIndex + 1} / {_coverFlowData.Length}";
        }

        private void BtnPrevCover_Click(object sender, RoutedEventArgs e)
        {
            DemoCoverFlow.Prev();
            UpdateCoverIndexText();
        }

        private void BtnNextCover_Click(object sender, RoutedEventArgs e)
        {
            DemoCoverFlow.Next();
            UpdateCoverIndexText();
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
            if (_navigating || _anchors == null || DataContext is not MediaDemoViewModel) return;
            UpdateActiveToc();
        }

        private void UpdateActiveToc()
        {
            if (_anchors == null || DataContext is not MediaDemoViewModel) return;
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
