using System;
using System.Windows;
using System.Windows.Input;
using Xaml.Effects.Toolkit.Uitity;

namespace Xaml.Effects.Toolkit.Styles.Windows
{
    public partial class VisualStudioWindow
    {
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (e.LeftButton == MouseButtonState.Pressed && mouseclickcount == 1)
            {
                var pos = e.GetPosition((UIElement)sender);
                if (win.WindowState == WindowState.Maximized)
                {
                    if (Math.Abs(title_mouseposition.X - pos.X) > 10 || Math.Abs(title_mouseposition.Y - pos.Y) > 10)
                    {
                        var mousePos = Win32Api.GetCursorPos();
                        win.WindowState = WindowState.Normal;
                        win.Left = mousePos.X - (win.Width * title_mousepos.X);
                        win.Top = mousePos.Y - title_mousepos.Y;
                        win.DragMove();
                        e.Handled = true;
                    }
                }
            }
        }
        private Int32 mouseclickcount { get; set; }
        private Point title_mouseposition { get; set; }
        private Point title_mousepos { get; set; }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouseclickcount = e.ClickCount;
                title_mouseposition = e.GetPosition((UIElement)sender);
                title_mousepos = new Point(title_mouseposition.X / win.Width, title_mouseposition.Y);
                if (e.ClickCount == 2)
                {
                    max_btn_Click(sender, e);
                }
                win.DragMove();
                e.Handled = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Win32Api.PopupSystemMenu(win);
                e.Handled = true;
            }
        }
        private void Icon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            Win32Api.PopupSystemMenu(win);
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            var setting = WindowTheme.GetTheme(win);
            if (setting == null || setting.ExitCommand == null)
            {
                win.Close();
            }
        }

        private void max_btn_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            var setting = WindowTheme.GetTheme(win);
            if (setting == null || setting.MaxButton == Visibility.Visible)
            {
                if (win.WindowState == System.Windows.WindowState.Maximized)
                {
                    win.WindowState = System.Windows.WindowState.Normal;
                    return;
                }
                win.WindowState = System.Windows.WindowState.Maximized;
                e.Handled = true;
            }


        }

        private void min_btn_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;

            var setting = WindowTheme.GetTheme(win);
            if (setting == null || setting.MinButton == Visibility.Visible)
            {
                win.WindowState = WindowState.Minimized; //设置窗口最小化
                e.Handled = true;
            }

        }
    }
}
