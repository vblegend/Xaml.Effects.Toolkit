using System.Windows;

namespace Xaml.Effects.Toolkit.Animation
{
    public abstract class ValueWrapper<T> : DependencyObject
    {
        public abstract T Value { get; set; }
    }
}
