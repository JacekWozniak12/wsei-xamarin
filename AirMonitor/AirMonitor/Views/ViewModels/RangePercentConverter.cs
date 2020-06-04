using System;
using System.Globalization;
using Xamarin.Forms;

namespace AirMonitor.Views.ViewModels
{
    public class RangePercentConverter : IValueConverter
    {
        float? t = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            t = value as float?;
            return string.Format("{0:0%}", t);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return t;
        }

        double GetParameter(object parameter)
        {
            if (parameter is double)
                return (double)parameter;

            else if (parameter is int)
                return (int)parameter;

            else if (parameter is string)
                return double.Parse((string)parameter);

            return 1;
        }

    }
}
