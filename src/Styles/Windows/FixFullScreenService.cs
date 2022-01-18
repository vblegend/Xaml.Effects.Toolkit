using System;
using Xaml.Effects.Toolkit.Uitity;

namespace Xaml.Effects.Toolkit.Styles.Windows
{
    public class FixFullScreenService : IService
    {
        protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Win32Api.WM_GETMINMAXINFO)
            {
                Win32Api.GetMinMaxInfo(hwnd, lParam);
                handled = true;
            }
            return base.WindowProc(hwnd, msg, wParam, lParam, ref handled);
        }

    }
}
