using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public partial class DesignerCanvas : Canvas
    {

        private ConnectorViewModel partialConnection;
        private Connector sourceConnector;
        private Connector sinkConnector;
        private Point? rubberbandSelectionStartPoint = null;
        private FullyCreatedConnectorInfo sinkDataItem;

        public DesignerCanvas()
        {
            this.AllowDrop = true;
        }

        public Connector SourceConnector
        {
            get => sourceConnector;
            set
            {
                if (sourceConnector != value)
                {
                    sourceConnector = value;
                    FullyCreatedConnectorInfo sourceDataItem = sourceConnector.DataContext as FullyCreatedConnectorInfo;


                    Rect rectangleBounds = sourceConnector.TransformToVisual(this).TransformBounds(new(sourceConnector.RenderSize));
                    Point point = new(rectangleBounds.Left + (rectangleBounds.Width / 2),
                                            rectangleBounds.Bottom + (rectangleBounds.Height / 2));
                    partialConnection = new(sourceDataItem, new PartCreatedConnectionInfo(point));
                    sourceDataItem.DataItem.Parent.AddItemCommand.Execute(partialConnection);
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if we are source of event, we are rubberband selecting
                if (e.Source == this)
                {
                    // in case that this click is the start for a 
                    // drag operation we cache the start point
                    rubberbandSelectionStartPoint = e.GetPosition(this);

                    IDiagramViewModel vm = (this.DataContext as IDiagramViewModel);
                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        vm.ClearSelectedItemsCommand.Execute(null);
                    }
                    e.Handled = true;
                }
            }


        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (sourceConnector != null)
            {
                FullyCreatedConnectorInfo sourceDataItem = sourceConnector.DataContext as FullyCreatedConnectorInfo;
                if (sinkConnector != null)
                {
                    int indexOfLastTempConnection = sinkDataItem.DataItem.Parent.Items.Count - 1;
                    sinkDataItem.DataItem.IsSelected = false;
                    sinkDataItem.DataItem.Parent.RemoveItemCommand.Execute(
                        sinkDataItem.DataItem.Parent.Items[indexOfLastTempConnection]);
                    if (sinkDataItem.DataItem != sourceDataItem.DataItem)
                    {
                        ConnectorViewModel con = new(sourceDataItem, sinkDataItem);
                        sinkDataItem.DataItem.Parent.AddItemCommand.Execute(con);
                        Default.undoActions.Push(() => UndoAddObject(con));
                        Default.redoActions.Clear();
                    }    
                    
                }
                else
                {
                    //Need to remove last item as we did not finish drawing the path
                    int indexOfLastTempConnection = sourceDataItem.DataItem.Parent.Items.Count - 1;
                    sourceDataItem.DataItem.Parent.RemoveItemCommand.Execute(
                        sourceDataItem.DataItem.Parent.Items[indexOfLastTempConnection]);
                }
            }
            sinkConnector = null;
            sourceConnector = null;
            sinkDataItem = null;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (SourceConnector != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point position = e.GetPosition(this);
                    partialConnection.SinkConnectorInfo = new PartCreatedConnectionInfo(position);
                    sinkConnector = null;
                    int cnt = 0;

                    while (sinkConnector == null && cnt < 20)
                    {
                        for (int i = 0; i <= cnt; ++i)
                        {
                            HitTesting(new(position.X - i, position.Y + cnt));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X + i, position.Y + cnt));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X - i, position.Y - cnt));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X + i, position.Y - cnt));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X - cnt, position.Y + i));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X + cnt, position.Y + i));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X - cnt, position.Y - i));
                            if (sinkConnector != null)
                                break;
                            HitTesting(new(position.X + cnt, position.Y - i));
                        }
                        ++cnt;
                    }
                    if (sinkConnector != null)
                    {
                        if (sinkDataItem != null)
                            sinkDataItem.DataItem.IsSelected = false;
                        sinkDataItem = sinkConnector.DataContext as FullyCreatedConnectorInfo;
                        sinkDataItem.DataItem.IsSelected = true;
                    }
                    else
                        if (sinkDataItem != null)
                            sinkDataItem.DataItem.IsSelected = false;
                }
            }
            else
            {
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                    rubberbandSelectionStartPoint = null;

                // ... but if mouse button is pressed and start
                // point value is set we do have one
                if (this.rubberbandSelectionStartPoint.HasValue)
                {
                    // create rubberband adorner
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (adornerLayer != null)
                    {
                        RubberbandAdorner adorner = new(this, rubberbandSelectionStartPoint);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                        }
                    }
                }
            }
            e.Handled = true;
        }


        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        public void HitTesting(Point hitPoint)
        {
            DependencyObject hitObject = this.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                    hitObject.GetType() != typeof(DesignerCanvas))
            {
                if (hitObject is Connector connector)
                    sinkConnector = connector;
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (e.Data.GetData(typeof(DragObject)) is DragObject dragObject)
            {
                (DataContext as IDiagramViewModel).ClearSelectedItemsCommand.Execute(null);
                Point position = e.GetPosition(this);
                DesignerItemViewModelBase itemBase = (DesignerItemViewModelBase)Activator.CreateInstance(dragObject.ContentType);
                itemBase.Id = Guid.NewGuid();
                itemBase.Left = Math.Max(0, position.X - itemBase.ItemWidth / 2);
                itemBase.Top = Math.Max(0, position.Y - itemBase.ItemHeight / 2);
                itemBase.IsSelected = true;
                (DataContext as IDiagramViewModel).AddItemCommand.Execute(itemBase);
                Default.undoActions.Push(() => UndoAddObject(itemBase));
                Default.redoActions.Clear();
                Default.HasChangedAction();
            }
            e.Handled = true;
        }

        private void UndoAddObject(SelectableDesignerItemViewModelBase itemBase)
        {
            (DataContext as IDiagramViewModel).RemoveItemCommand.Execute(itemBase);
            Default.redoActions.Push(() => RedoAddObject(itemBase));
            Default.HasChangedAction();
        }

        private void RedoAddObject(SelectableDesignerItemViewModelBase itemBase)
        {
            (DataContext as IDiagramViewModel).AddItemCommand.Execute(itemBase);
            Default.undoActions.Push(() => UndoAddObject(itemBase));
            Default.HasChangedAction();
        }
}
}
