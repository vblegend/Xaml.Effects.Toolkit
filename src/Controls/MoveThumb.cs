using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Xaml.Effects.Toolkit.Controls
{
    public class MoveThumb : Thumb
    {
        //private RotateTransform rotateTransform;
        private FrameworkElement designerItem;
        private Canvas designerCanvas;
        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }


        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as FrameworkElement;
            if (this.designerItem != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;
            }

            if (this.Host != null)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                args.RoutedEvent = UIElement.MouseLeftButtonDownEvent;
                args.Source = this;
                Host.RaiseEvent(args);
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem == null)
            {
                return;
            }

            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
            //变换
            var Transform = designerItem.RenderTransform as RotateTransform;
            if (Transform != null)
            {
                dragDelta = Transform.Transform(dragDelta);
            }




            //移动所有选中项

            var x = Canvas.GetLeft(designerItem);
            var y = Canvas.GetTop(designerItem);
            Canvas.SetLeft(designerItem, Math.Round(x + dragDelta.X, 2));
            Canvas.SetTop(designerItem, Math.Round(y + dragDelta.Y, 2));
        }


        #region Host
        public FrameworkElement Host
        {
            get { return (FrameworkElement)GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }
        public static readonly DependencyProperty HostProperty = DependencyProperty.Register("Host", typeof(FrameworkElement), typeof(MoveThumb), new PropertyMetadata(null));
        #endregion

    }
}
