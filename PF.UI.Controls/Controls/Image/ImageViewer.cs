using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using PF.UI.Shared.Data;
using PF.UI.Shared.Tools;

namespace PF.UI.Controls;

[TemplatePart(Name = ElementPanelMain, Type = typeof(Panel))]
[TemplatePart(Name = ElementCanvasSmallImg, Type = typeof(Canvas))]
[TemplatePart(Name = ElementBorderMove, Type = typeof(Border))]
[TemplatePart(Name = ElementBorderBottom, Type = typeof(Border))]
[TemplatePart(Name = ElementImageMain, Type = typeof(Image))]
public class ImageViewer : Control
{
    #region Constants

    private const string ElementPanelMain = "PART_PanelMain";
    private const string ElementCanvasSmallImg = "PART_CanvasSmallImg";
    private const string ElementBorderMove = "PART_BorderMove";
    private const string ElementBorderBottom = "PART_BorderBottom";
    private const string ElementImageMain = "PART_ImageMain";

    private const double ScaleInternal = 0.2;

    #endregion

    #region Data

    private static readonly SaveFileDialog SaveFileDialog = new()
    {
        Filter = "PNG 图像|*.png"
    };

    private Panel _panelMain;
    private Canvas _canvasSmallImg;
    private Border _borderMove;
    private Border _borderBottom;
    private Image _imageMain;

    private bool _borderSmallIsLoaded;
    private bool _canMoveX;
    private bool _canMoveY;
    private Thickness _imgActualMargin;
    private double _imgActualRotate;
    private double _imgActualScale = 1;
    private Point _imgCurrentPoint;
    private bool _imgIsMouseDown;
    private Thickness _imgMouseDownMargin;
    private Point _imgMouseDownPoint;
    private Point _imgSmallCurrentPoint;
    private bool _imgSmallIsMouseDown;
    private Thickness _imgSmallMouseDownMargin;
    private Point _imgSmallMouseDownPoint;
    private double _imgWidHeiScale;
    private bool _isOblique;
    private double _scaleInternalHeight;
    private double _scaleInternalWidth;
    private bool _showBorderBottom;
    private DispatcherTimer _dispatcher;
    private bool _isLoaded;
    private MouseBinding _mouseMoveBinding;

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty ShowImgMapProperty = DependencyProperty.Register(
        nameof(ShowImgMap), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.FalseBox));

    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        nameof(ImageSource), typeof(BitmapFrame), typeof(ImageViewer),
        new PropertyMetadata(default(BitmapFrame), OnImageSourceChanged));

    public static readonly DependencyProperty UriProperty = DependencyProperty.Register(
        nameof(Uri), typeof(Uri), typeof(ImageViewer),
        new PropertyMetadata(default(Uri), OnUriChanged));

    public static readonly DependencyProperty ShowToolBarProperty = DependencyProperty.Register(
        nameof(ShowToolBar), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.TrueBox));

    public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
        nameof(IsFullScreen), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.FalseBox));

    public static readonly DependencyProperty MoveGestureProperty = DependencyProperty.Register(
        nameof(MoveGesture), typeof(MouseGesture), typeof(ImageViewer),
        new UIPropertyMetadata(new MouseGesture(MouseAction.LeftClick), OnMoveGestureChanged));

    public static readonly DependencyProperty ImgPathProperty = DependencyProperty.Register(
        nameof(ImgPath), typeof(string), typeof(ImageViewer), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ImgSizeProperty = DependencyProperty.Register(
        nameof(ImgSize), typeof(long), typeof(ImageViewer), new PropertyMetadata(-1L));

    public static readonly DependencyProperty ShowFullScreenButtonProperty = DependencyProperty.Register(
        nameof(ShowFullScreenButton), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.FalseBox));

    public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(
        nameof(ShowCloseButton), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.FalseBox));

    public static readonly DependencyProperty ImageContentProperty = DependencyProperty.Register(
        nameof(ImageContent), typeof(object), typeof(ImageViewer), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register(
        nameof(ImageMargin), typeof(Thickness), typeof(ImageViewer), new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(
        nameof(ImageWidth), typeof(double), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.Double0Box));

    public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(
        nameof(ImageHeight), typeof(double), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.Double0Box));

    public static readonly DependencyProperty ImageScaleProperty = DependencyProperty.Register(
        nameof(ImageScale), typeof(double), typeof(ImageViewer),
        new PropertyMetadata(ValueBoxes.Double1Box, OnImageScaleChanged));

    public static readonly DependencyProperty ScaleStrProperty = DependencyProperty.Register(
        nameof(ScaleStr), typeof(string), typeof(ImageViewer), new PropertyMetadata("100%"));

    public static readonly DependencyProperty ImageRotateProperty = DependencyProperty.Register(
        nameof(ImageRotate), typeof(double), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.Double0Box));

    public static readonly DependencyProperty ShowSmallImgInternalProperty = DependencyProperty.Register(
        nameof(ShowSmallImgInternal), typeof(bool), typeof(ImageViewer), new PropertyMetadata(ValueBoxes.FalseBox));

    #endregion

    #region Properties

    public bool IsFullScreen
    {
        get => (bool)GetValue(IsFullScreenProperty);
        set => SetValue(IsFullScreenProperty, ValueBoxes.BooleanBox(value));
    }

    [ValueSerializer(typeof(MouseGestureValueSerializer))]
    [TypeConverter(typeof(MouseGestureConverter))]
    public MouseGesture MoveGesture
    {
        get => (MouseGesture)GetValue(MoveGestureProperty);
        set => SetValue(MoveGestureProperty, value);
    }

    public bool ShowImgMap
    {
        get => (bool)GetValue(ShowImgMapProperty);
        set => SetValue(ShowImgMapProperty, ValueBoxes.BooleanBox(value));
    }

    public BitmapFrame ImageSource
    {
        get => (BitmapFrame)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public Uri Uri
    {
        get => (Uri)GetValue(UriProperty);
        set => SetValue(UriProperty, value);
    }

    public bool ShowToolBar
    {
        get => (bool)GetValue(ShowToolBarProperty);
        set => SetValue(ShowToolBarProperty, ValueBoxes.BooleanBox(value));
    }

    public object ImageContent
    {
        get => GetValue(ImageContentProperty);
        set => SetValue(ImageContentProperty, value);
    }

    public string ImgPath
    {
        get => (string)GetValue(ImgPathProperty);
        set => SetValue(ImgPathProperty, value);
    }

    public long ImgSize
    {
        get => (long)GetValue(ImgSizeProperty);
        set => SetValue(ImgSizeProperty, value);
    }

    public bool ShowFullScreenButton
    {
        get => (bool)GetValue(ShowFullScreenButtonProperty);
        set => SetValue(ShowFullScreenButtonProperty, ValueBoxes.BooleanBox(value));
    }

    public Thickness ImageMargin
    {
        get => (Thickness)GetValue(ImageMarginProperty);
        set => SetValue(ImageMarginProperty, value);
    }

    public double ImageWidth
    {
        get => (double)GetValue(ImageWidthProperty);
        set => SetValue(ImageWidthProperty, value);
    }

    public double ImageHeight
    {
        get => (double)GetValue(ImageHeightProperty);
        set => SetValue(ImageHeightProperty, value);
    }

    public double ImageScale
    {
        get => (double)GetValue(ImageScaleProperty);
        set => SetValue(ImageScaleProperty, value);
    }

    public string ScaleStr
    {
        get => (string)GetValue(ScaleStrProperty);
        set => SetValue(ScaleStrProperty, value);
    }

    public double ImageRotate
    {
        get => (double)GetValue(ImageRotateProperty);
        set => SetValue(ImageRotateProperty, value);
    }

    public bool ShowSmallImgInternal
    {
        get => (bool)GetValue(ShowSmallImgInternalProperty);
        set => SetValue(ShowSmallImgInternalProperty, ValueBoxes.BooleanBox(value));
    }

    private double ImageOriWidth { get; set; }
    private double ImageOriHeight { get; set; }

    public bool ShowCloseButton
    {
        get => (bool)GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, ValueBoxes.BooleanBox(value));
    }

    internal bool ShowBorderBottom
    {
        get => _showBorderBottom;
        set
        {
            if (_showBorderBottom == value) return;
            _borderBottom?.BeginAnimation(OpacityProperty,
                value
                    ? AnimationHelper.CreateAnimation(1, 100)
                    : AnimationHelper.CreateAnimation(0, 400));
            _showBorderBottom = value;
        }
    }

    #endregion

    #region Constructor

    public ImageViewer()
    {
        CommandBindings.Add(new CommandBinding(ControlCommands.Save, ButtonSave_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.Open, ButtonOpen_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.Restore, ButtonActual_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.Reduce, ButtonReduce_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.Enlarge, ButtonEnlarge_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.RotateLeft, ButtonRotateLeft_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.RotateRight, ButtonRotateRight_OnClick));
        CommandBindings.Add(new CommandBinding(ControlCommands.MouseMove, ImageMain_OnMouseDown));
        OnMoveGestureChanged(MoveGesture);

        Loaded += (s, e) =>
        {
            _isLoaded = true;
            Init();
        };
    }

    public ImageViewer(Uri uri) : this() => Uri = uri;

    public ImageViewer(string path) : this(new Uri(path)) { }

    #endregion

    public override void OnApplyTemplate()
    {
        if (_canvasSmallImg != null)
        {
            _canvasSmallImg.MouseLeftButtonDown -= CanvasSmallImg_OnMouseLeftButtonDown;
            _canvasSmallImg.MouseLeftButtonUp -= CanvasSmallImg_OnMouseLeftButtonUp;
            _canvasSmallImg.MouseMove -= CanvasSmallImg_OnMouseMove;
        }

        base.OnApplyTemplate();

        _panelMain = GetTemplateChild(ElementPanelMain) as Panel;
        _canvasSmallImg = GetTemplateChild(ElementCanvasSmallImg) as Canvas;
        _borderMove = GetTemplateChild(ElementBorderMove) as Border;
        _imageMain = GetTemplateChild(ElementImageMain) as Image;
        _borderBottom = GetTemplateChild(ElementBorderBottom) as Border;

        if (_imageMain != null)
        {
            var t = new RotateTransform();
            BindingOperations.SetBinding(t, RotateTransform.AngleProperty,
                new Binding(ImageRotateProperty.Name) { Source = this });
            _imageMain.LayoutTransform = t;
        }

        if (_canvasSmallImg != null)
        {
            _canvasSmallImg.MouseLeftButtonDown += CanvasSmallImg_OnMouseLeftButtonDown;
            _canvasSmallImg.MouseLeftButtonUp += CanvasSmallImg_OnMouseLeftButtonUp;
            _canvasSmallImg.MouseMove += CanvasSmallImg_OnMouseMove;
        }

        _borderSmallIsLoaded = false;
    }

    private static void OnImageScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageViewer iv && e.NewValue is double v)
        {
            iv.ImageWidth = iv.ImageOriWidth * v;
            iv.ImageHeight = iv.ImageOriHeight * v;
            iv.ScaleStr = $"{v * 100:#0}%";
        }
    }

    private void Init()
    {
        if (ImageSource == null || !_isLoaded) return;

        if (ImageSource.IsDownloading)
        {
            _dispatcher = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _dispatcher.Tick += Dispatcher_Tick;
            _dispatcher.Start();
            return;
        }

        double width, height;
        if (!_isOblique)
        {
            width = ImageSource.PixelWidth;
            height = ImageSource.PixelHeight;
        }
        else
        {
            width = ImageSource.PixelHeight;
            height = ImageSource.PixelWidth;
        }

        ImageWidth = width;
        ImageHeight = height;
        ImageOriWidth = width;
        ImageOriHeight = height;
        _scaleInternalWidth = ImageOriWidth * ScaleInternal;
        _scaleInternalHeight = ImageOriHeight * ScaleInternal;

        if (Math.Abs(height) < 0.001 || Math.Abs(width) < 0.001)
        {
            MessageBox.Show("图片尺寸无效");
            return;
        }

        _imgWidHeiScale = width / height;
        var scaleWindow = ActualWidth / ActualHeight;
        ImageScale = 1;

        if (_imgWidHeiScale > scaleWindow)
        {
            if (width > ActualWidth)
                ImageScale = ActualWidth / width;
        }
        else if (height > ActualHeight)
        {
            ImageScale = ActualHeight / height;
        }

        ImageMargin = new Thickness((ActualWidth - ImageWidth) / 2, (ActualHeight - ImageHeight) / 2, 0, 0);
        _imgActualScale = ImageScale;
        _imgActualMargin = ImageMargin;

        InitBorderSmall();
    }

    private void Dispatcher_Tick(object sender, EventArgs e)
    {
        if (_dispatcher == null) return;
        if (ImageSource == null || !_isLoaded)
        {
            _dispatcher.Stop();
            _dispatcher.Tick -= Dispatcher_Tick;
            _dispatcher = null;
            return;
        }
        if (!ImageSource.IsDownloading)
        {
            _dispatcher.Stop();
            _dispatcher.Tick -= Dispatcher_Tick;
            _dispatcher = null;
            Init();
        }
    }

    private void ButtonActual_OnClick(object sender, RoutedEventArgs e)
    {
        var scaleAnimation = AnimationHelper.CreateAnimation(1);
        scaleAnimation.FillBehavior = FillBehavior.Stop;
        _imgActualScale = 1;
        scaleAnimation.Completed += (s, e1) =>
        {
            ImageScale = 1;
            _canMoveX = ImageWidth > ActualWidth;
            _canMoveY = ImageHeight > ActualHeight;
            BorderSmallShowSwitch();
        };

        var thickness = new Thickness((ActualWidth - ImageOriWidth) / 2, (ActualHeight - ImageOriHeight) / 2, 0, 0);
        var marginAnimation = AnimationHelper.CreateAnimation(thickness);
        marginAnimation.FillBehavior = FillBehavior.Stop;
        _imgActualMargin = thickness;
        marginAnimation.Completed += (s, e1) => { ImageMargin = thickness; };

        BeginAnimation(ImageScaleProperty, scaleAnimation);
        BeginAnimation(ImageMarginProperty, marginAnimation);
    }

    private void ButtonReduce_OnClick(object sender, RoutedEventArgs e) => ScaleImg(false);
    private void ButtonEnlarge_OnClick(object sender, RoutedEventArgs e) => ScaleImg(true);
    private void ButtonRotateLeft_OnClick(object sender, RoutedEventArgs e) => RotateImg(_imgActualRotate - 90);
    private void ButtonRotateRight_OnClick(object sender, RoutedEventArgs e) => RotateImg(_imgActualRotate + 90);

    private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
    {
        if (ImageSource == null) return;
        SaveFileDialog.FileName = $"{DateTime.Now:yyyy-M-d-H-m-s-fff}";
        if (SaveFileDialog.ShowDialog() == true)
        {
            using var fs = new FileStream(SaveFileDialog.FileName, FileMode.Create, FileAccess.Write);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(ImageSource));
            encoder.Save(fs);
        }
    }

    private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ImgPath) && File.Exists(ImgPath))
        {
            try { Process.Start(new ProcessStartInfo(ImgPath) { UseShellExecute = true }); }
            catch { /* ignore */ }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e) => MoveImg();
    protected override void OnMouseLeave(MouseEventArgs e) => ShowBorderBottom = false;
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        ScaleImg(e.Delta > 0);
        e.Handled = true;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        OnRenderSizeChanged();
    }

    private void OnRenderSizeChanged()
    {
        if (ImageWidth < 0.001 || ImageHeight < 0.001) return;

        _canMoveX = true;
        _canMoveY = true;

        var marginX = ImageMargin.Left;
        var marginY = ImageMargin.Top;

        if (ImageWidth <= ActualWidth)
        {
            _canMoveX = false;
            marginX = (ActualWidth - ImageWidth) / 2;
        }

        if (ImageHeight <= ActualHeight)
        {
            _canMoveY = false;
            marginY = (ActualHeight - ImageHeight) / 2;
        }

        ImageMargin = new Thickness(marginX, marginY, 0, 0);
        _imgActualMargin = ImageMargin;

        BorderSmallShowSwitch();
        _imgSmallMouseDownMargin = _borderMove.Margin;
        MoveSmallImg(_imgSmallMouseDownMargin.Left, _imgSmallMouseDownMargin.Top);
    }

    private void ImageMain_OnMouseDown(object sender, ExecutedRoutedEventArgs e)
    {
        _imgMouseDownPoint = Mouse.GetPosition(_panelMain);
        _imgMouseDownMargin = ImageMargin;
        _imgIsMouseDown = true;
    }

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e) => _imgIsMouseDown = false;

    private void BorderSmallShowSwitch()
    {
        if (_canMoveX || _canMoveY)
        {
            if (!_borderSmallIsLoaded)
            {
                _canvasSmallImg.Background = new VisualBrush(_imageMain);
                InitBorderSmall();
                _borderSmallIsLoaded = true;
            }
            ShowSmallImgInternal = true;
            UpdateBorderSmall();
        }
        else
        {
            ShowSmallImgInternal = false;
        }
    }

    private void InitBorderSmall()
    {
        if (_canvasSmallImg == null) return;
        var scaleWindow = _canvasSmallImg.MaxWidth / _canvasSmallImg.MaxHeight;
        if (_imgWidHeiScale > scaleWindow)
        {
            _canvasSmallImg.Width = _canvasSmallImg.MaxWidth;
            _canvasSmallImg.Height = _canvasSmallImg.Width / _imgWidHeiScale;
        }
        else
        {
            _canvasSmallImg.Width = _canvasSmallImg.MaxHeight * _imgWidHeiScale;
            _canvasSmallImg.Height = _canvasSmallImg.MaxHeight;
        }
    }

    private void UpdateBorderSmall()
    {
        if (!ShowSmallImgInternal) return;

        var widthMin = Math.Min(ImageWidth, ActualWidth);
        var heightMin = Math.Min(ImageHeight, ActualHeight);

        _borderMove.Width = widthMin / ImageWidth * _canvasSmallImg.Width;
        _borderMove.Height = heightMin / ImageHeight * _canvasSmallImg.Height;

        var marginX = -ImageMargin.Left / ImageWidth * _canvasSmallImg.Width;
        var marginY = -ImageMargin.Top / ImageHeight * _canvasSmallImg.Height;

        var marginXMax = _canvasSmallImg.Width - _borderMove.Width;
        var marginYMax = _canvasSmallImg.Height - _borderMove.Height;

        marginX = Math.Max(0, Math.Min(marginXMax, marginX));
        marginY = Math.Max(0, Math.Min(marginYMax, marginY));

        _borderMove.Margin = new Thickness(marginX, marginY, 0, 0);
    }

    private void ScaleImg(bool isEnlarge)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed) return;

        var oldImageWidth = ImageWidth;
        var oldImageHeight = ImageHeight;

        var tempScale = isEnlarge ? _imgActualScale + ScaleInternal : _imgActualScale - ScaleInternal;
        tempScale = Math.Max(ScaleInternal, Math.Min(50, tempScale));

        ImageScale = tempScale;

        var posCanvas = Mouse.GetPosition(_panelMain);
        var posImg = new Point(posCanvas.X - _imgActualMargin.Left, posCanvas.Y - _imgActualMargin.Top);

        var marginX = .5 * _scaleInternalWidth;
        var marginY = .5 * _scaleInternalHeight;

        if (ImageWidth > ActualWidth)
        {
            _canMoveX = true;
            if (ImageHeight > ActualHeight)
            {
                _canMoveY = true;
                marginX = posImg.X / oldImageWidth * _scaleInternalWidth;
                marginY = posImg.Y / oldImageHeight * _scaleInternalHeight;
            }
            else
            {
                _canMoveY = false;
            }
        }
        else
        {
            _canMoveY = ImageHeight > ActualHeight;
            _canMoveX = false;
        }

        Thickness thickness;
        if (isEnlarge)
        {
            thickness = new Thickness(_imgActualMargin.Left - marginX, _imgActualMargin.Top - marginY, 0, 0);
        }
        else
        {
            var marginActualX = _imgActualMargin.Left + marginX;
            var marginActualY = _imgActualMargin.Top + marginY;
            var subX = ImageWidth - ActualWidth;
            var subY = ImageHeight - ActualHeight;

            var right = Math.Abs(_borderMove.Width - _canvasSmallImg.ActualWidth + _borderMove.Margin.Left);
            var top = Math.Abs(_borderMove.Height - _canvasSmallImg.ActualHeight + _borderMove.Margin.Top);

            if (Math.Abs(ImageMargin.Left) < 0.001 || right < 0.001)
                marginActualX = _imgActualMargin.Left + _borderMove.Margin.Left /
                    (_canvasSmallImg.ActualWidth - _borderMove.Width) * _scaleInternalWidth;
            if (Math.Abs(ImageMargin.Top) < 0.001 || top < 0.001)
                marginActualY = _imgActualMargin.Top + _borderMove.Margin.Top /
                    (_canvasSmallImg.ActualHeight - _borderMove.Height) * _scaleInternalHeight;

            if (subX < 0.001) marginActualX = (ActualWidth - ImageWidth) / 2;
            if (subY < 0.001) marginActualY = (ActualHeight - ImageHeight) / 2;

            thickness = new Thickness(marginActualX, marginActualY, 0, 0);
        }

        ImageMargin = thickness;
        _imgActualScale = tempScale;
        _imgActualMargin = thickness;
        BorderSmallShowSwitch();

        _imgSmallMouseDownMargin = _borderMove.Margin;
        MoveSmallImg(_imgSmallMouseDownMargin.Left, _imgSmallMouseDownMargin.Top);
    }

    private void RotateImg(double rotate)
    {
        _imgActualRotate = rotate;
        _isOblique = ((int)_imgActualRotate - 90) % 180 == 0;
        ShowSmallImgInternal = false;
        Init();
        InitBorderSmall();

        var animation = AnimationHelper.CreateAnimation(rotate);
        animation.Completed += (s, e1) => { ImageRotate = rotate; };
        animation.FillBehavior = FillBehavior.Stop;
        BeginAnimation(ImageRotateProperty, animation);
    }

    private MouseButtonState GetMouseButtonState() => MoveGesture.MouseAction switch
    {
        MouseAction.LeftClick => Mouse.LeftButton,
        MouseAction.RightClick => Mouse.RightButton,
        MouseAction.MiddleClick => Mouse.MiddleButton,
        _ => Mouse.LeftButton
    };

    private void MoveImg()
    {
        _imgCurrentPoint = Mouse.GetPosition(_panelMain);
        ShowCloseButton = _imgCurrentPoint.Y < 200;
        ShowBorderBottom = _imgCurrentPoint.Y > ActualHeight - 200;

        if (GetMouseButtonState() == MouseButtonState.Released) return;

        if (_imgIsMouseDown)
        {
            var subX = _imgCurrentPoint.X - _imgMouseDownPoint.X;
            var subY = _imgCurrentPoint.Y - _imgMouseDownPoint.Y;

            var marginX = _imgMouseDownMargin.Left;
            if (ImageWidth > ActualWidth)
            {
                marginX = _imgMouseDownMargin.Left + subX;
                if (marginX >= 0) marginX = 0;
                else if (-marginX + ActualWidth >= ImageWidth) marginX = ActualWidth - ImageWidth;
                _canMoveX = true;
            }

            var marginY = _imgMouseDownMargin.Top;
            if (ImageHeight > ActualHeight)
            {
                marginY = _imgMouseDownMargin.Top + subY;
                if (marginY >= 0) marginY = 0;
                else if (-marginY + ActualHeight >= ImageHeight) marginY = ActualHeight - ImageHeight;
                _canMoveY = true;
            }

            ImageMargin = new Thickness(marginX, marginY, 0, 0);
            _imgActualMargin = ImageMargin;
            UpdateBorderSmall();
        }
    }

    private void MoveSmallImg()
    {
        if (!_imgSmallIsMouseDown) return;
        if (GetMouseButtonState() == MouseButtonState.Released) return;

        _imgSmallCurrentPoint = Mouse.GetPosition(_canvasSmallImg);
        var subX = _imgSmallCurrentPoint.X - _imgSmallMouseDownPoint.X;
        var subY = _imgSmallCurrentPoint.Y - _imgSmallMouseDownPoint.Y;

        MoveSmallImg(_imgSmallMouseDownMargin.Left + subX, _imgSmallMouseDownMargin.Top + subY);
    }

    private void MoveSmallImg(double marginX, double marginY)
    {
        // NaN-safe clamp: when canvas is not yet sized, NaN comparisons silently skip
        if (marginX < 0)
            marginX = 0;
        else if (marginX + _borderMove.Width >= _canvasSmallImg.Width)
            marginX = _canvasSmallImg.Width - _borderMove.Width;

        if (marginY < 0)
            marginY = 0;
        else if (marginY + _borderMove.Height >= _canvasSmallImg.Height)
            marginY = _canvasSmallImg.Height - _borderMove.Height;

        _borderMove.Margin = new Thickness(marginX, marginY, 0, 0);

        var marginActualX = (ActualWidth - ImageWidth) / 2;
        var marginActualY = (ActualHeight - ImageHeight) / 2;

        if (_canMoveX) marginActualX = -marginX / _canvasSmallImg.Width * ImageWidth;
        if (_canMoveY) marginActualY = -marginY / _canvasSmallImg.Height * ImageHeight;

        ImageMargin = new Thickness(marginActualX, marginActualY, 0, 0);
        _imgActualMargin = ImageMargin;
    }

    private void CanvasSmallImg_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _imgSmallMouseDownPoint = Mouse.GetPosition(_canvasSmallImg);
        _imgSmallMouseDownMargin = _borderMove.Margin;
        _imgSmallIsMouseDown = true;
        e.Handled = true;
    }

    private void CanvasSmallImg_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        => _imgSmallIsMouseDown = false;

    private void CanvasSmallImg_OnMouseMove(object sender, MouseEventArgs e) => MoveSmallImg();

    private static void OnMoveGestureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((ImageViewer)d).OnMoveGestureChanged((MouseGesture)e.NewValue);

    private void OnMoveGestureChanged(MouseGesture newValue)
    {
        InputBindings.Remove(_mouseMoveBinding);
        _mouseMoveBinding = new MouseBinding(ControlCommands.MouseMove, newValue);
        InputBindings.Add(_mouseMoveBinding);
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((ImageViewer)d).Init();

    private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((ImageViewer)d).OnUriChanged((Uri)e.NewValue);

    private void OnUriChanged(Uri newValue)
    {
        ImageSource = newValue is not null ? TryGetBitmapFrame(newValue) : null;

        if (ImageSource is not null && newValue.IsAbsoluteUri)
        {
            ImgPath = newValue.AbsolutePath;
            ImgSize = File.Exists(ImgPath) ? new FileInfo(ImgPath).Length : 0;
        }
        else
        {
            ImgPath = string.Empty;
            ImgSize = 0;
        }
    }

    private static BitmapFrame TryGetBitmapFrame(Uri source)
    {
        try { return BitmapFrame.Create(source); }
        catch { return null; }
    }
}
