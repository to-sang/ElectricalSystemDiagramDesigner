using System;
using DiagramDesigner;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class FanDesignerItemViewModel : DesignerItemViewModelBase
    {
        public FanDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, int angle) 
            : base(id, parent, left, top, angle) => Init();

        public FanDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle) 
            : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();


        public FanDesignerItemViewModel() : base() => Init();

        private readonly string imageUri = "pack://application:,,,/DemoApp;component/Images/Devices/Fan.png";

        private void Init()
        {
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = new BitmapImage(new(imageUri, UriKind.RelativeOrAbsolute));

            RotateTransform transform = new(Angle);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            this.ShowConnectors = false;

        }
    }
}
