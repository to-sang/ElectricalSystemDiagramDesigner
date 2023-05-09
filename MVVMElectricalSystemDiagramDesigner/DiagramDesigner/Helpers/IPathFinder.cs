using System.Collections.Generic;
using System.Windows;

namespace DiagramDesigner
{
    public interface IPathFinder
    {
        List<Point> GetConnectionLine(ConnectorInfo source, ConnectorInfo sink, bool showLastLine = false);
        List<Point> GetConnectionLine(ConnectorInfo source, Point sinkPoint, ConnectorOrientation preferredOrientation);
    }
}
