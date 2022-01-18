using System.Windows;

namespace Xaml.Effects.Toolkit.Animation.Wrappers
{
    public class ProgressWrapper : DependencyObject
    {

        /// <summary>
        /// 获取或设置Progress的值
        /// </summary>  
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// 标识 Progress 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressWrapper), new PropertyMetadata(0d));
    }

}
