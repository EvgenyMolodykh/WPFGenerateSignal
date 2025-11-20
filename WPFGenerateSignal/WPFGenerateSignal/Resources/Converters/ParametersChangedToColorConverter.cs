using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFGenerateSignal.Resources.Converters
{
    public class ParametersChangedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool parametersChanged && parametersChanged)
            {
          
                return new SolidColorBrush(Colors.LightGreen);
            }
      
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}