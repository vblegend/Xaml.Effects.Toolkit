using System;
using System.Windows.Data;
using Xaml.Effects.Toolkit.Uitity;

namespace Xaml.Effects.Toolkit.Converter
{
    public class EnumDescriptionConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public EnumDescriptionConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (type.BaseType == typeof(Enum))
                {
                    Enum ivalue = (Enum)value;
                    return ivalue.GetDescription();
                }
            }
            return "";
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
