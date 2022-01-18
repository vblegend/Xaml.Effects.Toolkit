using System;

namespace Xaml.Effects.Toolkit.Animation.Creators
{
    public class RandomDoubleCreator : RandomCreator<double>
    {
        private Random _random = new Random();

        public override double Next => _random.NextDouble() * _random.Next(Convert.ToInt32(Math.Round(Max)));
    }
}
