using PF.UI.Controls;
using PF.UI.Shared.Data;
using PF.UI.Shared.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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

        private void NavSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SidebarNav.FilterText = NavSearchBar.Text;
        }
    }
}
