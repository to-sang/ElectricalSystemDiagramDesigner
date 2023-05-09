using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DiagramDesigner.Helpers;

namespace DiagramDesigner
{
    public class ConnectorViewModel : SelectableDesignerItemViewModelBase
    {
        private FullyCreatedConnectorInfo sourceConnectorInfo;
        private ConnectorInfoBase sinkConnectorInfo;
        private Point sourceB;
        private Point sourceA;
        private List<Point> connectionPoints;
        private Point endPoint;
        private Rect area;


        public ConnectorViewModel(Guid id, IDiagramViewModel parent,
            FullyCreatedConnectorInfo sourceConnectorInfo, FullyCreatedConnectorInfo sinkConnectorInfo) : base(id, parent)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }

        public ConnectorViewModel()
        {
        }

        public ConnectorViewModel(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Id = Guid.NewGuid();
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }


        public static IPathFinder PathFinder { get; set; }

        public bool IsFullConnection
        {
            get { return sinkConnectorInfo is FullyCreatedConnectorInfo; }
        }

        public Point SourceA
        {
            get
            {
                return sourceA;
            }
            set
            {
                if (sourceA != value)
                {
                    sourceA = value;
                    UpdateArea();
                    NotifyChanged("SourceA");
                }
            }
        }

        public Point SourceB
        {
            get
            {
                return sourceB;
            }
            set
            {
                if (sourceB != value)
                {
                    sourceB = value;
                    UpdateArea();
                    NotifyChanged("SourceB");
                }
            }
        }

        public List<Point> ConnectionPoints
        {
            get
            {
                return connectionPoints;
            }
            private set
            {
                if (connectionPoints != value)
                {
                    connectionPoints = value;
                    NotifyChanged("ConnectionPoints");
                }
            }
        }

        public Point EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                if (endPoint != value)
                {
                    endPoint = value;
                    NotifyChanged("EndPoint");
                }
            }
        }
        public Rect Area
        {
            get
            {
                return area;
            }
            set
            {
                if (area != value)
                {
                    area = value;
                    UpdateConnectionPoints();
                    NotifyChanged("Area");
                }
            }
        }

        public ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position, double itemWidth, double itemHeight)
        {

            return new()
            {
                Orientation = orientation,
                DesignerItemSize = new(itemWidth, itemHeight),
                DesignerItemLeft = left,
                DesignerItemTop = top,
                Position = position

            };
        }
        public FullyCreatedConnectorInfo SourceConnectorInfo
        {
            get
            {
                return sourceConnectorInfo;
            }
            set
            {
                if (sourceConnectorInfo != value)
                {

                    sourceConnectorInfo = value;
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    NotifyChanged("SourceConnectorInfo");
                    (sourceConnectorInfo.DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                }
            }
        }

        public ConnectorInfoBase SinkConnectorInfo
        {
            get
            {
                return sinkConnectorInfo;
            }
            set
            {
                if (sinkConnectorInfo != value)
                {

                    sinkConnectorInfo = value;
                    if (SinkConnectorInfo is FullyCreatedConnectorInfo info)
                    {
                        SourceB = PointHelper.GetPointForConnector(info);
                        (((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                    }
                    else
                    {

                        SourceB = ((PartCreatedConnectionInfo)SinkConnectorInfo).CurrentLocation;
                    }
                    NotifyChanged("SinkConnectorInfo");
                }
            }
        }

        private void UpdateArea()
        {
            Area = new(SourceA, SourceB);
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new()
            {
                new(SourceA.X  <  SourceB.X ? 0d : Area.Width, SourceA.Y  <  SourceB.Y ? 0d : Area.Height),
                new(SourceA.X  >  SourceB.X ? 0d : Area.Width, SourceA.Y  >  SourceB.Y ? 0d : Area.Height)
            };

            ConnectorInfo sourceInfo = ConnectorInfo(SourceConnectorInfo.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0],
                                            sourceConnectorInfo.DataItem.ItemWidth,
                                            sourceConnectorInfo.DataItem.ItemHeight);

            if (IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                ConnectorInfo sinkInfo = ConnectorInfo(SinkConnectorInfo.Orientation,
                                  ConnectionPoints[1].X,
                                  ConnectionPoints[1].Y,
                                  ConnectionPoints[1],
                                  ((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem.ItemWidth,
                                  ((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem.ItemHeight);

                #region Busbar

                if (sourceInfo.DesignerItemSize.Width / sourceInfo.DesignerItemSize.Height > 10 || sourceInfo.DesignerItemSize.Height / sourceInfo.DesignerItemSize.Width > 10)
                    (sinkInfo, sourceInfo) = (sourceInfo, sinkInfo);

                if (sinkInfo.DesignerItemSize.Width / sinkInfo.DesignerItemSize.Height > 10 || sinkInfo.DesignerItemSize.Height / sinkInfo.DesignerItemSize.Width > 10)
                {
                    if (sinkInfo.DesignerItemSize.Width / sinkInfo.DesignerItemSize.Height > 10)
                    {
                        if (sourceInfo.DesignerItemTop < sinkInfo.DesignerItemTop)
                            SinkConnectorInfo.Orientation = ConnectorOrientation.Top;
                        else if (sourceInfo.DesignerItemTop > sinkInfo.DesignerItemTop)
                            SinkConnectorInfo.Orientation = ConnectorOrientation.Bottom;
                        else if (sourceInfo.DesignerItemLeft < sinkInfo.DesignerItemLeft)
                            SinkConnectorInfo.Orientation = ConnectorOrientation.Left;
                        else
                            SinkConnectorInfo.Orientation = ConnectorOrientation.Right;
                    }
                    else
                        if (sourceInfo.DesignerItemLeft < sinkInfo.DesignerItemLeft)
                        SinkConnectorInfo.Orientation = ConnectorOrientation.Left;
                    else if (sourceInfo.DesignerItemLeft > sinkInfo.DesignerItemLeft)
                        SinkConnectorInfo.Orientation = ConnectorOrientation.Right;
                    else if (sourceInfo.DesignerItemTop < sinkInfo.DesignerItemTop)
                        SinkConnectorInfo.Orientation = ConnectorOrientation.Top;
                    else
                        SinkConnectorInfo.Orientation = ConnectorOrientation.Bottom;

                    sinkInfo.Orientation = SinkConnectorInfo.Orientation;
                }

                #endregion

                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, sinkInfo);
            }
            else
            {
                ConnectionPoints = PathFinder.GetConnectionLine(sourceInfo, ConnectionPoints[1], ConnectorOrientation.Left);
                EndPoint = new();
            }
        }

        private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ItemHeight":
                case "ItemWidth":
                case "Left":
                case "Top":
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    if (this.SinkConnectorInfo is FullyCreatedConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)this.SinkConnectorInfo);
                    }
                    break;

            }
        }

        private void Init(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Parent = sourceConnectorInfo.DataItem.Parent;
            this.SourceConnectorInfo = sourceConnectorInfo;
            this.SinkConnectorInfo = sinkConnectorInfo;
            PathFinder = new OrthogonalPathFinder();
        }

        public string ToJSON()
        {
            string content = string.Empty;
            content += "\t\t\t\t{\n";
            content += "\t\t\t\t\t\"id\": \"" + Default.Guid2Int(Id) + "\",\n";
            content += "\t\t\t\t\t\"connectors\": [\n";
            content += "\t\t\t\t\t\t{\n";
            content += "\t\t\t\t\t\t\t\"diagramItemId\": " + sourceConnectorInfo.DataItem.GetJSONId() + ",\n";
            content += "\t\t\t\t\t\t\t\"position\": \"" + sourceConnectorInfo.Orientation.ToString() + "\"\n";
            content += "\t\t\t\t\t\t},\n";
            content += "\t\t\t\t\t\t{\n";
            content += "\t\t\t\t\t\t\t\"diagramItemId\": " + ((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem.GetJSONId() + ",\n";
            content += "\t\t\t\t\t\t\t\"position\": \"" + ((FullyCreatedConnectorInfo)sinkConnectorInfo).Orientation.ToString() + "\"\n";
            content += "\t\t\t\t\t\t}\n";
            content += "\t\t\t\t\t],\n";
            content += "\t\t\t\t\t\"points\": [\n";
            foreach(var point in ConnectionPoints)
            {
                content += "\t\t\t\t\t\t{\n";
                content += "\t\t\t\t\t\t\t\"x\": " + Math.Min(sourceA.X + point.X, sourceB.X + point.X) + ",\n";
                content += "\t\t\t\t\t\t\t\"y\": " + Math.Min(sourceA.Y + point.Y, sourceB.Y + point.Y) + "\n";
                content += "\t\t\t\t\t\t},\n";
            }
            content = content.Substring(0, content.Length - 2);
            content += "\n\t\t\t\t\t]\n";
            content += "\n\t\t\t\t}";
            return content;
        }

    }
}
