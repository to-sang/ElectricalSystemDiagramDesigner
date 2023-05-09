using System.Windows;

namespace DiagramDesigner
{
    public class PointHelper
    {
        public static Point GetPointForConnector(FullyCreatedConnectorInfo connector) => GetPointForConnector(connector.Orientation, connector.DataItem.Left, connector.DataItem.Top, connector.DataItem.ItemWidth, connector.DataItem.ItemHeight);
        
        public static Point GetPointForConnector(ConnectorOrientation orientation, double left, double top, double width, double height) => orientation switch
        {
            ConnectorOrientation.None => new(),
            ConnectorOrientation.Left => new(left, top + height / 2),
            ConnectorOrientation.Top => new(left + width / 2, top),
            ConnectorOrientation.Right => new(left + width, top + height / 2),
            ConnectorOrientation.Bottom => new(left + width / 2, top + height),
            _ => new()
        };
    }
}
