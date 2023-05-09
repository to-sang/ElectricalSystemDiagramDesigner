using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class Connector : Control
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DesignerCanvas canvas = GetDesignerCanvas(this);
            if (canvas != null)
            {
                canvas.SourceConnector = this;
                canvas.HitTesting(e.GetPosition(this));
            }    
        }

        public ConnectorOrientation Orientation { get; set; }

        // iterate through visual tree to get parent DesignerCanvas
        private DesignerCanvas GetDesignerCanvas(DependencyObject element)
        {
            while (element != null && element is not DesignerCanvas)
                element = VisualTreeHelper.GetParent(element);

            return element as DesignerCanvas;
        }

    }

    public enum ConnectorOrientation
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 3,
        Bottom = 4
    }
}
