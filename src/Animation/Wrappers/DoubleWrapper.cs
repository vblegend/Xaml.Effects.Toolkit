using System.Windows;

namespace Xaml.Effects.Toolkit.Animation.Wrappers
{
    public class DoubleWrapper : ValueWrapper<double>
    {
        public override double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(DoubleWrapper), new PropertyMetadata(default(double)));
    }
}
