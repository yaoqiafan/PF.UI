using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class SelectorsDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "ComboBox基础",   Title = "ComboBox",           Sub = "普通 / 可编辑 / 小号" },
            new DemoTocItem { Anchor = "ComboBox扩展",   Title = "ComboBox 扩展",      Sub = "TitleElement / Capsule 胶囊" },
            new DemoTocItem { Anchor = "CheckComboBox",  Title = "CheckComboBox",      Sub = "多选 / 全选 / Tag 标签" },
            new DemoTocItem { Anchor = "SearchComboBox", Title = "SearchComboBox",     Sub = "实时过滤下拉" },
            new DemoTocItem { Anchor = "AutoComplete",   Title = "AutoCompleteTextBox", Sub = "输入建议列表" },
            new DemoTocItem { Anchor = "NumericUpDown",  Title = "NumericUpDown",      Sub = "步长 / 最值 / 格式字符串" },
        };

        private string _lastInputResult = "在下方控件中操作，这里将显示交互结果...";
        public string LastInputResult
        {
            get => _lastInputResult;
            set => SetProperty(ref _lastInputResult, value);
        }

        public ICommand LogCommand { get; }

        public ObservableCollection<string> FruitList { get; } = new()
        {
            "苹果", "香蕉", "橙子", "葡萄", "西瓜", "草莓", "蓝莓", "芒果", "柠檬", "樱桃"
        };

        public ObservableCollection<string> CityList { get; } = new()
        {
            "北京", "上海", "广州", "深圳", "杭州", "成都", "武汉", "西安", "南京", "重庆"
        };

        private double _numericValue = 50;
        public double NumericValue
        {
            get => _numericValue;
            set
            {
                if (SetProperty(ref _numericValue, value))
                    LastInputResult = $"NumericUpDown 值变更: {value}  ({System.DateTime.Now:HH:mm:ss})";
            }
        }

        public SelectorsDemoViewModel()
        {
            LogCommand = new DelegateCommand<string>(msg =>
                LastInputResult = $"{msg}  ({System.DateTime.Now:HH:mm:ss})");
        }

        // ===== 代码示例 =====

        public const string XamlComboBox = @"<!-- 标准 ComboBox -->
<ComboBox pf:InfoElement.Placeholder=""请选择..."">
    <ComboBoxItem Content=""选项 A"" />
    <ComboBoxItem Content=""选项 B"" />
    <ComboBoxItem Content=""选项 C"" />
</ComboBox>

<!-- 可编辑 ComboBox -->
<ComboBox IsEditable=""True"" pf:InfoElement.Placeholder=""可输入或选择..."">
    <ComboBoxItem Content=""苹果"" />
    <ComboBoxItem Content=""香蕉"" />
</ComboBox>

<!-- 绑定数据源 + 小号 -->
<ComboBox ItemsSource=""{Binding FruitList}""
          pf:InfoElement.Placeholder=""请选择水果..."" />
<ComboBox Style=""{StaticResource ComboBox.Small}""
          pf:InfoElement.Placeholder=""小号..."" />";

        public const string XamlComboBoxExtend = @"<!-- ComboBoxExtend — 带浮动标题 -->
<ComboBox Style=""{StaticResource ComboBoxExtend}""
          pf:TitleElement.Title=""城市""
          pf:InfoElement.Placeholder=""请选择城市"">
    <ComboBoxItem Content=""北京"" />
    <ComboBoxItem Content=""上海"" />
</ComboBox>

<!-- Top 布局 -->
<ComboBox Style=""{StaticResource ComboBoxExtend}""
          pf:TitleElement.Title=""类型""
          pf:TitleElement.TitlePlacement=""Top""
          pf:InfoElement.Placeholder=""请选择类型"" />

<!-- 胶囊分段样式 ComboBoxCapsule -->
<!-- 所有选项横向平铺（非下拉），适合 3-5 个固定选项 -->
<ComboBox Style=""{StaticResource ComboBoxCapsule}"">
    <ComboBoxItem Content=""日"" IsSelected=""True"" />
    <ComboBoxItem Content=""周"" />
    <ComboBoxItem Content=""月"" />
    <ComboBoxItem Content=""年"" />
</ComboBox>";

        public const string XamlCheckComboBox = @"<!-- CheckComboBox — 多选下拉 -->
<pf:CheckComboBox pf:InfoElement.Placeholder=""请选择（多选）"">
    <pf:CheckComboBoxItem Content=""苹果"" />
    <pf:CheckComboBoxItem Content=""香蕉"" />
    <pf:CheckComboBoxItem Content=""橙子"" />
</pf:CheckComboBox>

<!-- 显示全选按钮 -->
<pf:CheckComboBox ShowSelectAllButton=""True""
                  pf:InfoElement.Placeholder=""支持全选/反选..."">
    <pf:CheckComboBoxItem Content=""北京"" />
    <pf:CheckComboBoxItem Content=""上海"" />
    <pf:CheckComboBoxItem Content=""广州"" />
</pf:CheckComboBox>

<!-- 扩展样式（带标题） -->
<pf:CheckComboBox Style=""{StaticResource CheckComboBoxExtend}""
                  pf:TitleElement.Title=""城市""
                  pf:TitleElement.TitlePlacement=""Top""
                  pf:InfoElement.Placeholder=""多选城市..."" />";

        public const string XamlSearchComboBox = @"<!-- SearchComboBox — 带搜索过滤的多选下拉 -->
<pf:SearchComboBox pf:InfoElement.Placeholder=""输入关键字过滤..."">
    <pf:SearchComboBoxItem Content=""北京"" />
    <pf:SearchComboBoxItem Content=""上海"" />
    <pf:SearchComboBoxItem Content=""广州"" />
    <pf:SearchComboBoxItem Content=""深圳"" />
    <pf:SearchComboBoxItem Content=""杭州"" />
</pf:SearchComboBox>";

        public const string XamlAutoComplete = @"<!-- AutoCompleteTextBox — 带建议列表的输入框 -->
<pf:AutoCompleteTextBox pf:InfoElement.Placeholder=""输入以显示建议..."">
    <pf:AutoCompleteTextBoxItem Content=""苹果"" />
    <pf:AutoCompleteTextBoxItem Content=""香蕉"" />
    <pf:AutoCompleteTextBoxItem Content=""橙子"" />
    <pf:AutoCompleteTextBoxItem Content=""葡萄"" />
    <pf:AutoCompleteTextBoxItem Content=""西瓜"" />
</pf:AutoCompleteTextBox>";

        public const string XamlNumericUpDown = @"<!-- NumericUpDown — 数值步进输入框 -->
<pf:NumericUpDown Value=""50"" Minimum=""0"" Maximum=""100"" Increment=""1"" />

<!-- 小数 + 格式字符串 -->
<pf:NumericUpDown Value=""3.14""
                  Minimum=""0"" Maximum=""10""
                  Increment=""0.01"" DecimalPlaces=""2""
                  ValueFormat=""{}{0:F2}"" />

<!-- 带标题的扩展样式 -->
<pf:NumericUpDown Style=""{StaticResource NumericUpDownExtend}""
                  pf:TitleElement.Title=""数量""
                  pf:TitleElement.TitlePlacement=""Top""
                  Value=""1"" Minimum=""1"" Maximum=""99"" />

<!-- 隐藏上下按钮 -->
<pf:NumericUpDown ShowUpDownButton=""False""
                  Value=""0"" Minimum=""-100"" Maximum=""100"" />";
    }
}
