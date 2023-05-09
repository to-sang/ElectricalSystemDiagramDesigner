using System;
using DiagramDesigner;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class TransformerDesignerItemViewModel : DesignerItemViewModelBase
    {
        public TransformerDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, int angle) 
            : base(id, parent, left, top, angle) => Init();

        public TransformerDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle) 
            : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();

        public TransformerDesignerItemViewModel() : base() => Init();
        private readonly string imageUri4 = "pack://application:,,,/DemoApp;component/Images/Devices/Transformer4.png";
        private readonly string imageUri3 = "pack://application:,,,/DemoApp;component/Images/Devices/Transformer3.png";
        private readonly string imageUri2 = "pack://application:,,,/DemoApp;component/Images/Devices/Transformer2.png";

        private void Init()
        {
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = new BitmapImage(new(imageUri3, System.UriKind.RelativeOrAbsolute));

            RotateTransform transform = new(0);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            this.ShowConnectors = false;
        }
    }
}
