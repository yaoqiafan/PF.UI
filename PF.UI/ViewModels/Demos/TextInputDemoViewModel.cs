using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class TextInputDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "TextBox基础",  Title = "TextBox 基础",  Sub = "WatermarkTextBox / 图标 / 只读" },
            new DemoTocItem { Anchor = "TextBox扩展",  Title = "TextBox 扩展",  Sub = "TitleElement — Top / Left 布局" },
            new DemoTocItem { Anchor = "SearchBar",    Title = "SearchBar",     Sub = "回车触发 / 实时搜索" },
            new DemoTocItem { Anchor = "PasswordBox",  Title = "PasswordBox",   Sub = "眼睛切换 / 安全模式" },
            new DemoTocItem { Anchor = "PinBox",       Title = "PinBox",        Sub = "验证码 / PIN 输入" },
            new DemoTocItem { Anchor = "MultiLine",    Title = "多行文本",       Sub = "AcceptsReturn / 自动换行" },
        };

        private string _lastInputResult = "在下方控件中输入内容，这里将显示交互结果...";
        public string LastInputResult
        {
            get => _lastInputResult;
            set => SetProperty(ref _lastInputResult, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand LogCommand { get; }

        public TextInputDemoViewModel()
        {
            SearchCommand = new DelegateCommand<string>(keyword =>
                LastInputResult = $"搜索: \"{keyword}\"  ({System.DateTime.Now:HH:mm:ss})");

            LogCommand = new DelegateCommand<string>(msg =>
                LastInputResult = $"{msg}  ({System.DateTime.Now:HH:mm:ss})");
        }

        // ===== 代码示例 =====

        public const string XamlTextBoxBasic = @"<!-- WatermarkTextBox — 内置水印占位文字（TextBox 子类） -->
<pf:WatermarkTextBox Watermark=""请输入内容..."" />

<!-- 水印右对齐 -->
<pf:WatermarkTextBox Watermark=""右对齐水印""
                     HorizontalContentAlignment=""Right"" />

<!-- 只读 / 禁用 -->
<TextBox Text=""只读文本"" IsReadOnly=""True"" />
<TextBox Text=""已禁用"" IsEnabled=""False"" />

<!-- 小号变体 -->
<TextBox Style=""{StaticResource TextBox.Small}""
         pf:InfoElement.Placeholder=""小号输入框..."" />

<!-- 注意：pf:IconElement.Geometry 在 WatermarkTextBox / TextBox 上无效 -->
<!-- 如需图标输入框，请使用 pf:SearchBar（右侧搜索按钮） -->";

        public const string XamlTextBoxExtend = @"<!-- TextBoxExtend — 带浮动标题标签 -->

<!-- 默认布局 (Left) -->
<TextBox Style=""{StaticResource TextBoxExtend}""
         pf:TitleElement.Title=""用户名""
         pf:InfoElement.Placeholder=""请输入用户名"" />

<!-- Top 布局：标题显示在输入框顶部 -->
<TextBox Style=""{StaticResource TextBoxExtend}""
         pf:TitleElement.Title=""邮箱""
         pf:TitleElement.TitlePlacement=""Top""
         pf:InfoElement.Placeholder=""user@example.com"" />

<!-- Left 布局（显式指定） -->
<TextBox Style=""{StaticResource TextBoxExtend}""
         pf:TitleElement.Title=""备注""
         pf:TitleElement.TitlePlacement=""Left""
         pf:InfoElement.Placeholder=""请输入备注"" />";

        public const string XamlSearchBar = @"<!-- SearchBar — 按 Enter 或点击图标触发 Command -->
<pf:SearchBar pf:InfoElement.Placeholder=""输入关键字按 Enter 搜索...""
              Command=""{Binding SearchCommand}""
              CommandParameter=""{Binding Text,
                  RelativeSource={RelativeSource Self}}"" />

<!-- 实时搜索：IsRealTime=True，每次输入立即触发 -->
<pf:SearchBar pf:InfoElement.Placeholder=""实时搜索...""
              IsRealTime=""True""
              Command=""{Binding SearchCommand}""
              CommandParameter=""{Binding Text,
                  RelativeSource={RelativeSource Self}}"" />

<!-- 带标题的扩展样式 -->
<pf:SearchBar Style=""{StaticResource SearchBarExtend}""
              pf:TitleElement.Title=""关键字""
              pf:TitleElement.TitlePlacement=""Top""
              pf:InfoElement.Placeholder=""搜索..."" />";

        public const string XamlPasswordBox = @"<!-- pf:PasswordBox — 自定义密码框（非原生 PasswordBox） -->

<!-- ShowEyeButton=True 显示/隐藏切换按钮 -->
<pf:PasswordBox pf:InfoElement.Placeholder=""请输入密码""
                ShowEyeButton=""True"" />

<!-- IsSafeEnabled=False 关闭安全模式，可双向绑定 UnsafePassword -->
<pf:PasswordBox pf:InfoElement.Placeholder=""可绑定密码""
                IsSafeEnabled=""False""
                ShowEyeButton=""True""
                UnsafePassword=""{Binding Password, Mode=TwoWay}"" />

<!-- 小号变体（注意：用 PasswordBoxPlus.Small，不是 PasswordBoxExtend） -->
<pf:PasswordBox Style=""{StaticResource PasswordBoxPlus.Small}""
                pf:InfoElement.Placeholder=""小号密码框""
                ShowEyeButton=""True"" />";

        public const string XamlPinBox = @"<!-- PinBox — 验证码 / PIN 分格输入 -->
<pf:PinBox Length=""6"" />

<!-- 密码模式（PasswordChar 替换显示字符） -->
<pf:PinBox Length=""4"" PasswordChar=""●"" />

<!-- 自定义单格尺寸与间距 -->
<pf:PinBox Length=""6""
           ItemWidth=""44"" ItemHeight=""48"" ItemMargin=""4,0"" />";

        public const string XamlMultiLine = @"<!-- 多行 TextBox -->
<TextBox AcceptsReturn=""True""
         TextWrapping=""Wrap""
         Height=""120""
         VerticalScrollBarVisibility=""Auto""
         pf:InfoElement.Placeholder=""请输入多行文本..."" />

<!-- WatermarkTextBox 多行 -->
<pf:WatermarkTextBox AcceptsReturn=""True""
                     TextWrapping=""Wrap""
                     Height=""120""
                     VerticalScrollBarVisibility=""Auto""
                     Watermark=""支持多行的水印文本框..."" />";
    }
}
