using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class NavigationDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "StepBar",    Title = "StepBar",    Sub = "步骤条 / 水平 / 垂直" },
            new DemoTocItem { Anchor = "SideMenu",   Title = "SideMenu",   Sub = "展开模式 / 多级菜单" },
            new DemoTocItem { Anchor = "TabControl", Title = "TabControl", Sub = "默认 / Capsule / Sliding" },
            new DemoTocItem { Anchor = "Drawer",     Title = "Drawer",     Sub = "四方向 / Cover/Push/Press" },
            new DemoTocItem { Anchor = "Ribbon",     Title = "Ribbon",     Sub = "Tab / Group 工具栏" },
        };

        // ===== StepBar =====
        private int _stepIndex;
        public int StepIndex
        {
            get => _stepIndex;
            set => SetProperty(ref _stepIndex, value);
        }

        private int _stepIndexV;
        public int StepIndexV
        {
            get => _stepIndexV;
            set => SetProperty(ref _stepIndexV, value);
        }

        public ICommand NextStepCommand { get; }
        public ICommand PrevStepCommand { get; }
        public ICommand NextStepVCommand { get; }
        public ICommand PrevStepVCommand { get; }

        // ===== Drawer =====
        private bool _drawerLeftOpen;
        public bool DrawerLeftOpen
        {
            get => _drawerLeftOpen;
            set => SetProperty(ref _drawerLeftOpen, value);
        }

        private bool _drawerRightOpen;
        public bool DrawerRightOpen
        {
            get => _drawerRightOpen;
            set => SetProperty(ref _drawerRightOpen, value);
        }

        private bool _drawerTopOpen;
        public bool DrawerTopOpen
        {
            get => _drawerTopOpen;
            set => SetProperty(ref _drawerTopOpen, value);
        }

        private bool _drawerPushOpen;
        public bool DrawerPushOpen
        {
            get => _drawerPushOpen;
            set => SetProperty(ref _drawerPushOpen, value);
        }

        private bool _drawerBottomOpen;
        public bool DrawerBottomOpen
        {
            get => _drawerBottomOpen;
            set => SetProperty(ref _drawerBottomOpen, value);
        }

        private bool _drawerPressOpen;
        public bool DrawerPressOpen
        {
            get => _drawerPressOpen;
            set => SetProperty(ref _drawerPressOpen, value);
        }

        public NavigationDemoViewModel()
        {
            NextStepCommand  = new DelegateCommand(() => { if (StepIndex  < 3) StepIndex++;  });
            PrevStepCommand  = new DelegateCommand(() => { if (StepIndex  > 0) StepIndex--;  });
            NextStepVCommand = new DelegateCommand(() => { if (StepIndexV < 3) StepIndexV++; });
            PrevStepVCommand = new DelegateCommand(() => { if (StepIndexV > 0) StepIndexV--; });
        }

        // ===== 代码示例 =====

        public const string XamlStepBar = @"<!-- StepBar 水平步骤条 -->
<pf:StepBar StepIndex=""{Binding StepIndex}"" IsMouseSelectable=""True"">
    <pf:StepBarItem Content=""基本信息"" />
    <pf:StepBarItem Content=""详细配置"" />
    <pf:StepBarItem Content=""确认预览"" />
    <pf:StepBarItem Content=""完成"" />
</pf:StepBar>

<!-- 垂直步骤条 -->
<pf:StepBar StepIndex=""{Binding StepIndex}""
            Dock=""Left"" IsMouseSelectable=""True"">
    <pf:StepBarItem Content=""步骤 1"" />
    <pf:StepBarItem Content=""步骤 2"" />
    <pf:StepBarItem Content=""步骤 3"" />
</pf:StepBar>";

        public const string XamlSideMenu = @"<!-- SideMenu — ExpandMode 控制展开行为 -->

<!-- ShowOne：同时只展开一项（手风琴） -->
<pf:SideMenu ExpandMode=""ShowOne"">
    <pf:SideMenuItem Header=""首页"">
        <pf:SideMenuItem Header=""子菜单 1"" />
        <pf:SideMenuItem Header=""子菜单 2"" />
    </pf:SideMenuItem>
    <pf:SideMenuItem Header=""设置"">
        <pf:SideMenuItem Header=""系统参数"" />
        <pf:SideMenuItem Header=""用户管理"" />
    </pf:SideMenuItem>
</pf:SideMenu>

<!-- Freedom：无限制，可同时展开多项 -->
<pf:SideMenu ExpandMode=""Freedom"">
    <pf:SideMenuItem Header=""菜单 A"">
        <pf:SideMenuItem Header=""A-1"" />
        <pf:SideMenuItem Header=""A-2"" />
    </pf:SideMenuItem>
</pf:SideMenu>";

        public const string XamlTabControl = @"<!-- 默认 TabControl -->
<TabControl>
    <TabItem Header=""选项卡 1""><TextBlock Text=""内容 1"" /></TabItem>
    <TabItem Header=""选项卡 2""><TextBlock Text=""内容 2"" /></TabItem>
    <TabItem Header=""选项卡 3""><TextBlock Text=""内容 3"" /></TabItem>
</TabControl>

<!-- 胶囊样式 TabControlCapsule -->
<TabControl Style=""{StaticResource TabControlCapsule}"">
    <TabItem Header=""日"" /><TabItem Header=""周"" /><TabItem Header=""月"" />
</TabControl>

<!-- 滑动样式 TabControlSliding -->
<TabControl Style=""{StaticResource TabControlSliding}"">
    <TabItem Header=""概览"" /><TabItem Header=""详情"" /><TabItem Header=""日志"" />
</TabControl>";

        public const string XamlDrawer = @"<!-- Drawer 需包裹在 DrawerContainer 中 -->
<pf:DrawerContainer>
    <!-- Cover 模式（覆盖内容，默认） -->
    <pf:Drawer IsOpen=""{Binding DrawerOpen}""
               Dock=""Left""
               ShowMode=""Cover""
               ShowMask=""True""
               MaskCanClose=""True"">
        <StackPanel Width=""200"" Padding=""16"">
            <TextBlock Text=""侧边栏内容"" />
        </StackPanel>
    </pf:Drawer>

    <!-- Push 模式（推开内容区） -->
    <pf:Drawer IsOpen=""{Binding DrawerOpen}""
               Dock=""Left""
               ShowMode=""Push"">
        <StackPanel Width=""200"" />
    </pf:Drawer>

    <ContentPresenter />
</pf:DrawerContainer>";

        public const string XamlRibbon = @"<!-- Ribbon 工具栏 -->
<pf:Ribbon>
    <pf:RibbonTab Header=""文件"">
        <pf:RibbonGroup Header=""操作"">
            <Button Content=""新建"" />
            <Button Content=""打开"" />
            <Button Content=""保存"" />
        </pf:RibbonGroup>
        <pf:RibbonGroup Header=""导出"">
            <Button Content=""导出 PDF"" />
            <Button Content=""导出 Excel"" />
        </pf:RibbonGroup>
    </pf:RibbonTab>
    <pf:RibbonTab Header=""编辑"">
        <pf:RibbonGroup Header=""剪贴板"">
            <Button Content=""剪切"" />
            <Button Content=""复制"" />
            <Button Content=""粘贴"" />
        </pf:RibbonGroup>
    </pf:RibbonTab>
</pf:Ribbon>";
    }
}
