using System;
using System.Windows;

namespace Xaml.Effects.Toolkit.Controls
{
    public class RollingLabel
    {
        public RollingLabel()
        {

        }

        #region Label Content Roll Duration
        public static readonly DependencyProperty RollDurationProperty =
            DependencyProperty.RegisterAttached("RollDuration", typeof(Duration), typeof(RollingLabel),
            new FrameworkPropertyMetadata(new Duration(new TimeSpan(0, 0, 2)), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取一个对象的主题属性
        /// </summary>
        /// <param name="dpo"></param>
        /// <returns></returns>
        public static Duration GetRollDuration(DependencyObject dpo)
        {
            return (Duration)dpo.GetValue(RollDurationProperty);
        }

        /// <summary>
        /// 设置一个对象的主题属性
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value"></param>
        public static void SetRollDuration(DependencyObject dpo, Duration value)
        {
            dpo.SetValue(RollDurationProperty, value);
        }
        #endregion
    }
}
