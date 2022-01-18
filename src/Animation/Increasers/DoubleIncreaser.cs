using System;
using System.Windows;

namespace Xaml.Effects.Toolkit.Animation.Increasers
{
    public class DoubleIncreaser : Increaser<double>
    {
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(Double), typeof(DoubleIncreaser), new PropertyMetadata(default(double)));

        private Double _current;

        public override Double Next
        {
            get
            {
                var result = Start + _current;
                _current += Step;
                return result;
            }
        }

        public override Double Start
        {
            get { return (Double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
    }
}
