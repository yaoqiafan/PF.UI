using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace PF.UI.ViewModels.Demos
{
    /// <summary>
    /// 演示页 ViewModel 基类。
    /// KeepAlive = false：每次导航都创建新实例，离开时旧实例随即销毁。
    /// </summary>
    public abstract class DemoViewModelBase : BindableBase, IRegionMemberLifetime
    {
        public bool KeepAlive => false;
    }
}
