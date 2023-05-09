using System;
using DiagramDesigner;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class BusbarDesignerItemViewModel : DesignerItemViewModelBase
    {
        public BusbarDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle)
             : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();

        public BusbarDesignerItemViewModel() : base() => Init();

        private readonly string imageUri = "pack://application:,,,/DemoApp;component/Images/Devices/Busbar.png";

        private void Init()
        {
            this.ShowConnectors = false;
            if (ItemWidth == 60 && ItemHeight == 60)
            {
                ItemWidth = 1000;
                ItemHeight = 10;
            }
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = new BitmapImage(new(imageUri, System.UriKind.RelativeOrAbsolute));

            RotateTransform transform = new(0);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            ImageSource.Freeze();
        }
    }
}
