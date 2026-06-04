using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PF.UI.Controls
{
    public partial class DemoCodeBlock : UserControl
    {
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(nameof(Code), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata(string.Empty, OnCodeChanged));

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public static readonly DependencyProperty CodeLanguageProperty =
            DependencyProperty.Register(nameof(CodeLanguage), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata("XAML", OnLanguageChanged));

        public string CodeLanguage
        {
            get => (string)GetValue(CodeLanguageProperty);
            set => SetValue(CodeLanguageProperty, value);
        }

        public ObservableCollection<CodeLine> Lines { get; } = new();

        public DemoCodeBlock()
        {
            InitializeComponent();
            LinesItemsControl.ItemsSource = Lines;
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (DemoCodeBlock)d;
            ctl.Lines.Clear();
            var text = e.NewValue as string;
            if (string.IsNullOrEmpty(text))
            {
                ctl.Lines.Add(new CodeLine { LineNumber = 1, Text = string.Empty });
                return;
            }

            var rawLines = text.Replace("\r\n", "\n").Split('\n');
            for (int i = 0; i < rawLines.Length; i++)
            {
                ctl.Lines.Add(new CodeLine
                {
                    LineNumber = i + 1,
                    Text = rawLines[i]
                });
            }
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (DemoCodeBlock)d;
            ctl.LanguageLabel.Text = e.NewValue as string ?? "XAML";
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Code ?? string.Empty);
            CopyLabel.Text = "✅ 已复制";
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, args) =>
            {
                CopyLabel.Text = "复制";
                timer.Stop();
            };
            timer.Start();
        }

        private void LineRow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0x2A, 0x2A, 0x2A));
                // Find the copy button in the template
                var grid = border.Child as Grid;
                if (grid != null && grid.Children.Count > 2)
                {
                    var btn = grid.Children[2] as Button;
                    if (btn != null) btn.Visibility = Visibility.Visible;
                }
            }
        }

        private void LineRow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Colors.Transparent);
                var grid = border.Child as Grid;
                if (grid != null && grid.Children.Count > 2)
                {
                    var btn = grid.Children[2] as Button;
                    if (btn != null) btn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LineCopy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string lineText)
            {
                Clipboard.SetText(lineText);
                // Brief visual feedback on the button
                if (btn.Content is UIElement oldContent)
                {
                    var check = new PF.UI.Controls.PackIcon
                    {
                        Kind = PackIconKind.Check,
                        Width = 11,
                        Height = 11
                    };
                    // Use a simple approach: change the icon color briefly
                    btn.ToolTip = "✅ 已复制";
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                    timer.Tick += (s, args) =>
                    {
                        btn.ToolTip = "复制此行";
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
        }
    }

    public class CodeLine
    {
        public int LineNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
