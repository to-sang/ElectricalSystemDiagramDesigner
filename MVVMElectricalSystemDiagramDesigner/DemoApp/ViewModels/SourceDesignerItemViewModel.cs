using System;
using DiagramDesigner;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class SourceDesignerItemViewModel : DesignerItemViewModelBase
    {

        public SourceDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, int angle)
            : base(id, parent, left, top, angle) => Init();

        public SourceDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle)
             : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();


        public SourceDesignerItemViewModel() => Init();


        private readonly string imageUri = "pack://application:,,,/DemoApp;component/Images/Devices/Source.png";

        private void Init()
        {
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = new BitmapImage(new(imageUri, System.UriKind.RelativeOrAbsolute));

            RotateTransform transform = new(0);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            this.ShowConnectors = false;
            ItemHeight = 50;
            ItemWidth = 200;
        }
    }
}
