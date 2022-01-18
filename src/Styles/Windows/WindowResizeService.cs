using Xaml.Effects.Toolkit.Uitity;
using System;
using System.Windows;
using Xaml.Effects.Toolkit.Styles.Windows;


namespace Xaml.Effects.Toolkit.Styles.Windows
{
    public class WindowResizeService : IService
    {
        public WindowResizeService()
        {
            mousePoint = new Point();
            ResizeThickness = 4;
        }

        protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32Api.WM_NCHITTEST)
            {
                Win32Api.RECT rect = Win32Api.GetWindowRect(hwnd);
                this.mousePoint.X = (lParam.ToInt32() & 0xFFFF);
                this.mousePoint.Y = (lParam.ToInt32() >> 16);
                ////告诉系统你已经处理过该消息，不然设置为false
                //窗口左上角
                if (this.mousePoint.Y - rect.top <= this.agWidth
                                 && this.mousePoint.X - rect.left <= this.agWidth)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTTOPLEFT);
                }
                // 窗口左下角　　
                else if (rect.Height + rect.top - this.mousePoint.Y <= this.agWidth
                                 && this.mousePoint.X - rect.left <= this.agWidth)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTBOTTOMLEFT);
                }
                // 窗口右上角
                else if (this.mousePoint.Y - rect.top <= this.agWidth
                 && rect.Width + rect.left - this.mousePoint.X <= this.agWidth)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTTOPRIGHT);
                }
                // 窗口右下角
                else if (rect.Width + rect.left - this.mousePoint.X <= this.agWidth
                 && rect.Height + rect.top - this.mousePoint.Y <= this.agWidth)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTBOTTOMRIGHT);
                }
                // 窗口左侧
                else if (this.mousePoint.X - rect.left <= this.bThickness)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTLEFT);
                }
                // 窗口右侧
                else if (rect.Width + rect.left - this.mousePoint.X <= this.bThickness)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTRIGHT);
                }
                // 窗口上方
                else if (this.mousePoint.Y - rect.top <= this.bThickness)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTTOP);
                }
                // 窗口下方
                else if (rect.Height + rect.top - this.mousePoint.Y <= this.bThickness)
                {
                    handled = true;
                    return new IntPtr((int)Win32Api.HitTest.HTBOTTOM);
                }
            }



            return base.WindowProc(hwnd, msg, wParam, lParam, ref handled);
        }



        /// <summary>
        /// 拐角宽度
        /// </summary>
        private Int32 agWidth
        {

            get
            {
                return ResizeThickness * 3;
            }
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        private Int32 bThickness
        {

            get
            {
                return ResizeThickness;
            }
        }





        #region ResizeThickness

        public static readonly DependencyProperty ResizeThicknessProperty =
        DependencyProperty.Register("ResizeThickness", typeof(Int32), typeof(WindowResizeService),
        new PropertyMetadata(4, ResizeThickness_Changle));

        private static void ResizeThickness_Changle(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {

        }
        /// <summary>
        /// 窗口调整区域厚度
        /// </summary>
        public Int32 ResizeThickness
        {
            get
            {
                return (Int32)this.GetValue(ResizeThicknessProperty);
            }
            set
            {
                this.SetValue(ResizeThicknessProperty, value);
            }
        }


        #endregion






        private Point mousePoint;

    }
}
