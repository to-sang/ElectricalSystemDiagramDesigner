using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    // Note: I couldn't find a useful open sourceInfo library that does
    // orthogonal routing so started to write something on my own.
    // Categorize this as a quick and dirty short term solution.
    // I will keep on searching.

    // Helper class to provide an orthogonal connection path
    public class OrthogonalPathFinder : IPathFinder
    {
        private const int margin = 20;

        public List<Point> GetConnectionLine(ConnectorInfo sourceInfo, ConnectorInfo sinkInfo, bool showLastLine = false)
        {
            List<Point> linePoints = new();

            Rect rectSource = GetRectWithMargin(sourceInfo, margin);
            Rect rectSink = GetRectWithMargin(sinkInfo, margin);

            Point startPoint = GetOffsetPoint(sourceInfo, rectSource);
            Point endPoint = GetOffsetPoint(sinkInfo, rectSink);

            if (sinkInfo.DesignerItemSize.Width / sinkInfo.DesignerItemSize.Height > 10 || sinkInfo.DesignerItemSize.Height / sinkInfo.DesignerItemSize.Width > 10)
            {
                if (sinkInfo.DesignerItemSize.Width / sinkInfo.DesignerItemSize.Height > 10)
                {
                    if (sourceInfo.DesignerItemTop < sinkInfo.DesignerItemTop)
                    {
                        if (sourceInfo.Orientation == ConnectorOrientation.Bottom)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y - margin));
                            linePoints.Add(new(startPoint.X, endPoint.Y + margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Left)
                        {
                            linePoints.Add(new(startPoint.X + margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, endPoint.Y + margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Right)
                        {
                            linePoints.Add(new(startPoint.X - margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, endPoint.Y + margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Top)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y + margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X + margin * 2 + sourceInfo.DesignerItemSize.Width / 2, startPoint.Y));
                            linePoints.Add(new(startPoint.X + margin * 2 + sourceInfo.DesignerItemSize.Width / 2, endPoint.Y + margin));
                        }
                        return linePoints;
                    }
                    else if (sourceInfo.DesignerItemTop > sinkInfo.DesignerItemTop)
                    {
                        if (sourceInfo.Orientation == ConnectorOrientation.Bottom)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y - margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X - margin * 2 - sourceInfo.DesignerItemSize.Width / 2, startPoint.Y));
                            linePoints.Add(new(startPoint.X - margin * 2 - sourceInfo.DesignerItemSize.Width / 2, endPoint.Y - margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Left)
                        {
                            linePoints.Add(new(startPoint.X + margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, endPoint.Y - margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Right)
                        {
                            linePoints.Add(new(startPoint.X - margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, endPoint.Y - margin));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Top)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y + margin));
                            linePoints.Add(new(startPoint.X, endPoint.Y - margin));
                        }
                        return linePoints;
                    }
                }
                else
                {
                    if (sourceInfo.DesignerItemLeft < sinkInfo.DesignerItemLeft)
                    {
                        if (sourceInfo.Orientation == ConnectorOrientation.Bottom)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y - margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(endPoint.X + margin, startPoint.Y));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Left)
                        {
                            linePoints.Add(new(startPoint.X + margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, startPoint.Y + margin * 2 + sourceInfo.DesignerItemSize.Height / 2));
                            linePoints.Add(new(endPoint.X + margin, startPoint.Y + margin * 2 + sourceInfo.DesignerItemSize.Height / 2));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Right)
                        {
                            linePoints.Add(new(startPoint.X - margin, startPoint.Y));
                            linePoints.Add(new(endPoint.X + margin, startPoint.Y));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Top)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y + margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(endPoint.X + margin, startPoint.Y));
                        }
                        return linePoints;
                    }
                    else if (sourceInfo.DesignerItemLeft > sinkInfo.DesignerItemLeft)
                    {
                        if (sourceInfo.Orientation == ConnectorOrientation.Bottom)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y - margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(endPoint.X - margin, startPoint.Y));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Left)
                        {
                            linePoints.Add(new(startPoint.X + margin, startPoint.Y));
                            linePoints.Add(new(endPoint.X - margin, startPoint.Y));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Right)
                        {
                            linePoints.Add(new(startPoint.X - margin, startPoint.Y));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(startPoint.X, startPoint.Y - margin * 2 - sourceInfo.DesignerItemSize.Height / 2));
                            linePoints.Add(new(endPoint.X - margin, startPoint.Y - margin * 2 - sourceInfo.DesignerItemSize.Height / 2));
                        }
                        if (sourceInfo.Orientation == ConnectorOrientation.Top)
                        {
                            linePoints.Add(new(startPoint.X, startPoint.Y + margin));
                            linePoints.Add(startPoint);
                            linePoints.Add(new(endPoint.X - margin, startPoint.Y));
                        }
                        return linePoints;
                    }
                }
            }

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (!rectSink.Contains(currentPoint) && !rectSource.Contains(endPoint))
            {
                while (true)
                {
                    #region sourceInfo node

                    if (IsPointVisible(currentPoint, endPoint, new[] { rectSource, rectSink }))
                    {
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }

                    Point neighbour = GetNearestVisibleNeighborSink(currentPoint, endPoint, sinkInfo, rectSource, rectSink);
                    if (!double.IsNaN(neighbour.X))
                    {
                        linePoints.Add(neighbour);
                        linePoints.Add(endPoint);
                        currentPoint = endPoint;
                        break;
                    }

                    if (currentPoint == startPoint)
                    {
                        bool flag;
                        Point n = GetNearestNeighborSource(sourceInfo, endPoint, rectSource, rectSink, out flag);
                        linePoints.Add(n);
                        currentPoint = n;

                        if (!IsRectVisible(currentPoint, rectSink, new[] { rectSource }))
                        {
                            Point n1, n2;
                            GetOppositeCorners(sourceInfo.Orientation, rectSource, out n1, out n2);
                            if (flag)
                            {
                                linePoints.Add(n1);
                                currentPoint = n1;
                            }
                            else
                            {
                                linePoints.Add(n2);
                                currentPoint = n2;
                            }
                            if (!IsRectVisible(currentPoint, rectSink, new[] { rectSource }))
                            {
                                if (flag)
                                {
                                    linePoints.Add(n2);
                                    currentPoint = n2;
                                }
                                else
                                {
                                    linePoints.Add(n1);
                                    currentPoint = n1;
                                }
                            }
                        }
                    }
                    #endregion

                    #region sinkInfo node

                    else // from here on we jump to the sinkInfo node
                    {
                        Point n1, n2; // neighbour corner
                        Point s1, s2; // opposite corner
                        GetNeighborCorners(sinkInfo.Orientation, rectSink, out s1, out s2);
                        GetOppositeCorners(sinkInfo.Orientation, rectSink, out n1, out n2);

                        bool n1Visible = IsPointVisible(currentPoint, n1, new[] { rectSource, rectSink });
                        bool n2Visible = IsPointVisible(currentPoint, n2, new[] { rectSource, rectSink });

                        if (n1Visible && n2Visible)
                        {
                            if (rectSource.Contains(n1))
                            {
                                linePoints.Add(n2);
                                if (rectSource.Contains(s2))
                                {
                                    linePoints.Add(n1);
                                    linePoints.Add(s1);
                                }
                                else
                                    linePoints.Add(s2);

                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }

                            if (rectSource.Contains(n2))
                            {
                                linePoints.Add(n1);
                                if (rectSource.Contains(s1))
                                {
                                    linePoints.Add(n2);
                                    linePoints.Add(s2);
                                }
                                else
                                    linePoints.Add(s1);

                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }

                            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
                            {
                                linePoints.Add(n1);
                                if (rectSource.Contains(s1))
                                {
                                    linePoints.Add(n2);
                                    linePoints.Add(s2);
                                }
                                else
                                    linePoints.Add(s1);
                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }
                            else
                            {
                                linePoints.Add(n2);
                                if (rectSource.Contains(s2))
                                {
                                    linePoints.Add(n1);
                                    linePoints.Add(s1);
                                }
                                else
                                    linePoints.Add(s2);
                                linePoints.Add(endPoint);
                                currentPoint = endPoint;
                                break;
                            }
                        }
                        else if (n1Visible)
                        {
                            linePoints.Add(n1);
                            if (rectSource.Contains(s1))
                            {
                                linePoints.Add(n2);
                                linePoints.Add(s2);
                            }
                            else
                                linePoints.Add(s1);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                        else
                        {
                            linePoints.Add(n2);
                            if (rectSource.Contains(s2))
                            {
                                linePoints.Add(n1);
                                linePoints.Add(s1);
                            }
                            else
                                linePoints.Add(s2);
                            linePoints.Add(endPoint);
                            currentPoint = endPoint;
                            break;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                linePoints.Add(endPoint);
            }

            linePoints = OptimizeLinePoints(linePoints, new[] { rectSource, rectSink }, sourceInfo.Orientation, sinkInfo.Orientation);

            CheckPathEnd(sourceInfo, sinkInfo, showLastLine, linePoints);



            return linePoints;
        }

        public List<Point> GetConnectionLine(ConnectorInfo sourceInfo, Point sinkPoint, ConnectorOrientation preferredOrientation)
        {
            List<Point> linePoints = new();
            Rect rectSource = GetRectWithMargin(sourceInfo, 10);
            Point startPoint = GetOffsetPoint(sourceInfo, rectSource);
            Point endPoint = sinkPoint;

            linePoints.Add(startPoint);
            Point currentPoint = startPoint;

            if (!rectSource.Contains(endPoint))
            {
                while (true)
                {
                    if (IsPointVisible(currentPoint, endPoint, new[] { rectSource }))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }

                    bool sideFlag;
                    Point n = GetNearestNeighborSource(sourceInfo, endPoint, rectSource, out sideFlag);
                    linePoints.Add(n);
                    currentPoint = n;

                    if (IsPointVisible(currentPoint, endPoint, new[] { rectSource }))
                    {
                        linePoints.Add(endPoint);
                        break;
                    }
                    else
                    {
                        Point n1, n2;
                        GetOppositeCorners(sourceInfo.Orientation, rectSource, out n1, out n2);
                        if (sideFlag)
                            linePoints.Add(n1);
                        else
                            linePoints.Add(n2);

                        linePoints.Add(endPoint);
                        break;
                    }
                }
            }
            else
            {
                linePoints.Add(endPoint);
            }

            if (preferredOrientation != ConnectorOrientation.None)
                linePoints = OptimizeLinePoints(linePoints, new[] { rectSource }, sourceInfo.Orientation, preferredOrientation);
            else
                linePoints = OptimizeLinePoints(linePoints, new[] { rectSource }, sourceInfo.Orientation, GetOpositeOrientation(sourceInfo.Orientation));

            return linePoints;
        }

        private static List<Point> OptimizeLinePoints(List<Point> linePoints, Rect[] rectangles, ConnectorOrientation sourceOrientation, ConnectorOrientation sinkOrientation)
        {
            List<Point> points = new();
            int cut = 0;

            for (int i = 0; i < linePoints.Count; i++)
            {
                if (i >= cut)
                {
                    for (int k = linePoints.Count - 1; k > i; k--)
                    {
                        if (IsPointVisible(linePoints[i], linePoints[k], rectangles))
                        {
                            cut = k;
                            break;
                        }
                    }
                    points.Add(linePoints[i]);
                }
            }

            #region Line
            for (int j = 0; j < points.Count - 1; j++)
            {
                if (points[j].X != points[j + 1].X && points[j].Y != points[j + 1].Y)
                {
                    ConnectorOrientation orientationFrom;
                    ConnectorOrientation orientationTo;

                    // orientation from point
                    if (j == 0)
                        orientationFrom = sourceOrientation;
                    else
                        orientationFrom = GetOrientation(points[j], points[j - 1]);

                    // orientation to pint 
                    if (j == points.Count - 2)
                        orientationTo = sinkOrientation;
                    else
                        orientationTo = GetOrientation(points[j + 1], points[j + 2]);


                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        double centerX = Math.Min(points[j].X, points[j + 1].X) + Math.Abs(points[j].X - points[j + 1].X) / 2;
                        points.Insert(j + 1, new(centerX, points[j].Y));
                        points.Insert(j + 2, new(centerX, points[j + 2].Y));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        double centerY = Math.Min(points[j].Y, points[j + 1].Y) + Math.Abs(points[j].Y - points[j + 1].Y) / 2;
                        points.Insert(j + 1, new(points[j].X, centerY));
                        points.Insert(j + 2, new(points[j + 2].X, centerY));
                        if (points.Count - 1 > j + 3)
                            points.RemoveAt(j + 3);
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Left || orientationFrom == ConnectorOrientation.Right) &&
                        (orientationTo == ConnectorOrientation.Top || orientationTo == ConnectorOrientation.Bottom))
                    {
                        points.Insert(j + 1, new(points[j + 1].X, points[j].Y));
                        return points;
                    }

                    if ((orientationFrom == ConnectorOrientation.Top || orientationFrom == ConnectorOrientation.Bottom) &&
                        (orientationTo == ConnectorOrientation.Left || orientationTo == ConnectorOrientation.Right))
                    {
                        points.Insert(j + 1, new(points[j].X, points[j + 1].Y));
                        return points;
                    }
                }
            }
            #endregion

            return points;
        }

        private static ConnectorOrientation GetOrientation(Point p1, Point p2)
        {
            if (p1.X == p2.X)
            {
                if (p1.Y >= p2.Y)
                    return ConnectorOrientation.Bottom;
                else
                    return ConnectorOrientation.Top;
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X >= p2.X)
                    return ConnectorOrientation.Right;
                else
                    return ConnectorOrientation.Left;
            }
            throw new("Failed to retrieve orientation");
        }

        private static Orientation GetOrientation(ConnectorOrientation sourceOrientation)
        {
            switch (sourceOrientation)
            {
                case ConnectorOrientation.Left:
                    return Orientation.Horizontal;
                case ConnectorOrientation.Top:
                    return Orientation.Vertical;
                case ConnectorOrientation.Right:
                    return Orientation.Horizontal;
                case ConnectorOrientation.Bottom:
                    return Orientation.Vertical;
                default:
                    throw new("Unknown ConnectorOrientation");
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo sourceInfo, Point endPoint, Rect rectSource, Rect rectSink, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(sourceInfo.Orientation, rectSource, out n1, out n2);

            if (rectSink.Contains(n1))
            {
                flag = false;
                return n2;
            }

            if (rectSink.Contains(n2))
            {
                flag = true;
                return n1;
            }

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestNeighborSource(ConnectorInfo sourceInfo, Point endPoint, Rect rectSource, out bool flag)
        {
            Point n1, n2; // neighbors
            GetNeighborCorners(sourceInfo.Orientation, rectSource, out n1, out n2);

            if ((Distance(n1, endPoint) <= Distance(n2, endPoint)))
            {
                flag = true;
                return n1;
            }
            else
            {
                flag = false;
                return n2;
            }
        }

        private static Point GetNearestVisibleNeighborSink(Point currentPoint, Point endPoint, ConnectorInfo sinkInfo, Rect rectSource, Rect rectSink)
        {
            Point s1, s2; // neighbors on sinkInfo side
            GetNeighborCorners(sinkInfo.Orientation, rectSink, out s1, out s2);

            bool flag1 = IsPointVisible(currentPoint, s1, new[] { rectSource, rectSink });
            bool flag2 = IsPointVisible(currentPoint, s2, new[] { rectSource, rectSink });

            if (flag1) // s1 visible
            {
                if (flag2) // s1 and s2 visible
                {
                    if (rectSink.Contains(s1))
                        return s2;

                    if (rectSink.Contains(s2))
                        return s1;

                    if ((Distance(s1, endPoint) <= Distance(s2, endPoint)))
                        return s1;
                    else
                        return s2;

                }
                else
                {
                    return s1;
                }
            }
            else // s1 not visible
            {
                if (flag2) // only s2 visible
                {
                    return s2;
                }
                else // s1 and s2 not visible
                {
                    return new(double.NaN, double.NaN);
                }
            }
        }

        private static bool IsPointVisible(Point fromPoint, Point targetPoint, Rect[] rectangles)
        {
            foreach (Rect rect in rectangles)
            {
                if (RectangleIntersectsLine(rect, fromPoint, targetPoint))
                    return false;
            }
            return true;
        }

        private static bool IsRectVisible(Point fromPoint, Rect targetRect, Rect[] rectangles)
        {
            if (IsPointVisible(fromPoint, targetRect.TopLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.TopRight, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomLeft, rectangles))
                return true;

            if (IsPointVisible(fromPoint, targetRect.BottomRight, rectangles))
                return true;

            return false;
        }

        private static bool RectangleIntersectsLine(Rect rect, Point startPoint, Point endPoint)
        {
            rect.Inflate(-1, -1);
            return rect.IntersectsWith(new(startPoint, endPoint));
        }

        private static void GetOppositeCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Top:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Right:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case ConnectorOrientation.Bottom:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                default:
                    throw new("No opposite corners found!");
            }
        }

        private static void GetNeighborCorners(ConnectorOrientation orientation, Rect rect, out Point n1, out Point n2)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    n1 = rect.TopLeft; n2 = rect.BottomLeft;
                    break;
                case ConnectorOrientation.Top:
                    n1 = rect.TopLeft; n2 = rect.TopRight;
                    break;
                case ConnectorOrientation.Right:
                    n1 = rect.TopRight; n2 = rect.BottomRight;
                    break;
                case ConnectorOrientation.Bottom:
                    n1 = rect.BottomLeft; n2 = rect.BottomRight;
                    break;
                default:
                    throw new("No neighour corners found!");
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            return Point.Subtract(p1, p2).Length;
        }

        private static Rect GetRectWithMargin(ConnectorInfo connectorThumb, double margin)
        {
            Rect rect = new(connectorThumb.DesignerItemLeft,
                                 connectorThumb.DesignerItemTop,
                                 0,
                                 0);

            rect.Inflate(margin, margin);

            return rect;
        }

        private static Point GetOffsetPoint(ConnectorInfo connector, Rect rect)
        {
            Point offsetPoint = new();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Left:
                    offsetPoint = new(rect.Left, connector.Position.Y);
                    break;
                case ConnectorOrientation.Top:
                    offsetPoint = new(connector.Position.X, rect.Top);
                    break;
                case ConnectorOrientation.Right:
                    offsetPoint = new(rect.Right, connector.Position.Y);
                    break;
                case ConnectorOrientation.Bottom:
                    offsetPoint = new(connector.Position.X, rect.Bottom);
                    break;
                default:
                    break;
            }

            return offsetPoint;
        }

        private static void CheckPathEnd(ConnectorInfo sourceInfo, ConnectorInfo sinkInfo, bool showLastLine, List<Point> linePoints)
        {
            if (showLastLine)
            {
                Point startPoint = new(0, 0);
                Point endPoint = new(0, 0);
                double marginPath = 15;
                switch (sourceInfo.Orientation)
                {
                    case ConnectorOrientation.Left:
                        startPoint = new(sourceInfo.Position.X - marginPath, sourceInfo.Position.Y);
                        break;
                    case ConnectorOrientation.Top:
                        startPoint = new(sourceInfo.Position.X, sourceInfo.Position.Y - marginPath);
                        break;
                    case ConnectorOrientation.Right:
                        startPoint = new(sourceInfo.Position.X + marginPath, sourceInfo.Position.Y);
                        break;
                    case ConnectorOrientation.Bottom:
                        startPoint = new(sourceInfo.Position.X, sourceInfo.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }

                switch (sinkInfo.Orientation)
                {
                    case ConnectorOrientation.Left:
                        endPoint = new(sinkInfo.Position.X - marginPath, sinkInfo.Position.Y);
                        break;
                    case ConnectorOrientation.Top:
                        endPoint = new(sinkInfo.Position.X, sinkInfo.Position.Y - marginPath);
                        break;
                    case ConnectorOrientation.Right:
                        endPoint = new(sinkInfo.Position.X + marginPath, sinkInfo.Position.Y);
                        break;
                    case ConnectorOrientation.Bottom:
                        endPoint = new(sinkInfo.Position.X, sinkInfo.Position.Y + marginPath);
                        break;
                    default:
                        break;
                }
                linePoints.Insert(0, startPoint);
                linePoints.Add(endPoint);
            }
            else
            {
                linePoints.Insert(0, sourceInfo.Position);
                linePoints.Add(sinkInfo.Position);
            }
        }

        private static ConnectorOrientation GetOpositeOrientation(ConnectorOrientation connectorOrientation)
        {
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Left:
                    return ConnectorOrientation.Right;
                case ConnectorOrientation.Top:
                    return ConnectorOrientation.Bottom;
                case ConnectorOrientation.Right:
                    return ConnectorOrientation.Left;
                case ConnectorOrientation.Bottom:
                    return ConnectorOrientation.Top;
                default:
                    return ConnectorOrientation.None;
            }
        }
    }
}
