using System;
using System.Windows;
using System.Windows.Controls;

namespace Xaml.Effects.Toolkit.Controls
{
    /// <summary>
    /// BusyBox.xaml 的交互逻辑
    /// </summary>
    public partial class BusyBox : UserControl
    {
        public BusyBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(BusyBox), new PropertyMetadata(false, OnIsActiveSourceChanged));

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (bool)GetValue(IsActiveProperty);
            }
            set
            {
                SetValue(IsActiveProperty, value);
                this.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void OnIsActiveSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var box = (BusyBox)o;
            box.IsActive = (Boolean)e.NewValue;
        }
    }
}
