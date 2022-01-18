using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Xaml.Effects.Toolkit.Converter
{
    /// <summary>
    /// Boolean to visible converter
    /// </summary>
    public class ImageLoadConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public ImageLoadConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String imageUrl)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.CreateOptions = BitmapCreateOptions.DelayCreation;
                bi.UriSource = new Uri(imageUrl);
                bi.EndInit();
                return bi;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

