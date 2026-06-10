using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PF.UI.ViewModels.Demos
{
    public class ConvertersDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Boolean",  Title = "Boolean 系列",  Sub = "6 个转换器" },
            new DemoTocItem { Anchor = "Object",   Title = "Object/String", Sub = "7 个转换器" },
            new DemoTocItem { Anchor = "Number",   Title = "数值/时间",     Sub = "6 个转换器" },
            new DemoTocItem { Anchor = "Color",    Title = "颜色/画刷",     Sub = "5 个转换器" },
            new DemoTocItem { Anchor = "Internal", Title = "内部辅助",      Sub = "9 个模板辅助" },
        };

        // ─── Boolean 系列 ──────────────────────────────────────────────────

        public const string XamlBoolDecl = @"<!-- 在 Window/UserControl/App.xaml Resources 中声明 -->
xmlns:conv=""clr-namespace:PF.UI.Shared.Tools.Converter;assembly=PF.UI.Shared""

<conv:Boolean2BooleanReConverter    x:Key=""BoolRe"" />
<conv:Boolean2StringConverter       x:Key=""Bool2Str"" />
<conv:Boolean2VisibilityReConverter x:Key=""BoolVisRe"" />
<conv:BooleanArr2BooleanConverter   x:Key=""BoolArr2Bool"" />
<conv:BooleanArr2VisibilityConverter x:Key=""BoolArr2Vis"" />
<conv:NullableToVisibilityConverter x:Key=""Nullable2Vis"" />

<!-- PF.UI.Resources 已在 Default.xaml 中预注册上述所有转换器，
     引入主题后可直接使用无需再次声明 -->";

        public const string XamlBoolUsage = @"<!-- Boolean2BooleanReConverter：bool 取反 -->
<!-- IsLoading=true → Button.IsEnabled=false -->
<Button IsEnabled=""{Binding IsLoading,
        Converter={StaticResource BoolRe}}"" />

<!-- Boolean2StringConverter：参数格式 ""TrueText|FalseText"" -->
<TextBlock Text=""{Binding IsOnline,
           Converter={StaticResource Bool2Str},
           ConverterParameter='在线|离线'}"" />

<!-- Boolean2VisibilityReConverter：true → Collapsed（与内置 BoolToVis 相反）-->
<Border Visibility=""{Binding IsEmpty,
        Converter={StaticResource BoolVisRe}}"" />

<!-- BooleanArr2BooleanConverter：多个 bool → 单个 bool -->
<!-- ConverterParameter: And（全部为 true）/ Or（任一为 true） -->
<Button IsEnabled=""{MultiBinding Converter={StaticResource BoolArr2Bool}
                                  ConverterParameter=And}"">
    <MultiBinding.Bindings>
        <Binding Path=""IsConnected"" />
        <Binding Path=""IsIdle"" />
    </MultiBinding.Bindings>
</Button>

<!-- BooleanArr2VisibilityConverter：多 bool → Visibility，同上支持 And/Or -->
<Border Visibility=""{MultiBinding Converter={StaticResource BoolArr2Vis}
                                   ConverterParameter=Or}"">
    <MultiBinding.Bindings>
        <Binding Path=""HasWarning"" />
        <Binding Path=""HasError"" />
    </MultiBinding.Bindings>
</Border>

<!-- NullableToVisibilityConverter：HasValue=true → Visible -->
<TextBlock Text=""{Binding SelectedItem.Name}""
           Visibility=""{Binding SelectedItem,
               Converter={StaticResource Nullable2Vis}}"" />";

        // ─── Object / String 系列 ──────────────────────────────────────────

        public const string XamlObjectDecl = @"<conv:Object2BooleanConverter    x:Key=""Obj2Bool"" />
<conv:Object2BooleanReConverter  x:Key=""Obj2BoolRe"" />
<conv:Object2VisibilityConverter x:Key=""Obj2Vis"" />
<conv:Object2VisibilityReConverter x:Key=""Obj2VisRe"" />
<conv:Object2StringConverter     x:Key=""Obj2Str"" />
<conv:String2VisibilityConverter x:Key=""Str2Vis"" />
<conv:String2VisibilityReConverter x:Key=""Str2VisRe"" />";

        public const string XamlObjectUsage = @"<!-- Object2BooleanConverter：obj != null → true -->
<Button IsEnabled=""{Binding SelectedItem, Converter={StaticResource Obj2Bool}}"" />

<!-- Object2BooleanReConverter：obj == null → true（反） -->
<TextBlock Text=""请先选择一项""
           Visibility=""{Binding SelectedItem,
               Converter={StaticResource Obj2Bool}}"" />

<!-- Object2VisibilityConverter：obj != null → Visible -->
<Border Visibility=""{Binding CurrentUser, Converter={StaticResource Obj2Vis}}"">
    <!-- 已登录才显示的内容 -->
</Border>

<!-- Object2VisibilityReConverter：obj != null → Collapsed（空态占位） -->
<TextBlock Text=""暂无数据""
           Visibility=""{Binding ItemList, Converter={StaticResource Obj2VisRe}}"" />

<!-- Object2StringConverter：调用 .ToString()，比 StringFormat 更通用 -->
<TextBlock Text=""{Binding SomeEnum, Converter={StaticResource Obj2Str}}"" />

<!-- String2VisibilityConverter：!IsNullOrEmpty → Visible -->
<TextBlock Text=""{Binding ErrorMsg}""
           Visibility=""{Binding ErrorMsg, Converter={StaticResource Str2Vis}}"" />

<!-- String2VisibilityReConverter：IsNullOrEmpty → Visible（占位反向） -->
<TextBlock Text=""(无内容)""
           Visibility=""{Binding Content, Converter={StaticResource Str2VisRe}}"" />";

        // ─── 数值 / 时间 ────────────────────────────────────────────────────

        public const string XamlNumberDecl = @"<conv:Int2StringConverter         x:Key=""Int2Str"" />
<conv:Long2FileSizeConverter      x:Key=""FileSize"" />
<conv:Number2PercentageConverter  x:Key=""Pct"" />
<conv:DoubleMinConverter          x:Key=""DblMin"" />
<conv:Double2GridLengthConverter  x:Key=""Dbl2GL"" />
<conv:DateTimeToStringConverter   x:Key=""DT2Str"" />";

        public const string XamlNumberUsage = @"<!-- Int2StringConverter：参数作 format string -->
<!-- 42 → ""042"" -->
<TextBlock Text=""{Binding Index, Converter={StaticResource Int2Str},
                            ConverterParameter=D3}"" />

<!-- Long2FileSizeConverter：字节数 → 人类可读文件大小 -->
<!-- 1572864 → ""1.5 MB"" -->
<TextBlock Text=""{Binding FileBytes, Converter={StaticResource FileSize}}"" />

<!-- Number2PercentageConverter（IMultiValueConverter）：value/max → ""nn%"" -->
<TextBlock>
    <TextBlock.Text>
        <MultiBinding Converter=""{StaticResource Pct}"">
            <Binding Path=""Completed"" />   <!-- 当前值 -->
            <Binding Path=""Total"" />       <!-- 最大值 -->
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>

<!-- DoubleMinConverter：返回 value 与 parameter 中的较小值 -->
<!-- 用于限制动态宽度不超过固定上限 -->
<Border Width=""{Binding PanelWidth,
        Converter={StaticResource DblMin},
        ConverterParameter=320}"" />

<!-- Double2GridLengthConverter：数值 → GridLength(n, Pixel) -->
<RowDefinition Height=""{Binding RowH, Converter={StaticResource Dbl2GL}}"" />

<!-- DateTimeToStringConverter：参数作 format string -->
<TextBlock Text=""{Binding CreatedAt, Converter={StaticResource DT2Str},
                            ConverterParameter='yyyy-MM-dd HH:mm'}"" />";

        // ─── 颜色 / 画刷 ────────────────────────────────────────────────────

        public const string XamlColorDecl = @"<conv:Color2HexStringConverter    x:Key=""Color2Hex"" />
<conv:Color2ChannelAConverter     x:Key=""Color2A"" />
<conv:HsbToColorConverter         x:Key=""Hsb2Color"" />
<conv:HsbLinearGradientConverter  x:Key=""Hsb2Grad"" />
<conv:BrushRoundConverter         x:Key=""BrushRound"" />";

        public const string XamlColorUsage = @"<!-- Color2HexStringConverter：Color → ""#AARRGGBB"" 十六进制字符串 -->
<TextBlock Text=""{Binding PickedColor, Converter={StaticResource Color2Hex}}"" />

<!-- Color2ChannelAConverter：Color → Alpha 通道字节值（0-255）-->
<TextBlock Text=""{Binding PickedColor, Converter={StaticResource Color2A}}"" />

<!-- HsbToColorConverter（双接口）：
     IValueConverter：Hue double → Color（固定 S=1 B=1）
     IMultiValueConverter：[H, S, B] → Color -->
<Rectangle>
    <Rectangle.Fill>
        <SolidColorBrush>
            <SolidColorBrush.Color>
                <MultiBinding Converter=""{StaticResource Hsb2Color}"">
                    <Binding Path=""Hue"" />
                    <Binding Path=""Saturation"" />
                    <Binding Path=""Brightness"" />
                </MultiBinding>
            </SolidColorBrush.Color>
        </SolidColorBrush>
    </Rectangle.Fill>
</Rectangle>

<!-- HsbLinearGradientConverter：Hue → 包含完整饱和度渐变的 LinearGradientBrush -->
<!-- 用于 ColorPicker 的饱和度/亮度选择面板背景 -->
<Rectangle Fill=""{Binding Hue, Converter={StaticResource Hsb2Grad}}"" />

<!-- BrushRoundConverter：像素对齐修正，消除亚像素渲染时的模糊边缘 -->
<Border BorderBrush=""{Binding ThemeBrush,
        Converter={StaticResource BrushRound}}"" />";
    }
}
