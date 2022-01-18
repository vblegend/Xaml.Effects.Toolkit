using System;
using System.Windows;

namespace Xaml.Effects.Toolkit.Styles.Windows
{
    public abstract class IService : Freezable
    {

        /// <summary>
        /// 当前服务所属窗口
        /// </summary>
        protected Window Window { get; private set; }

        internal void Attach(Window window)
        {
            this.Initialize(window);
        }

        internal IntPtr RouteMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return this.WindowProc(hwnd, msg, wParam, lParam, ref handled);
        }

        internal void UnAttach()
        {
            this.UnInitialize();
        }







        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="window"></param>
        protected virtual void Initialize(Window window)
        {
            this.Window = window;
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// 卸载对象
        /// </summary>
        /// <param name="window"></param>
        protected virtual void UnInitialize()
        {

        }

        protected override Freezable CreateInstanceCore()
        {
            Type type = base.GetType();
            return (Freezable)Activator.CreateInstance(type);
        }
    }
}
