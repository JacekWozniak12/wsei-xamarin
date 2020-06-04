using System;
using System.Globalization;
using Xamarin.Forms;

namespace AirMonitor.Views.ViewModels
{
    public class IntTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value;
        }
    }
}
