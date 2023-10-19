
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Xaml.Effects.Toolkit.Controls
{
    public class ImageCanvas : Canvas
    {
        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageCanvas), new FrameworkPropertyMetadata(default(ImageSource), PropertyChangedCallback));


        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ImageCanvas).InvalidateVisual();
        }



        public Int16 OffsetX
        {
            get
            {
                return (Int16)GetValue(OffsetXProperty);
            }
            set
            {
                SetValue(OffsetXProperty, value);
            }
        }
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(Int16), typeof(ImageCanvas), new FrameworkPropertyMetadata(default(Int16), PropertyChangedCallback));


        public Int16 OffsetY
        {
            get
            {
                return (Int16)GetValue(OffsetYProperty);
            }
            set
            {
                SetValue(OffsetYProperty, value);
            }
        }
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(Int16), typeof(ImageCanvas), new FrameworkPropertyMetadata(default(Int16), PropertyChangedCallback));



        protected override void OnRender(DrawingContext dc)
        {
            var pen = new Pen()
            {
                Brush = Brushes.White,
                DashStyle = new DashStyle(new double[] { 1, 3 }, 2),
                Thickness = 1,
            };
            if (Source != null)
            {
                var left = this.RenderSize.Width / 2 + this.OffsetX;
                var top = this.RenderSize.Height / 2 + this.OffsetY;
                dc.DrawImage(Source, new Rect(left, top, Source.Width, Source.Height));
                dc.DrawLine(pen, new Point(0, this.RenderSize.Height / 2), new Point(this.RenderSize.Width, this.RenderSize.Height / 2));
                dc.DrawLine(pen, new Point(this.RenderSize.Width / 2, 0), new Point(this.RenderSize.Width / 2, this.RenderSize.Height));
            }

            base.OnRender(dc);
        }
    }
}
