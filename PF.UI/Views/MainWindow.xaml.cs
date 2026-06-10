using PF.UI.Controls;
using PF.UI.Models;
using PF.UI.Shared.Data;
using PF.UI.Shared.Tools;
using PF.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SwWindow = System.Windows.Window;

namespace PF.UI.Views
{
    public partial class MainWindow : PF.UI.Controls.Window
    {
        private bool _isDarkTheme;
        private bool _isAnimating;

        private static readonly string ThemeFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PF.UI", "theme.txt");

        public MainWindow()
        {
            // 在 InitializeComponent 之前加载持久化的主题
            var saved = ReadSavedTheme();
            _isDarkTheme = saved == "Dark";
            ((App)Application.Current).UpdateSkin(saved ?? "Default");

            InitializeComponent();
            ThemeIcon.Kind = _isDarkTheme ? PackIconKind.WeatherSunny : PackIconKind.WeatherNight;

            // Prism 自动装配的 DataContext 在 InitializeComponent 期间已设置
            if (DataContext is MainWindowViewModel vm)
                BuildNavMenu(vm);
            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is MainWindowViewModel newVm)
                    BuildNavMenu(newVm);
            };
        }

        // ─── 主题切换 ─────────────────────────────────────────────────────

        private void ToggleThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var win = GetWindow(this);
            var w = (int)Math.Max(win.ActualWidth, 1);
            var h = (int)Math.Max(win.ActualHeight, 1);

            // 1. 快照整个窗口（旧主题）
            var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(win);

            // 2. 计算圆心（从主题按钮位置扩散）
            var btnPos = ThemeBtn.TransformToAncestor(win).Transform(new Point(0, 0));
            var cx = btnPos.X + ThemeBtn.ActualWidth / 2;
            var cy = btnPos.Y + ThemeBtn.ActualHeight / 2;

            // 3. 创建独立透明窗口作为覆盖层（不受主窗口资源更新影响）
            var overlayImg = new Image
            {
                Source = rtb,
                Width = w,
                Height = h,
                Stretch = Stretch.None
            };
            var fullRect = new RectangleGeometry(new Rect(0, 0, w, h));
            var hole = new EllipseGeometry { Center = new Point(cx, cy), RadiusX = 0, RadiusY = 0 };
            overlayImg.Clip = new CombinedGeometry(GeometryCombineMode.Exclude, fullRect, hole);

            var overlayWin = new SwWindow
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                Width = w,
                Height = h,
                Left = win.Left,
                Top = win.Top,
                Content = overlayImg,
                Focusable = false
            };
            overlayWin.Show();

            // 4. 切换主题（覆盖层已完全遮蔽主窗口，用户看不到切换过程）
            _isDarkTheme = !_isDarkTheme;
            var name = _isDarkTheme ? "Dark" : "Default";
            ((App)Application.Current).UpdateSkin(name);
            SaveTheme(name);
            ThemeIcon.Kind = _isDarkTheme ? PackIconKind.WeatherSunny : PackIconKind.WeatherNight;


            // 5. 与渲染管线同步，每帧更新椭圆半径
            var maxR = Math.Sqrt(w * w + h * h) + 20;
            var sw = Stopwatch.StartNew();
            const int durationMs = 600;

            EventHandler renderHandler = null!;
            renderHandler = (_, _) =>
            {
                var t = Math.Min(sw.ElapsedMilliseconds / (double)durationMs, 1.0);
                double eased = t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
                hole.RadiusX = maxR * eased;
                hole.RadiusY = maxR * eased;

                if (t >= 1.0)
                {
                    CompositionTarget.Rendering -= renderHandler;
                    overlayWin.Close();
                    _isAnimating = false;
                }
            };
            CompositionTarget.Rendering += renderHandler;
        }

        // ─── 主题持久化 ──────────────────────────────────────────────────

        private static string? ReadSavedTheme()
        {
            try
            {
                return File.Exists(ThemeFilePath) ? File.ReadAllText(ThemeFilePath).Trim() : null;
            }
            catch { return null; }
        }

        private static void SaveTheme(string name)
        {
            try
            {
                var dir = Path.GetDirectoryName(ThemeFilePath);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(ThemeFilePath, name);
            }
            catch { }
        }

        // ─── 侧边栏 ───────────────────────────────────────────────────────

        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            NavDrawer.IsOpen = !NavDrawer.IsOpen;
        }

        private void NavDrawer_Opened(object sender, RoutedEventArgs e)
        {
            HamburgerIcon.Kind = PackIconKind.MenuOpen;
            NavSearchBar.Focus();
        }

        private void NavDrawer_Closed(object sender, RoutedEventArgs e)
        {
            HamburgerIcon.Kind = PackIconKind.Menu;
            NavSearchBar.Text = string.Empty;
        }

        // ─── 导航菜单构建与搜索过滤 ──────────────────────────────────────

        private sealed record NavMenuEntry(SideMenuItem Header, NavGroup Group,
            List<(SideMenuItem Item, NavItem Nav)> Children);

        private readonly List<NavMenuEntry> _navIndex = new();

        private void BuildNavMenu(MainWindowViewModel vm)
        {
            NavMenu.Items.Clear();
            _navIndex.Clear();

            foreach (var group in vm.NavGroups)
            {
                var header = new SideMenuItem
                {
                    Header = group.Title,
                    DataContext = group,
                    Icon = CreateIcon(group.Icon, 18)
                };
                // 组展开状态与模型双向同步（VM 导航时自动展开所在组）
                BindingOperations.SetBinding(header, SideMenuItem.IsExpandedProperty,
                    new Binding(nameof(NavGroup.IsExpanded)) { Source = group, Mode = BindingMode.TwoWay });

                var children = new List<(SideMenuItem, NavItem)>();
                foreach (var nav in group.Children)
                {
                    var item = new SideMenuItem
                    {
                        Header = nav.Title,
                        DataContext = nav,
                        Icon = CreateIcon(nav.Icon, 16),
                        ToolTip = nav.Description
                    };
                    header.Items.Add(item);
                    children.Add((item, nav));
                }

                NavMenu.Items.Add(header);
                _navIndex.Add(new NavMenuEntry(header, group, children));
            }

            // 模板应用完成后按模型状态还原各组初始展开
            Dispatcher.InvokeAsync(() =>
            {
                foreach (var entry in _navIndex)
                    entry.Header.SwitchPanelArea(entry.Group.IsExpanded);
            }, DispatcherPriority.Loaded);
        }

        private static PackIcon CreateIcon(PackIconKind kind, double size) => new()
        {
            Kind = kind,
            Width = size,
            Height = size,
            VerticalAlignment = VerticalAlignment.Center
        };

        private void NavSearchBar_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyNavFilter(NavSearchBar.Text);

        private void ApplyNavFilter(string? text)
        {
            var keyword = text?.Trim() ?? string.Empty;
            var searching = keyword.Length > 0;

            foreach (var (header, group, children) in _navIndex)
            {
                var headerMatch = searching && Match(group.Title, keyword);
                var anyChildVisible = false;

                foreach (var (item, nav) in children)
                {
                    // 组标题命中时显示组内全部子项，否则按子项标题/描述过滤
                    var visible = !searching || headerMatch ||
                                  Match(nav.Title, keyword) || Match(nav.Description, keyword);
                    item.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                    anyChildVisible |= visible;
                }

                var headerVisible = !searching || headerMatch || anyChildVisible;
                header.Visibility = headerVisible ? Visibility.Visible : Visibility.Collapsed;

                // 搜索时自动展开命中的组；清空时还原为仅展开选中项所在组
                header.SwitchPanelArea(searching ? headerVisible : ContainsSelected(group));
            }

            static bool Match(string source, string keyword) =>
                source.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        private bool ContainsSelected(NavGroup group) =>
            DataContext is MainWindowViewModel { SelectedItem: { } selected } &&
            group.Children.Contains(selected);
    }
}
