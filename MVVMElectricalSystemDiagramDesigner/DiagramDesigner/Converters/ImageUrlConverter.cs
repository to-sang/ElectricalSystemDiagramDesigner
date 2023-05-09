using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class ImageUrlConverter : IValueConverter
    {
        static ImageUrlConverter()
        {
            Instance = new();
        }

        public static ImageUrlConverter Instance
        {
            get;
            private set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri imagePath = new(value.ToString(), UriKind.RelativeOrAbsolute);
            ImageSource source = new BitmapImage(imagePath);
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new();
        }
    }
}
