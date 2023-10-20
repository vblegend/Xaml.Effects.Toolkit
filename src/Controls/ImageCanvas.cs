
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xaml.Effects.Toolkit.Uitity;

namespace Xaml.Effects.Toolkit.Controls
{
    public class ImageCanvas : Canvas
    {
        public ImageCanvas()
        {
            this.Focusable = true;
        }



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
            base.OnRender(dc);
            var pen1 = new Pen()
            {
                Brush = Brushes.White,
                DashStyle = new DashStyle(new double[] { 1, 3 }, 2),
                Thickness = 1,
            };
            var pen2 = new Pen()
            {
                Brush = Brushes.Yellow,
                DashStyle = new DashStyle(new double[] { 1, 3 }, 2),
                Thickness = 1,
            };

            if (Source != null)
            {
                var halfWidth = Math.Round(this.RenderSize.Width / 2);
                var halfHeight = Math.Round(this.RenderSize.Height / 2);


                var left = halfWidth + this.OffsetX;
                var top = halfHeight + this.OffsetY;
                dc.DrawImage(Source, new Rect(left, top, Source.Width, Source.Height));
                dc.DrawLine(pen1, new Point(0, halfHeight), new Point(this.RenderSize.Width, halfHeight));
                dc.DrawLine(pen1, new Point(halfWidth, 0), new Point(halfWidth, this.RenderSize.Height));




                dc.DrawLine(pen2, new Point(0, halfHeight + 20), new Point(this.RenderSize.Width, halfHeight + 20));
                dc.DrawLine(pen2, new Point(halfWidth + 24, 0), new Point(halfWidth + 24, this.RenderSize.Height));



                 
            }

            this.RenderSize = new Size(this.Width, this.Height);
        }


        private Point ?mousePoint { get; set; }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.mousePoint = e.GetPosition(this).Round(0);
            base.OnMouseLeftButtonDown(e);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            var point = e.GetPosition(this).Round(0);
            if (e.LeftButton == MouseButtonState.Pressed && mousePoint != null)
            {
                var p = point - this.mousePoint.Value;
                this.OffsetX += (short)p.X;
                this.OffsetY += (short)p.Y;
                this.mousePoint = point;

                this.InvalidateVisual();
            }
            base.OnMouseMove(e);
        }


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.mousePoint = null;
            base.OnMouseLeftButtonUp(e);
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {

                case Key.Up:
                    this.OffsetY--;
                    break;
                case Key.Down:
                    this.OffsetY++;
                    break;

                case Key.Left:
                    this.OffsetX--;
                    break;

                case Key.Right:
                    this.OffsetX++;
                    break;

            }

        }


    }
}
