using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PF.UI.ViewModels.Demos
{
    public class TextEffectsDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Highlight", Title = "HighlightTextBlock", Sub = "关键词高亮" },
            new DemoTocItem { Anchor = "Outline",   Title = "OutlineText",        Sub = "描边/空心文字" },
            new DemoTocItem { Anchor = "Simple",    Title = "SimpleText",         Sub = "轻量格式文本" },
        };

        // ─── HighlightTextBlock ────────────────────────────────────────

        private string _sourceText = "PF.UI 是一套面向 .NET 8 的企业级 WPF 控件库，提供 90+ 自定义控件和完整主题支持。";
        public string SourceText
        {
            get => _sourceText;
            set => SetProperty(ref _sourceText, value);
        }

        private string _queriesText = "控件";
        public string QueriesText
        {
            get => _queriesText;
            set => SetProperty(ref _queriesText, value);
        }

        // ─── 代码示例 ──────────────────────────────────────────────────

        public const string XamlHighlight = @"<!-- HighlightTextBlock — 关键词高亮文本块 -->
<!-- SourceText：原始文本；QueriesText：高亮关键词（支持空格分隔多词）-->
<pf:HighlightTextBlock
    SourceText=""{Binding SourceText}""
    QueriesText=""{Binding QueriesText}""
    HighlightBrush=""{DynamicResource LightPrimaryBrush}""
    HighlightTextBrush=""{DynamicResource PrimaryBrush}""
    TextWrapping=""Wrap"" />

<!-- 多关键词（空格分隔）-->
<pf:HighlightTextBlock SourceText=""Hello World from PF.UI""
                       QueriesText=""Hello UI""
                       HighlightBrush=""{DynamicResource LightDangerBrush}""
                       HighlightTextBrush=""{DynamicResource DangerBrush}"" />";

        public const string XamlOutline = @"<!-- OutlineText — 描边/空心文字 -->
<!-- Fill: 填充色；Stroke: 描边色；StrokeThickness: 描边宽度 -->
<!-- StrokePosition: Center | Outside | InSide -->
<pf:OutlineText Text=""PF.UI""
                Fill=""{DynamicResource PrimaryBrush}""
                Stroke=""White""
                StrokeThickness=""2""
                FontSize=""48""
                StrokePosition=""Center"" />

<!-- 空心文字（Fill=Transparent）-->
<pf:OutlineText Text=""OUTLINE""
                Fill=""Transparent""
                Stroke=""{DynamicResource PrimaryBrush}""
                StrokeThickness=""3""
                FontSize=""40""
                StrokePosition=""Outside"" />

<!-- 自定义字体+渐变描边 -->
<pf:OutlineText Text=""Gradient""
                FontSize=""52"" FontWeight=""Bold"">
    <pf:OutlineText.Stroke>
        <LinearGradientBrush StartPoint=""0,0"" EndPoint=""1,0"">
            <GradientStop Color=""{DynamicResource PrimaryColor}"" Offset=""0"" />
            <GradientStop Color=""{DynamicResource InfoColor}"" Offset=""1"" />
        </LinearGradientBrush>
    </pf:OutlineText.Stroke>
</pf:OutlineText>";

        public const string XamlSimple = @"<!-- SimpleText — 轻量格式文本（比 TextBlock 渲染性能更高）-->
<!-- 适合大量纯文本展示场景，不支持内联 Run/Span，但支持基本字体属性 -->
<pf:SimpleText Text=""SimpleText 轻量文本""
               FontSize=""16"" FontWeight=""Bold""
               Foreground=""{DynamicResource PrimaryTextBrush}"" />

<pf:SimpleText Text=""支持 TextWrapping 和 TextTrimming""
               TextWrapping=""Wrap""
               TextTrimming=""CharacterEllipsis""
               Foreground=""{DynamicResource SecondaryTextBrush}"" />

<!-- 与 TextBlock 的区别：
     SimpleText 基于 GlyphRun/DrawingVisual，跳过 WPF 布局树开销
     适合高密度数据表格、日志列表等场景（100k+ 行性能差距明显）
     不支持 Inlines（Run/Hyperlink/LineBreak 等）-->";
    }
}
