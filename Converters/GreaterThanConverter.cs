using System;
using System.Globalization;
using System.Windows.Data;

namespace RichIZ.Converters
{
    public class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue && parameter is string paramString)
            {
                if (decimal.TryParse(paramString, out decimal threshold))
                {
                    return decimalValue > threshold;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
