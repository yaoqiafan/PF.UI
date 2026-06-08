using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

namespace PF.UI.ViewModels.Demos
{
    public class ImageViewerDemoViewModel : BindableBase
    {
        public ObservableCollection<DemoTocItem> TocItems { get; } = new()
        {
            new DemoTocItem { Anchor = "Basic",    Title = "基础用法",    Sub = "Uri / ImageSource 绑定，工具栏操作" },
            new DemoTocItem { Anchor = "NoToolBar",Title = "隐藏工具栏",  Sub = "ShowToolBar=False，仅保留缩放拖拽" },
            new DemoTocItem { Anchor = "MiniMap",  Title = "缩略图导航",  Sub = "ShowImgMap=True，右下角显示缩略图" },
            new DemoTocItem { Anchor = "Tip",      Title = "叠层内容",    Sub = "TipElement 附加属性，文字/自定义控件" },
        };

        private static readonly OpenFileDialog OpenFileDialog = new()
        {
            Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.webp|所有文件|*.*"
        };

        private Uri _imageUri;
        public Uri ImageUri
        {
            get => _imageUri;
            set => SetProperty(ref _imageUri, value);
        }

        public ICommand BrowseCommand { get; }

        public ImageViewerDemoViewModel()
        {
            BrowseCommand = new DelegateCommand(() =>
            {
                if (OpenFileDialog.ShowDialog() == true)
                    ImageUri = new Uri(OpenFileDialog.FileName);
            });
        }

        public const string XamlBasic = @"<!-- ImageViewer — 图片浏览器 -->
<!-- Uri 属性接受本地路径或网络地址 -->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                Height=""400""
                ShowToolBar=""True"" />

<!-- 鼠标悬停底部显示工具栏：缩放 / 旋转 / 保存 / 1:1 -->
<!-- 鼠标滚轮缩放，左键拖拽平移 -->
<!-- MoveGesture 可切换拖拽按键（默认 LeftClick）-->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                MoveGesture=""MiddleClick"" />";

        public const string XamlNoToolBar = @"<!-- ShowToolBar=False 仅保留鼠标交互 -->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                ShowToolBar=""False""
                Height=""300"" />";

        public const string XamlMiniMap = @"<!-- ShowImgMap=True 放大后右下角出现缩略图导航 -->
<!-- 可在缩略图上拖动查看区域框 -->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                ShowImgMap=""True""
                Height=""400"" />";

        public const string XamlTip = @"<!-- TipElement 叠层内容 — 纯文本 -->
<!-- Placement 枚举：LeftTop/Left/LeftBottom/TopLeft/Top/TopRight/
                    RightTop/Right/RightBottom/BottomLeft/Bottom/BottomRight -->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                pf:TipElement.Content=""图片说明文字""
                pf:TipElement.Placement=""BottomLeft""
                pf:TipElement.Visibility=""Visible"" />

<!-- 自定义 UIElement 叠层 -->
<pf:ImageViewer Uri=""{Binding ImageUri}""
                pf:TipElement.Placement=""TopRight""
                pf:TipElement.Visibility=""Visible"">
    <pf:TipElement.Content>
        <StackPanel>
            <TextBlock Text=""标题"" FontSize=""13"" FontWeight=""Bold""
                       Foreground=""{DynamicResource TextIconBrush}"" />
            <TextBlock Text=""副标题"" FontSize=""11""
                       Foreground=""{DynamicResource TextIconBrush}"" Opacity="".75"" />
        </StackPanel>
    </pf:TipElement.Content>
</pf:ImageViewer>";
    }
}
