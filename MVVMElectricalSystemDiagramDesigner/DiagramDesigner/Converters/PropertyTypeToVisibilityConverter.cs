using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiagramDesigner
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class PropertyTypeToVisibilityConverter : IValueConverter
    {
        static PropertyTypeToVisibilityConverter()
        {
            Instance = new();
        }

        public static PropertyTypeToVisibilityConverter Instance
        {
            get;
            private set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == (string)parameter)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new();
    }
}
