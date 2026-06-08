using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        public DemoCodeBlock()
        {
            InitializeComponent();
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DemoCodeBlock)d).ApplyCode((e.NewValue as string) ?? string.Empty);
        }

        private void ApplyCode(string text)
        {
            if (CodeTextBox == null) return;
            var normalized = text.Replace("\r\n", "\n");
            var lineCount = string.IsNullOrEmpty(normalized) ? 1 : normalized.Split('\n').Length;
            CodeTextBox.Text = normalized;
            LineNumbersBox.Text = string.Join("\n",
                Enumerable.Range(1, lineCount).Select(i => i.ToString("D2")));
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DemoCodeBlock)d).LanguageLabel.Text = (e.NewValue as string) ?? "XAML";
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
