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

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string),
                typeof(SidebarNavView), new PropertyMetadata(string.Empty, OnFilterTextChanged));

        public string FilterText
        {
            get => (string)GetValue(FilterTextProperty);
            set => SetValue(FilterTextProperty, value);
        }

        public SidebarNavView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
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
                    _childListBoxes.Add(lb);
                CollectListBoxes(child);
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (SidebarNavView)d;
            self.SyncListBoxSelections(e.NewValue as NavItem);
        }

        private static void OnFilterTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (SidebarNavView)d;
            self.ApplyFilter((e.NewValue as string) ?? string.Empty);
        }

        private void ApplyFilter(string filter)
        {
            if (Groups == null) return;

            filter = filter.Trim();

            if (string.IsNullOrEmpty(filter))
            {
                // 无搜索词：全部恢复
                foreach (var g in Groups)
                {
                    g.IsVisible = true;
                    g.IsExpanded = g.Title == "概览"; // 回到初始状态
                    foreach (var item in g.Children)
                        item.IsVisible = true;
                }
                return;
            }

            foreach (var g in Groups)
            {
                bool hasVisibleChild = false;
                foreach (var item in g.Children)
                {
                    var match = item.Title.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                             || item.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
                    item.IsVisible = match;
                    if (match) hasVisibleChild = true;
                }
                g.IsExpanded = hasVisibleChild;
                g.IsVisible = hasVisibleChild;
            }

            // 如果只有一个结果，不需要滚动到它
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
                    SelectedItem = item;
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
                group.IsExpanded = !group.IsExpanded;
        }
    }
}
