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

        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(nameof(Language), typeof(string), typeof(DemoCodeBlock),
                new PropertyMetadata("XAML", OnLanguageChanged));

        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public DemoCodeBlock()
        {
            InitializeComponent();
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (DemoCodeBlock)d;
            ctl.CodeText.Text = e.NewValue as string ?? string.Empty;
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
    }
}
