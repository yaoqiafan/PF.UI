using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class SlidersDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "NativeSlider",  Title = "原生 Slider",   Sub = "主题化水平/垂直/刻度" },
            new DemoTocItem { Anchor = "RangeSlider",   Title = "RangeSlider",   Sub = "双端区间滑块" },
            new DemoTocItem { Anchor = "PreviewSlider", Title = "PreviewSlider", Sub = "悬停气泡预览" },
            new DemoTocItem { Anchor = "CompareSlider", Title = "CompareSlider", Sub = "前后内容对比" },
        };

        private string _lastResult = "拖动下方滑块查看效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ===== 原生 Slider =====
        private double _sliderValue = 50;
        public double SliderValue
        {
            get => _sliderValue;
            set
            {
                if (SetProperty(ref _sliderValue, value))
                    LastResult = $"Slider: {value:F0}  ({DateTime.Now:HH:mm:ss})";
            }
        }

        // ===== RangeSlider =====
        private double _rangeStart = 20;
        public double RangeStart
        {
            get => _rangeStart;
            set
            {
                if (SetProperty(ref _rangeStart, value))
                    LastResult = $"Range: [{value:F0}, {_rangeEnd:F0}]  ({DateTime.Now:HH:mm:ss})";
            }
        }

        private double _rangeEnd = 70;
        public double RangeEnd
        {
            get => _rangeEnd;
            set
            {
                if (SetProperty(ref _rangeEnd, value))
                    LastResult = $"Range: [{_rangeStart:F0}, {value:F0}]  ({DateTime.Now:HH:mm:ss})";
            }
        }

        public const string XamlNativeSlider = @"<!-- WPF Slider — 使用 PF.UI 主题，无需额外样式 -->
<Slider Minimum=""0"" Maximum=""100""
        Value=""{Binding SliderValue, Mode=TwoWay}"" />

<!-- 带刻度 + 对齐刻度 -->
<Slider Minimum=""0"" Maximum=""100""
        TickFrequency=""10"" TickPlacement=""BottomRight""
        IsSnapToTickEnabled=""True"" />

<!-- 垂直方向 -->
<Slider Orientation=""Vertical"" Minimum=""0"" Maximum=""100"" Height=""120"" />

<!-- 禁用态 -->
<Slider Minimum=""0"" Maximum=""100"" Value=""60"" IsEnabled=""False"" />";

        public const string XamlRangeSlider = @"<!-- RangeSlider — 双 Thumb，ValueStart / ValueEnd 独立绑定 -->
<pf:RangeSlider Minimum=""0"" Maximum=""100""
                ValueStart=""{Binding RangeStart, Mode=TwoWay,
                              UpdateSourceTrigger=PropertyChanged}""
                ValueEnd=""{Binding RangeEnd, Mode=TwoWay,
                              UpdateSourceTrigger=PropertyChanged}"" />

<!-- IsMoveToPointEnabled — 点击轨道直接移到最近端点 -->
<pf:RangeSlider Minimum=""0"" Maximum=""100""
                ValueStart=""20"" ValueEnd=""70""
                IsMoveToPointEnabled=""True"" />

<!-- 带刻度 -->
<pf:RangeSlider Minimum=""0"" Maximum=""100""
                ValueStart=""30"" ValueEnd=""80""
                TickFrequency=""10"" TickPlacement=""BottomRight""
                IsSnapToTickEnabled=""True"" />

<!-- TipElement 附加属性 — 拖动时显示浮动数值气泡 -->
<pf:RangeSlider Minimum=""0"" Maximum=""100""
                ValueStart=""20"" ValueEnd=""60""
                pf:TipElement.Visibility=""Visible""
                pf:TipElement.Placement=""Top""
                pf:TipElement.StringFormat=""#0"" />

<!-- 垂直方向 + TipElement -->
<pf:RangeSlider Orientation=""Vertical"" Height=""120""
                Minimum=""0"" Maximum=""100""
                ValueStart=""25"" ValueEnd=""75""
                pf:TipElement.Visibility=""Visible""
                pf:TipElement.Placement=""Right""
                pf:TipElement.StringFormat=""#0.0"" />";

        public const string XamlPreviewSlider = @"<!-- PreviewSlider — 鼠标悬停时在 Thumb 上方显示浮动气泡 -->
<pf:PreviewSlider Minimum=""0"" Maximum=""100"" Value=""50"">
    <pf:PreviewSlider.PreviewContent>
        <!-- 气泡通过 RelativeSource 绑定继承的 PreviewPosition 附加属性 -->
        <Border Background=""{DynamicResource PrimaryBrush}""
                CornerRadius=""4"" Padding=""10,4"">
            <TextBlock Foreground=""White"" FontSize=""11""
                       Text=""{Binding Path=(pf:PreviewSlider.PreviewPosition),
                                       RelativeSource={RelativeSource Self},
                                       StringFormat=F0}"" />
        </Border>
    </pf:PreviewSlider.PreviewContent>
</pf:PreviewSlider>";

        public const string XamlCompareSlider = @"<!-- CompareSlider — 拖动 Thumb 在 SourceContent / TargetContent 间切分显示 -->
<pf:CompareSlider Minimum=""0"" Maximum=""100"" Value=""50"" Height=""160"">
    <pf:CompareSlider.SourceContent>
        <Border Background=""{DynamicResource PrimaryBrush}"">
            <TextBlock Text=""处理前"" Foreground=""White"" FontSize=""18"" FontWeight=""Bold""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center"" />
        </Border>
    </pf:CompareSlider.SourceContent>
    <pf:CompareSlider.TargetContent>
        <Border Background=""{DynamicResource DangerBrush}"">
            <TextBlock Text=""处理后"" Foreground=""White"" FontSize=""18"" FontWeight=""Bold""
                       HorizontalAlignment=""Center"" VerticalAlignment=""Center"" />
        </Border>
    </pf:CompareSlider.TargetContent>
</pf:CompareSlider>";
    }
}
