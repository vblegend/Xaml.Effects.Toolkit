using System.Windows;
using System.Windows.Controls;

namespace Xaml.Effects.Toolkit.Controls
{
    // Implements ItemsControl for ToolboxItems    
    public class Toolbox : ItemsControl
    {


        #region ItemSize
        // Defines the ItemHeight and ItemWidth properties of
        // the WrapPanel used for this Toolbox
        public Size ItemSize
        {
            get
            {
                return (Size)GetValue(ItemSizeProperty);
            }
            set
            {
                SetValue(ItemSizeProperty, value);
            }
        }
        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(Size), typeof(Toolbox), new PropertyMetadata(new Size(50, 50), OnItemSizeChanged));

        private static void OnItemSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var sender = o as Toolbox;
            //Canvas.SetTop(sender, (Size)e.NewValue);
            // sender.InvalidateMeasure();
        }
        #endregion




        // Creates or identifies the element that is used to display the given item.        
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxItem();
        }

        // Determines if the specified item is (or is eligible to be) its own container.        
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}
