using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PF.UI.Models;

namespace PF.UI.Controls
{
    public partial class SidebarNavView : UserControl
    {
        private readonly List<ListBox> _childListBoxes = new();
        private bool _syncing;

        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register(nameof(Groups), typeof(ObservableCollection<NavGroup>),
                typeof(SidebarNavView), new PropertyMetadata(null));

        public ObservableCollection<NavGroup> Groups
        {
            get => (ObservableCollection<NavGroup>)GetValue(GroupsProperty);
            set => SetValue(GroupsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(NavItem),
                typeof(SidebarNavView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        public NavItem? SelectedItem
        {
            get => (NavItem?)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public SidebarNavView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            // 延迟到布局完成后收集 ListBox 实例
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                CollectListBoxes(this);
                SyncListBoxSelections(SelectedItem);
            }));
        }

        private void CollectListBoxes(DependencyObject parent)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is ListBox lb)
                {
                    _childListBoxes.Add(lb);
                }
                CollectListBoxes(child);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (SidebarNavView)d;
            self.SyncListBoxSelections(e.NewValue as NavItem);
        }

        private void SyncListBoxSelections(NavItem? item)
        {
            if (_syncing) return;
            _syncing = true;
            try
            {
                foreach (var lb in _childListBoxes)
                {
                    if (item != null && lb.Items.Contains(item))
                    {
                        if (lb.SelectedItem != item)
                            lb.SelectedItem = item;
                    }
                    else
                    {
                        lb.SelectedItem = null;
                    }
                }
            }
            finally
            {
                _syncing = false;
            }
        }

        private void ChildListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_syncing) return;

            if (sender is ListBox lb && lb.SelectedItem is NavItem item)
            {
                _syncing = true;
                try
                {
                    // 同步到外部（触发 TwoWay→ViewModel→导航）
                    SelectedItem = item;
                    // 清除其他 ListBox 的选中
                    foreach (var other in _childListBoxes.Where(x => x != lb))
                        other.SelectedItem = null;
                }
                finally
                {
                    _syncing = false;
                }
            }
        }

        private void GroupHeader_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is NavGroup group)
            {
                group.IsExpanded = !group.IsExpanded;
            }
        }
    }
}
