using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace PF.UI.Controls;

public class LoadingLine : LoadingBase
{
    private const double MoveLength = 80;

    private const double UniformScale = .6;

    public LoadingLine()
    {
        SetBinding(HeightProperty, new Binding("DotDiameter") { Source = this });
    }

    protected sealed override void UpdateDots()
    {
        var dotCount = DotCount;
        var dotInterval = DotInterval;
        var dotDiameter = DotDiameter;
        var dotSpeed = DotSpeed;
        var dotDelayTime = DotDelayTime;

        if (dotCount < 1) return;
        PrivateCanvas.Children.Clear();

        //计算相关尺寸
        var centerWidth = dotDiameter * dotCount + dotInterval * (dotCount - 1) + MoveLength;
        var speedDownLength = (ActualWidth - MoveLength) / 2;
        var speedUniformLength = centerWidth / 2;

        //定义动画
        Storyboard = new Storyboard
        {
            RepeatBehavior = RepeatBehavior.Forever
        };

        //创建圆点
        for (var i = 0; i < dotCount; i++)
        {
            var ellipse = CreateEllipse(i, dotInterval, dotDiameter);

            // 位移动画作用于 RenderTransform，走渲染线程的独立动画，不占用 UI 线程做逐帧布局，
            // 避免动画 Margin（依赖动画）导致的掉帧。起始位置（错位堆叠）仍由 Margin 静态设置一次。
            var frames = new DoubleAnimationUsingKeyFrames
            {
                BeginTime = TimeSpan.FromMilliseconds(dotDelayTime * i)
            };
            //开始位置
            var frame0 = new LinearDoubleKeyFrame
            {
                Value = 0,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero)
            };

            //开始位置到匀速开始
            var frame1 = new EasingDoubleKeyFrame
            {
                EasingFunction = new PowerEase
                {
                    EasingMode = EasingMode.EaseOut
                },
                Value = speedDownLength,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (1 - UniformScale) / 2))
            };

            //匀速开始到匀速结束
            var frame2 = new LinearDoubleKeyFrame
            {
                Value = speedDownLength + speedUniformLength,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed * (1 + UniformScale) / 2))
            };

            //匀速结束到匀加速结束
            var frame3 = new EasingDoubleKeyFrame
            {
                EasingFunction = new PowerEase
                {
                    EasingMode = EasingMode.EaseIn
                },
                Value = ActualWidth + speedUniformLength,
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(dotSpeed))
            };

            frames.KeyFrames.Add(frame0);
            frames.KeyFrames.Add(frame1);
            frames.KeyFrames.Add(frame2);
            frames.KeyFrames.Add(frame3);

            Storyboard.SetTarget(frames, ellipse);
            Storyboard.SetTargetProperty(frames,
                new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.Children.Add(frames);

            PrivateCanvas.Children.Add(ellipse);
        }

        Storyboard.Begin();
        if (!IsRunning)
        {
            Storyboard.Pause();
        }
    }

    private Ellipse CreateEllipse(int index, double dotInterval, double dotDiameter)
    {
        var ellipse = base.CreateEllipse(index);
        ellipse.HorizontalAlignment = HorizontalAlignment.Left;
        ellipse.VerticalAlignment = VerticalAlignment.Top;
        ellipse.Margin = new Thickness(-(dotInterval + dotDiameter) * index, 0, 0, 0);
        ellipse.RenderTransform = new TranslateTransform();
        return ellipse;
    }
}
