using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace PF.UI.Controls
{
    public partial class DemoCodeBlock : UserControl
    {
        #region Code DP
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(nameof(Code), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata(string.Empty, OnCodeChanged));

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }
        #endregion

        #region CodeLanguage DP
        public static readonly DependencyProperty CodeLanguageProperty =
            DependencyProperty.Register(nameof(CodeLanguage), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata("XAML", OnLanguageChanged));

        public string CodeLanguage
        {
            get => (string)GetValue(CodeLanguageProperty);
            set => SetValue(CodeLanguageProperty, value);
        }
        #endregion

        #region LineNumbersText DP（只读，由 Code 计算）
        private static readonly DependencyPropertyKey LineNumbersTextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LineNumbersText), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LineNumbersTextProperty = LineNumbersTextPropertyKey.DependencyProperty;

        public string LineNumbersText
        {
            get => (string)GetValue(LineNumbersTextProperty);
            private set => SetValue(LineNumbersTextPropertyKey, value);
        }
        #endregion

        private static readonly FontFamily _consolas = new FontFamily("Consolas");
        private static readonly Brush _foregroundBrush = new SolidColorBrush(Color.FromRgb(0xD4, 0xD4, 0xD4));
        private static readonly Brush _lineNumBrush = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
        private static readonly Brush _selectionBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x4F, 0x78));

        private bool _contentCreated;

        public DemoCodeBlock()
        {
            InitializeComponent();
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var block = (DemoCodeBlock)d;
            var text = (e.NewValue as string) ?? string.Empty;

            // 统一换行符，若有变化回写（第二次进入时已是 \n，不再触发）
            var normalized = text.Replace("\r\n", "\n");
            if (text != normalized)
            {
                block.Code = normalized;
                return;
            }

            var lineCount = string.IsNullOrEmpty(normalized) ? 1 : normalized.Split('\n').Length;
            block.LineNumbersText = string.Join("\n",
                Enumerable.Range(1, lineCount).Select(i => i.ToString("D2")));
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DemoCodeBlock)d).LanguageLabel.Text = (e.NewValue as string) ?? "XAML";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var expanding = CodeContent.Visibility != Visibility.Visible;

            if (expanding && !_contentCreated)
            {
                CodeContent.Content = BuildCodeArea();
                _contentCreated = true;
            }

            CodeContent.Visibility = expanding ? Visibility.Visible : Visibility.Collapsed;
            HeaderBorder.BorderThickness = expanding ? new Thickness(0, 0, 0, 1) : new Thickness(0);
            ToggleLabel.Text = expanding ? "折叠" : "展开";
            ToggleButton.ToolTip = expanding ? "折叠代码" : "展开代码";
            ToggleIcon.Kind = expanding ? PackIconKind.ChevronUp : PackIconKind.ChevronDown;
        }

        private UIElement BuildCodeArea()
        {
            // 行号：TextBlock，无需交互
            var lineNumbers = new TextBlock
            {
                Width = 30,
                Margin = new Thickness(0, 0, 10, 0),
                FontFamily = _consolas,
                FontSize = 12.5,
                Foreground = _lineNumBrush,
                TextAlignment = TextAlignment.Right
            };
            lineNumbers.SetBinding(TextBlock.TextProperty,
                new Binding(nameof(LineNumbersText)) { Source = this });

            // 代码：只读 TextBox，保留鼠标拖选 / Ctrl+A / Ctrl+C
            var codeBox = new TextBox
            {
                AcceptsReturn = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                CaretBrush = Brushes.White,
                FontFamily = _consolas,
                FontSize = 12.5,
                Foreground = _foregroundBrush,
                IsReadOnly = true,
                Padding = new Thickness(0),
                SelectionBrush = _selectionBrush,
                SelectionOpacity = 0.6,
                TextWrapping = TextWrapping.NoWrap,
                Style = null
            };
            ScrollViewer.SetHorizontalScrollBarVisibility(codeBox, ScrollBarVisibility.Disabled);
            ScrollViewer.SetVerticalScrollBarVisibility(codeBox, ScrollBarVisibility.Disabled);
            codeBox.SetBinding(TextBox.TextProperty,
                new Binding(nameof(Code)) { Source = this });

            var grid = new Grid { Margin = new Thickness(8) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetColumn(lineNumbers, 0);
            Grid.SetColumn(codeBox, 1);
            grid.Children.Add(lineNumbers);
            grid.Children.Add(codeBox);

            return new ScrollViewer
            {
                MaxHeight = 300,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = grid
            };
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Code ?? string.Empty);
            CopyLabel.Text = "✅ 已复制";
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, args) => { CopyLabel.Text = "复制"; timer.Stop(); };
            timer.Start();
        }
    }
}
