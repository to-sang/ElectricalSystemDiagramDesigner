using DiagramDesigner;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class GroundDesignerItemViewModel : DesignerItemViewModelBase
    {
        public GroundDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, int angle)
            : base(id, parent, left, top, angle) => Init();

        public GroundDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle)
             : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();


        private readonly string imageUri = "pack://application:,,,/DemoApp;component/Images/Devices/Ground.png";

        public GroundDesignerItemViewModel() : base() => Init();


        private void Init()
        {
            this.ShowConnectors = false;
            if (ItemWidth == 60 && ItemHeight == 60)
            {
                ItemWidth = 60;
                ItemHeight = 30;
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
