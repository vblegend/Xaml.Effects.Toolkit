using System.Windows;

namespace Xaml.Effects.Toolkit.Animation
{
    public abstract class Increaser<T> : DependencyObject
    {
        public virtual T Start { get; set; }

        public T Step { get; set; }

        public abstract T Next { get; }
    }
}
