using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private bool _contentCreated;

        public DemoCodeBlock()
        {
            InitializeComponent();
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var block = (DemoCodeBlock)d;
            var text = (e.NewValue as string) ?? string.Empty;

            // 统一换行符，如有变化回写（第二次进入时 text 已经是 \n，不再触发）
            var normalized = text.Replace("\r\n", "\n");
            if (!ReferenceEquals(text, normalized) && text != normalized)
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
                CodeContent.ContentTemplate = (DataTemplate)Resources["CodeAreaTemplate"];
                _contentCreated = true;
            }

            CodeContent.Visibility = expanding ? Visibility.Visible : Visibility.Collapsed;
            HeaderBorder.BorderThickness = expanding ? new Thickness(0, 0, 0, 1) : new Thickness(0);
            ToggleLabel.Text = expanding ? "折叠" : "展开";
            ToggleButton.ToolTip = expanding ? "折叠代码" : "展开代码";
            ToggleIcon.Kind = expanding ? PackIconKind.ChevronUp : PackIconKind.ChevronDown;
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
