using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Xaml.Effects.Toolkit.Controls
{
    public class AnimationPanel : Panel
    {
        //默认动画时间
        public static readonly Duration DefDuration = new Duration(TimeSpan.FromMilliseconds(300));

        protected override Size MeasureOverride(Size availableSize)
        {
            var retSize = new Size();
            foreach (UIElement ui in InternalChildren)
            {
                ui.Measure(new Size(availableSize.Width, availableSize.Height));
                retSize.Height += ui.DesiredSize.Height;
                retSize.Width = Math.Max(retSize.Width, ui.DesiredSize.Width);
            }
            return retSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var next = new Point();
            foreach (UIElement ui in InternalChildren)
            {
                ui.Arrange(new Rect(new Point(), ui.DesiredSize));

                var transform = ui.RenderTransform as TranslateTransform;
                if (transform == null)
                    ui.RenderTransform = transform = new TranslateTransform();
                transform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(next.Y, DefDuration));
                next.Y += ui.RenderSize.Height;
            }
            return finalSize;
        }
    }
}