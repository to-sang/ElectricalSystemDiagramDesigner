using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace DiagramDesigner.Controls
{
    public class DragThumb : Thumb
    {
        public DragThumb()
        {
            base.DragDelta += new(DragThumb_DragDelta);
            base.DragStarted += new(DragThumb_DragStarted);
            //base.DragCompleted += new(DragThumb_DragCompleted);
        }

        //private void DragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        //{
            
        //}

        private void DragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (this.DataContext is DesignerItemViewModelBase designerItem)
            {
                var designerItems = designerItem.SelectedItems;
                Dictionary<Guid, KeyValuePair<double, double>> previousPositions = new();
                foreach(var item in designerItems.OfType<DesignerItemViewModelBase>())
                    previousPositions.Add(item.Id, new(item.Left, item.Top));
                Default.undoActions.Push(() => UndoMoveCommand(previousPositions, designerItem.Parent));
                Default.redoActions.Clear();
            }    
        }

        private void UndoMoveCommand(Dictionary<Guid, KeyValuePair<double, double>> previousPositions, IDiagramViewModel DiagramViewModel)
        {
            Dictionary<Guid, KeyValuePair<double, double>> futurePositions = new();
            foreach (var previousPosition in previousPositions)
            {
                DesignerItemViewModelBase designerItemViewModel = (DesignerItemViewModelBase)DiagramViewModel.Items.Where(item => item.Id == previousPosition.Key).FirstOrDefault();
                futurePositions.Add(designerItemViewModel.Id, new(designerItemViewModel.Left, designerItemViewModel.Top));
                designerItemViewModel.Left = previousPosition.Value.Key;
                designerItemViewModel.Top = previousPosition.Value.Value;
            }
            Default.redoActions.Push(() => RedoMoveCommand(futurePositions, DiagramViewModel));
            Default.HasChangedAction();
        }

        private void RedoMoveCommand(Dictionary<Guid, KeyValuePair<double, double>> futurePositions, IDiagramViewModel DiagramViewModel)
        {
            Dictionary<Guid, KeyValuePair<double, double>> previousPositions = new();
            foreach (var futurePosition in futurePositions)
            {
                DesignerItemViewModelBase designerItemViewModel = (DesignerItemViewModelBase)DiagramViewModel.Items.Where(item => item.Id == futurePosition.Key).FirstOrDefault();
                previousPositions.Add(designerItemViewModel.Id, new(designerItemViewModel.Left, designerItemViewModel.Top));
                designerItemViewModel.Left = futurePosition.Value.Key;
                designerItemViewModel.Top = futurePosition.Value.Value;
            }
            Default.undoActions.Push(() => UndoMoveCommand(previousPositions, DiagramViewModel));
            Default.HasChangedAction();
        }    
            

        void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.DataContext is DesignerItemViewModelBase designerItem && designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                // we only move DesignerItems
                var designerItems = designerItem.SelectedItems;

                foreach (DesignerItemViewModelBase item in designerItems.OfType<DesignerItemViewModelBase>())
                {
                    double left = item.Left;
                    double top = item.Top;
                    minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                    minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                    double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                    double deltaVertical = Math.Max(-minTop, e.VerticalChange);
                    item.Left += deltaHorizontal;
                    item.Top += deltaVertical;
                    // prevent dragging items out of groupitem
                    if (item.Parent is IDiagramViewModel and DesignerItemViewModelBase)
                    {
                        DesignerItemViewModelBase groupItem = (DesignerItemViewModelBase)item.Parent;
                        if (item.Left + item.ItemWidth >= groupItem.ItemWidth) item.Left = groupItem.ItemWidth - item.ItemWidth;
                        if (item.Top + item.ItemHeight >= groupItem.ItemHeight) item.Top = groupItem.ItemHeight - item.ItemHeight;
                    }

                }
                e.Handled = true;
                Default.HasChangedAction();
            }
        }
    }
}
