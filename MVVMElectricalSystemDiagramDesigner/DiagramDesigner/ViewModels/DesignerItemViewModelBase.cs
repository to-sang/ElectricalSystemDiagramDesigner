using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiagramDesigner
{
    public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase
    {

        private double left;
        private double top;
        private bool showConnectors = false;
        private readonly List<FullyCreatedConnectorInfo> connectors = new();

        private double itemWidth = 60;
        private double itemHeight = 60;

        public DesignerItemViewModelBase(Guid id, IDiagramViewModel parent, double left, double top, int angle) : base(id, parent)
        {
            this.left = left;
            this.top = top;
            this.angle = angle;
            Init();
        }

        public DesignerItemViewModelBase(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle) : base(id, parent)
        {
            this.left = left;
            this.top = top;
            this.itemWidth = itemWidth;
            this.itemHeight = itemHeight;
            this.angle = angle;
            Init();
        }

        public DesignerItemViewModelBase() : base()
        {
            Init();
        }

        public double ItemWidth
        {
            get => itemWidth;
            set
            {
                if (itemWidth != value)
                {
                    double temp = itemWidth;
                    Default.redoActions.Clear();
                    RedoChangeWidth(value);
                }
            }
        }

        private void UndoChangeWidth(double previousWidth)
        {
            double temp = itemWidth;
            Default.redoActions.Push(() => RedoChangeWidth(temp));
            itemWidth = previousWidth;
            connectors[0].ConnectorWidth = previousWidth;
            connectors[1].ConnectorWidth = previousWidth;
            NotifyChanged("ItemWidth");
        }

        private void RedoChangeWidth(double futureWidth)
        {
            double temp = itemWidth;
            Default.undoActions.Push(() => UndoChangeWidth(temp));
            itemWidth = futureWidth;
            connectors[0].ConnectorWidth = futureWidth;
            connectors[1].ConnectorWidth = futureWidth;
            NotifyChanged("ItemWidth");
        }

        public double ItemHeight
        {
            get => itemHeight;
            set
            {
                if (itemHeight != value)
                {
                    double temp = itemHeight;
                    Default.redoActions.Clear();
                    RedoChangeHeight(value);
                }
            }
        }

        private void UndoChangeHeight(double previousHeight)
        {
            double temp = itemHeight;
            Default.redoActions.Push(() => RedoChangeHeight(temp));
            itemHeight = previousHeight;
            connectors[2].ConnectorHeight = previousHeight;
            connectors[3].ConnectorHeight = previousHeight;
            NotifyChanged("ItemHeight");
        }

        private void RedoChangeHeight(double futureHeight)
        {
            double temp = itemHeight;
            Default.undoActions.Push(() => UndoChangeHeight(temp));
            itemHeight = futureHeight;
            connectors[2].ConnectorHeight = futureHeight;
            connectors[3].ConnectorHeight = futureHeight;
            NotifyChanged("ItemHeight");
        }

        public FullyCreatedConnectorInfo TopConnector => connectors[0];


        public FullyCreatedConnectorInfo BottomConnector => connectors[1];


        public FullyCreatedConnectorInfo LeftConnector => connectors[2];


        public FullyCreatedConnectorInfo RightConnector => connectors[3];

        public bool ShowConnectors
        {
            get => showConnectors;
            set
            {
                if (showConnectors != value)
                {
                    showConnectors = value;
                    TopConnector.ShowConnectors = value;
                    BottomConnector.ShowConnectors = value;
                    RightConnector.ShowConnectors = value;
                    LeftConnector.ShowConnectors = value;
                    NotifyChanged("ShowConnectors");
                }
            }
        }


        public double Left
        {
            get => left;
            set
            {
                if (left != value)
                {
                    left = value;
                    NotifyChanged("Left");
                }
            }
        }

        public double Top
        {
            get => top;
            set
            {
                if (top != value)
                {
                    top = value;
                    NotifyChanged("Top");
                }
            }
        }


        private void Init()
        {
            connectors.Add(new(this, ConnectorOrientation.Top));
            connectors.Add(new(this, ConnectorOrientation.Bottom));
            connectors.Add(new(this, ConnectorOrientation.Left));
            connectors.Add(new(this, ConnectorOrientation.Right));
            connectors[0].ConnectorWidth = connectors[1].ConnectorWidth = ItemWidth;
            connectors[2].ConnectorHeight = connectors[3].ConnectorHeight = ItemHeight;
            Angle = 0;
        }

        private int angle;

        public int Angle
        {
            get => angle;
            set
            {
                if (angle != value)
                {
                    angle = value;
                    AngleChanged(preRotate, Angle);
                    Default.redoActions.Clear();
                    NotifyChanged("Angle");
                }
            }
        }
        private ImageSource imageSource;
        public ImageSource ImageSource { get => imageSource; set { imageSource = value; NotifyChanged("ImageSource"); } }

        private int preRotate = 0;
        private void AngleChanged(int preAngle, int thisAngle)
        {
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = (TransformedBitmap)imageSource;

            RotateTransform transform = new(thisAngle - preAngle);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            ImageSource.Freeze();
            if ((preAngle - thisAngle) % 180 != 0)
                (ItemWidth, ItemHeight) = (ItemHeight, ItemWidth);
            Default.undoActions.Push(() => UndoRotate(thisAngle, preAngle));
            preRotate = Angle;
        }    

        private void UndoRotate(int preAngle, int thisAngle)
        {
            TransformedBitmap TempImage = new();

            TempImage.BeginInit();
            TempImage.Source = (TransformedBitmap)imageSource;

            RotateTransform transform = new(thisAngle - preAngle);
            TempImage.Transform = transform;
            TempImage.EndInit();

            ImageSource = TempImage;
            ImageSource.Freeze();
            if ((preAngle - thisAngle) % 180 != 0)
                (ItemWidth, ItemHeight) = (ItemHeight, ItemWidth);
            Default.redoActions.Push(() => AngleChanged(preAngle, thisAngle));
            preRotate = Angle;
        }    

        public string ToJSON()
        {
            string content = "";
            content += "\t\t\t\t{\n";
            content += "\t\t\t\t\t\"id\": " + GetJSONId() + ",\n";
            content += "\t\t\t\t\t\"type\": \"" + GetJSONType(GetType().Name) + "\",\n";
            content += "\t\t\t\t\t\"left\": " + Left + ",\n";
            content += "\t\t\t\t\t\"top\": " + Top + ",\n";
            content += "\t\t\t\t\t\"width\": " + ItemWidth + ",\n";
            content += "\t\t\t\t\t\"height\": " + ItemHeight + ",\n";
            content += "\t\t\t\t\t\"angle\": " + Angle + "\n";
            content += "\t\t\t\t}";
            return content;
        }

        public string GetJSONId() => (Default.IdInt(Id) == -1 ? "\"" + Default.Guid2Int(Id) + "\"" : Default.IdInt(Id).ToString());

        public string GetJSONType(string type) => type.Substring(0, Math.Max(type.Length - 21, 0));
    }
}
