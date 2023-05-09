using System.Windows;

namespace DiagramDesigner
{
    public class PartCreatedConnectionInfo : ConnectorInfoBase
    {
        public Point CurrentLocation { get; private set; }

        public PartCreatedConnectionInfo(Point currentLocation) : base(ConnectorOrientation.None)
        {
            this.CurrentLocation = currentLocation;
        }
    }
}
