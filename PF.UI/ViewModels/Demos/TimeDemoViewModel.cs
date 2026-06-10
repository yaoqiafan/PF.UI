using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class TimeDemoViewModel : DemoViewModelBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "FlipClock",     Title = "FlipClock",     Sub = "翻转数字时钟" },
            new DemoTocItem { Anchor = "Clock",         Title = "Clock",         Sub = "圆形 / 列表时钟" },
            new DemoTocItem { Anchor = "Pickers",       Title = "Pickers",       Sub = "TimePicker · DateTimePicker" },
            new DemoTocItem { Anchor = "DateTimeRange", Title = "范围选择",      Sub = "DateTimeSelector · CalendarWithClock" },
            new DemoTocItem { Anchor = "ListClock",     Title = "ListClock 对比", Sub = "CalendarWithListClock" },
        };

        private string _lastResult = "与下方控件交互查看效果...";
        public string LastResult
        {
            get => _lastResult;
            set => SetProperty(ref _lastResult, value);
        }

        // ===== TimePicker =====
        private DateTime? _pickerTime;
        public DateTime? PickerTime
        {
            get => _pickerTime;
            set
            {
                if (SetProperty(ref _pickerTime, value) && value.HasValue)
                    LastResult = $"TimePicker: {value:HH:mm:ss}  ({DateTime.Now:HH:mm:ss})";
            }
        }

        // ===== DateTimePicker =====
        private DateTime? _pickerDateTime;
        public DateTime? PickerDateTime
        {
            get => _pickerDateTime;
            set
            {
                if (SetProperty(ref _pickerDateTime, value) && value.HasValue)
                    LastResult = $"DateTimePicker: {value:yyyy-MM-dd HH:mm}  ({DateTime.Now:HH:mm:ss})";
            }
        }

        // ===== DateTimeSelector =====
        private DateTime _startTime = DateTime.Today;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (SetProperty(ref _startTime, value))
                    LastResult = $"Range: {value:MM-dd HH:mm} → {_endTime:MM-dd HH:mm}  ({DateTime.Now:HH:mm:ss})";
            }
        }

        private DateTime _endTime = DateTime.Today.AddDays(7);
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (SetProperty(ref _endTime, value))
                    LastResult = $"Range: {_startTime:MM-dd HH:mm} → {value:MM-dd HH:mm}  ({DateTime.Now:HH:mm:ss})";
            }
        }

        public const string XamlFlipClock = @"<!-- FlipClock — 内置 DispatcherTimer 每 200ms 自动刷新 DateTime.Now -->
<pf:FlipClock />

<!-- 也可手动设置 DisplayTime -->
<pf:FlipClock DisplayTime=""{Binding SomeDateTime}"" />";

        public const string XamlClock = @"<!-- Clock — 圆形表盘，点击/拖转选时间 -->
<pf:Clock Width=""240"" ShowConfirmButton=""True""
          SelectedTime=""{Binding SelectedTime, Mode=TwoWay}"" />

<!-- ListClock — 三列滚动列表选时/分/秒 -->
<pf:ListClock Width=""220"" ShowConfirmButton=""True""
              SelectedTime=""{Binding SelectedTime, Mode=TwoWay}"" />

<!-- ClockBase 公共属性 -->
<!-- TimeFormat=""HH:mm:ss""（默认） -->
<!-- ShowConfirmButton=""True/False""  SelectedTime=""DateTime?"" -->
<!-- DisplayTime=""DateTime""（当前显示时间） -->";

        public const string XamlPickers = @"<!-- TimePicker — 文本框 + 弹出 Clock 选时间 -->
<pf:TimePicker Width=""220""
               SelectedTime=""{Binding PickerTime, Mode=TwoWay}"" />

<!-- DateTimePicker — 文本框 + 弹出 CalendarWithClock -->
<pf:DateTimePicker Width=""220""
                   SelectedDateTime=""{Binding PickerDateTime, Mode=TwoWay}"" />

<!-- 原生 WPF DatePicker（自动应用 PF 主题） -->
<DatePicker Width=""220"" />";

        public const string XamlDateTimeRange = @"<!-- DateTimeSelector — 起止时间范围选择器 -->
<pf:DateTimeSelector
    StartTime=""{Binding StartTime, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}""
    EndTime=""{Binding EndTime, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"" />

<!-- CalendarWithClock — 日历 + 圆形时钟组合面板 -->
<pf:CalendarWithClock ShowConfirmButton=""True""
                      SelectedDateTime=""{Binding PickerDateTime, Mode=TwoWay}"" />";

        public const string XamlListClock = @"<!-- CalendarWithListClock — 日历 + 列表式时钟（更紧凑，适合小屏）-->
<pf:CalendarWithListClock ShowConfirmButton=""True""
                          SelectedDateTime=""{Binding PickerDateTime, Mode=TwoWay}"" />

<!-- 与 CalendarWithClock 的区别：
     CalendarWithClock    → 圆形表盘（Clock），视觉直观但占用空间较大
     CalendarWithListClock → 列表滚动（ListClock），紧凑，适合窄面板
     两者均支持 ShowConfirmButton、SelectedDateTime、DateTimeFormat 属性 -->";
    }
}
