using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using PF.UI.Controls;
using PF.UI.Models;
using PF.UI.Views.Demos;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace PF.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public ObservableCollection<NavGroup> NavGroups { get; } = new();

        private NavItem? _selectedItem;
        public NavItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                {
                    // 自动展开包含选中项的组
                    foreach (var g in NavGroups)
                    {
                        if (g.Children.Contains(value))
                            g.IsExpanded = true;
                    }
                    _regionManager.RequestNavigate("DemoRegion", value.ViewName);
                }
            }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            BuildNavGroups();

            // 延迟到窗口 Loaded 后导航首页
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                () => SelectedItem = NavGroups[0].Children[0]);
        }

        private void BuildNavGroups()
        {
            // ===== 1. 概览 =====
            NavGroups.Add(new NavGroup
            {
                Title = "概览",
                Icon = PackIconKind.HomeOutline,
                IsExpanded = true,
                Children =
                {
                    new NavItem
                    {
                        Title = "快速开始",
                        Description = "PF.UI 简介与安装",
                        ViewName = nameof(OverviewDemoView),
                        Icon = PackIconKind.InformationOutline
                    }
                }
            });

            // ===== 2. 按钮 =====
            NavGroups.Add(new NavGroup
            {
                Title = "按钮",
                Icon = PackIconKind.GestureTapButton,
                Children =
                {
                    new NavItem
                    {
                        Title = "按钮演示",
                        Description = "Button / ProgressButton / SplitButton / Shield ...",
                        ViewName = nameof(ButtonsDemoView),
                        Icon = PackIconKind.RadioboxMarked
                    }
                }
            });

            // ===== 3. 输入 =====
            NavGroups.Add(new NavGroup
            {
                Title = "输入",
                Icon = PackIconKind.FormTextbox,
                Children =
                {
                    new NavItem
                    {
                        Title = "文本输入",
                        Description = "TextBox / WatermarkTextBox / SearchBar / PasswordBox / PinBox",
                        ViewName = nameof(TextInputDemoView),
                        Icon = PackIconKind.TextBoxOutline
                    },
                    new NavItem
                    {
                        Title = "选择器",
                        Description = "ComboBox / CheckComboBox / AutoComplete / NumericUpDown ...",
                        ViewName = nameof(SelectorsDemoView),
                        Icon = PackIconKind.ChevronDownBoxOutline
                    }
                }
            });

            // ===== 4. 数据展示 =====
            NavGroups.Add(new NavGroup
            {
                Title = "数据展示",
                Icon = PackIconKind.TableLarge,
                Children =
                {
                    new NavItem
                    {
                        Title = "数据控件",
                        Description = "DataGrid / ListBox / ListView / TreeView / PropertyGrid",
                        ViewName = nameof(DataDemoView),
                        Icon = PackIconKind.GridLarge
                    }
                }
            });

            // ===== 5. 布局 =====
            NavGroups.Add(new NavGroup
            {
                Title = "布局",
                Icon = PackIconKind.ViewDashboardOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "面板演示",
                        Description = "FlexPanel / HoneycombPanel / CirclePanel / WaterfallPanel ...",
                        ViewName = nameof(PanelsDemoView),
                        Icon = PackIconKind.Grid
                    }
                }
            });

            // ===== 6. 导航 =====
            NavGroups.Add(new NavGroup
            {
                Title = "导航",
                Icon = PackIconKind.MenuOpen,
                Children =
                {
                    new NavItem
                    {
                        Title = "导航控件",
                        Description = "SideMenu / StepBar / TabControl / Ribbon / Drawer",
                        ViewName = nameof(NavigationDemoView),
                        Icon = PackIconKind.DockRight
                    }
                }
            });

            // ===== 7. 反馈（拆分为3页）=====
            NavGroups.Add(new NavGroup
            {
                Title = "反馈",
                Icon = PackIconKind.BellOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "消息通知",
                        Description = "Growl / Notification / Poptip",
                        ViewName = nameof(GrowlNotificationView),
                        Icon = PackIconKind.MessageTextOutline
                    },
                    new NavItem
                    {
                        Title = "状态标记",
                        Description = "Badge / Card / Divider / Empty",
                        ViewName = nameof(StatusIndicatorView),
                        Icon = PackIconKind.LabelOutline
                    },
                    new NavItem
                    {
                        Title = "加载与进度",
                        Description = "Loading / ProgressBar / ConfettiCannon",
                        ViewName = nameof(ProgressLoadingView),
                        Icon = PackIconKind.ProgressCheck
                    }
                }
            });

            // ===== 8. 对话框 =====
            NavGroups.Add(new NavGroup
            {
                Title = "对话框",
                Icon = PackIconKind.MessageTextOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "模态对话框",
                        Description = "DialogOverlay 页内模态对话框",
                        ViewName = nameof(DialogDemoView),
                        Icon = PackIconKind.WindowMaximize
                    }
                }
            });

            // ===== 9. 图标与动画 =====
            NavGroups.Add(new NavGroup
            {
                Title = "图标与动画",
                Icon = PackIconKind.AutoFix,
                Children =
                {
                    new NavItem
                    {
                        Title = "PackIcon",
                        Description = "700+ Material 图标库",
                        ViewName = nameof(PackIconDemoView),
                        Icon = PackIconKind.StarOutline
                    },
                    new NavItem
                    {
                        Title = "Ripple",
                        Description = "涟漪点击效果",
                        ViewName = nameof(RippleDemoView),
                        Icon = PackIconKind.Waves
                    },
                    new NavItem
                    {
                        Title = "过渡动画",
                        Description = "TransitioningContentControl / Sprite / RunningBlock ...",
                        ViewName = nameof(AnimationDemoView),
                        Icon = PackIconKind.Transition
                    }
                }
            });

            // ===== 10. 标签与评分 =====
            NavGroups.Add(new NavGroup
            {
                Title = "标签与评分",
                Icon = PackIconKind.LabelOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "Chip 标签",
                        Description = "可交互胶囊标签",
                        ViewName = nameof(ChipDemoView),
                        Icon = PackIconKind.Label
                    },
                    new NavItem
                    {
                        Title = "Tag & Rate",
                        Description = "Tag / TagContainer / Rate 评分",
                        ViewName = nameof(TagsRateDemoView),
                        Icon = PackIconKind.StarOutline
                    }
                }
            });

            // ===== 11. 滑块 =====
            NavGroups.Add(new NavGroup
            {
                Title = "滑块",
                Icon = PackIconKind.TuneVariant,
                Children =
                {
                    new NavItem
                    {
                        Title = "滑块演示",
                        Description = "RangeSlider / CompareSlider / CircleProgressBar ...",
                        ViewName = nameof(SlidersDemoView),
                        Icon = PackIconKind.ArrowSplitHorizontal
                    }
                }
            });

            // ===== 12. 时间 =====
            NavGroups.Add(new NavGroup
            {
                Title = "时间",
                Icon = PackIconKind.ClockOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "时间日历",
                        Description = "Clock / FlipClock / DateTimePicker / CalendarWithClock ...",
                        ViewName = nameof(TimeDemoView),
                        Icon = PackIconKind.CalendarMonthOutline
                    }
                }
            });

            // ===== 13. 颜色 =====
            NavGroups.Add(new NavGroup
            {
                Title = "颜色",
                Icon = PackIconKind.PaletteOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "颜色系统",
                        Description = "主题色系 · ColorPicker · HSB 工具 · Material 调色板",
                        ViewName = nameof(ColorDemoView),
                        Icon = PackIconKind.PaletteOutline
                    }
                }
            });

            // ===== 14. 附加属性 =====
            NavGroups.Add(new NavGroup
            {
                Title = "附加属性",
                Icon = PackIconKind.Paperclip,
                Children =
                {
                    new NavItem
                    {
                        Title = "附加属性一览",
                        Description = "WindowAttach / RippleAssist / DataGridAttach / Element 类 ...",
                        ViewName = nameof(AttachedDemoView),
                        Icon = PackIconKind.PuzzleOutline
                    }
                }
            });

            // ===== 15. 转换器 =====
            NavGroups.Add(new NavGroup
            {
                Title = "转换器",
                Icon = PackIconKind.SwapHorizontal,
                Children =
                {
                    new NavItem
                    {
                        Title = "转换器一览",
                        Description = "33 个 IValueConverter / IMultiValueConverter",
                        ViewName = nameof(ConvertersDemoView),
                        Icon = PackIconKind.CodeJson
                    }
                }
            });

            // ===== 16. 交互 =====
            NavGroups.Add(new NavGroup
            {
                Title = "交互",
                Icon = PackIconKind.MouseOutline,
                Children =
                {
                    new NavItem
                    {
                        Title = "行为与命令",
                        Description = "EventToCommand / FluidMoveBehavior / 内置命令 ...",
                        ViewName = nameof(InteractivityDemoView),
                        Icon = PackIconKind.CursorDefault
                    }
                }
            });

            // ===== 17. 窗口 =====
            NavGroups.Add(new NavGroup
            {
                Title = "窗口",
                Icon = PackIconKind.MonitorScreenshot,
                Children =
                {
                    new NavItem
                    {
                        Title = "窗口演示",
                        Description = "BlurWindow / GlowWindow / PopupWindow / NotifyIcon ...",
                        ViewName = nameof(WindowsDemoView),
                        Icon = PackIconKind.ApplicationOutline
                    }
                }
            });
        }
    }
}
