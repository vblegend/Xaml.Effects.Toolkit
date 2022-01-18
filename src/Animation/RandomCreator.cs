using System.Windows;
namespace Xaml.Effects.Toolkit.Animation
{
    public abstract class RandomCreator<T> : DependencyObject
    {
        public T Max { get; set; }

        public abstract T Next { get; }
    }
}
