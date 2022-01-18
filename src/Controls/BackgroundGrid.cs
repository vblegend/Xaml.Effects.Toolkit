using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xaml.Effects.Toolkit.Controls
{

    public enum BackgroundStyle
    {
        SolidColor,
        Image
    }


    /// <summary>
    /// 九宫格背景Border
    /// </summary>
    public class BackgroundGrid : Grid
    {


        /// <summary>
        /// 图片源
        /// </summary>
        public BackgroundStyle BackgroundStyle
        {
            get { return (BackgroundStyle)base.GetValue(BackgroundStyleProperty); }
            set { base.SetValue(BackgroundStyleProperty, value); }
        }

        /// <summary>
        /// 图片源
        /// </summary>
        public static readonly DependencyProperty BackgroundStyleProperty = DependencyProperty.Register("BackgroundStyle",
        typeof(BackgroundStyle), typeof(BackgroundGrid),
        new FrameworkPropertyMetadata(BackgroundStyle.SolidColor, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// 图片四个边距
        /// </summary>
        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        /// <summary>
        /// 图片四个边距
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty =
        DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(BackgroundGrid),
        new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// 是否九宫格方式
        /// </summary>
        private bool IsNineGrid
        {
            get { return !ImageMargin.Equals(new Thickness()); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawImage(dc, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }

        private void DrawImage(DrawingContext dc, Rect rect)
        {
            if (BackgroundStyle == BackgroundStyle.SolidColor)
            {
                if (this.Background is SolidColorBrush)
                {
                    dc.DrawRectangle(Background, null, rect);
                }
            }
            else if (BackgroundStyle == BackgroundStyle.Image)
            {
                if (this.Background is ImageBrush imageBrush)
                {
                    ImageSource source = imageBrush.ImageSource;
                    if (source != null)
                    {

                        if (IsNineGrid)
                        {
                            Thickness margin = Clamp(ImageMargin, new Size(source.Width, source.Height), rect.Size);
                            double[] xGuidelines = { 0, margin.Left, rect.Width - margin.Right, rect.Width };
                            double[] yGuidelines = { 0, margin.Top, rect.Height - margin.Bottom, rect.Height };
                            GuidelineSet guidelineSet = new GuidelineSet(xGuidelines, yGuidelines);
                            guidelineSet.Freeze();
                            dc.PushGuidelineSet(guidelineSet);
                            double[] vx = { 0D, margin.Left / source.Width, (source.Width - margin.Right) / source.Width, 1D };
                            double[] vy = { 0D, margin.Top / source.Height, (source.Height - margin.Bottom) / source.Height, 1D };
                            double[] x = { rect.Left, rect.Left + margin.Left, rect.Right - margin.Right, rect.Right };
                            double[] y = { rect.Top, rect.Top + margin.Top, rect.Bottom - margin.Bottom, rect.Bottom };
                            for (int i = 0; i < 3; ++i)
                            {
                                for (int j = 0; j < 3; ++j)
                                {
                                    ImageBrush brush = new ImageBrush(source);
                                    brush.Viewbox = new Rect(vx[j], vy[i], Math.Max(0D, (vx[j + 1] - vx[j])),
                                    Math.Max(0D, (vy[i + 1] - vy[i])));

                                    dc.DrawRectangle(brush, null,
                                    new Rect(x[j], y[i], Math.Max(0D, (x[j + 1] - x[j])), Math.Max(0D, (y[i + 1] - y[i]))));
                                }
                            }
                            dc.Pop();
                        }
                        else
                        {
                            ImageBrush brush = new ImageBrush(source);
                            dc.DrawRectangle(brush, null, rect);
                        }
                    }
                }
                else
                {
                    Pen p = (Pen)this.FindResource("ERROR_STROKE_PEN");
                    dc.DrawRectangle(null, p, rect);
                    dc.DrawLine(p, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Bottom));
                    dc.DrawLine(p, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Top));
                }
            }
        }

        private static Thickness Clamp(Thickness margin, Size firstMax, Size secondMax)
        {
            double left = Clamp(margin.Left, firstMax.Width, secondMax.Width);
            double top = Clamp(margin.Top, firstMax.Height, secondMax.Height);
            double right = Clamp(margin.Right, firstMax.Width - left, secondMax.Width - left);
            double bottom = Clamp(margin.Bottom, firstMax.Height - top, secondMax.Height - top);

            return new Thickness(left, top, right, bottom);
        }

        private static double Clamp(double value, double firstMax, double secondMax)
        {
            return Math.Max(0, Math.Min(Math.Min(value, firstMax), secondMax));
        }
    }
}
