using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using Xaml.Effects.Toolkit.Actions;

namespace Xaml.Effects.Toolkit.Behaviors
{
    public class EventBehavior : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action", typeof(ICommand), typeof(EventBehavior), new PropertyMetadata(null));

        /// <summary>
        /// 要执行的 Action 
        /// </summary>
        public ICommand Action
        {
            get { return (ICommand)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (Action != null)
            {
                Action.Execute(parameter);
            }
        }
    }
}
