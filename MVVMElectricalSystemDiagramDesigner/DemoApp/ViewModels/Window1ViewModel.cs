using System;
using System.Collections.Generic;
using System.Linq;
using DiagramDesigner;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace DemoApp
{
    public partial class Window1ViewModel : INPCBase
    {
        private List<SelectableDesignerItemViewModelBase> itemsToRemove;
        private PagingViewModel pagingViewModel = new();
        private readonly CommandBindingCollection _CommandBindings = new();

        public CommandBindingCollection CommandBindings => _CommandBindings;
        public Window1ViewModel()
        {
            this.ToolBoxViewModel = new();
            this.PagingViewModel = new();
            Default.HasChangedAction = SelectedItemsChanged;

            this.PropertyViewModel = new(DiagramViewModel.SelectedItems);

            ConnectorViewModel.PathFinder = new OrthogonalPathFinder();
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.New, ExecuteCreateNewDiagramCommand));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Open, ExecuteOpenCommand));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Save, ExecuteSaveCommand, SaveCmdCanExecute));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.SaveAs, ExecuteSaveAsCommand, SaveCmdCanExecute));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Copy, ExecuteCopySelectedItemsCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Paste, ExecutePasteCommand, PasteCmdCanExecute));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Cut, ExecuteCutSelectedItemsCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Delete, ExecuteDeleteSelectedItemsCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(Group, ExecuteGroupOrUngroupCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignTop, ExecuteAlignTopCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignVerticalCenters, ExecuteAlignVerticalCentersCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignBottom, ExecuteAlignBottomCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignLeft, ExecuteAlignLeftCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignHorizontalCenters, ExecuteAlignHorizontalCentersCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(AlignRight, ExecuteAlignRightCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(DistributeHorizontal, ExecuteDistributeHorizontalCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(DistributeVertical, ExecuteDistributeVerticalCommand, HasItemsSelected));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.SelectAll, ExecuteSelectAllCommand));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Undo, ExecuteUndoCommand, CanUndoExecute));
            this.AddCommandBinding(new CommandBinding(ApplicationCommands.Redo, ExecuteRedoCommand, CanRedoExecute));
            this.AddCommandBinding(new CommandBinding(Import, ExecuteImportCommand));
            PrintCanvas = new SimpleCommand(ExecutePrintCanvasCommand);
        }

        private void ExecutePrintCanvasCommand(object obj)
        {
            
        }

        private void SelectedItemsChanged()
        {
            this.PropertyViewModel = new(DiagramViewModel.SelectedItems);
        }

        public SimpleCommand PrintCanvas { get; private set; }

        private void AddCommandBinding(CommandBinding binding)
        {
            CommandManager.RegisterClassCommandBinding(GetType(), binding);
            CommandBindings.Add(binding);
        }
        public ToolBoxViewModel ToolBoxViewModel { get; private set; }

        private PropertyViewModel propertyViewModel;

        public PropertyViewModel PropertyViewModel 
        {
            get => propertyViewModel;
            set
            {
                if (propertyViewModel != value)
                {
                    this.propertyViewModel = value;
                    NotifyChanged("PropertyViewModel");
                }
            }
        }


        public PagingViewModel PagingViewModel
        {
            get => pagingViewModel;
            set
            {
                if (pagingViewModel != value)
                {
                    this.pagingViewModel = value;
                    NotifyChanged("PagingViewModel");
                }
            }
        }

        private DiagramViewModel DiagramViewModel => (DiagramViewModel)PagingViewModel.DiagramViewModel;

        private static Rect GetBoundingRectangle(IEnumerable<SelectableDesignerItemViewModelBase> items, double margin)
        {
            double x1 = double.MaxValue;
            double y1 = double.MaxValue;
            double x2 = double.MinValue;
            double y2 = double.MinValue;

            foreach (DesignerItemViewModelBase item in items.OfType<DesignerItemViewModelBase>())
            {
                x1 = Math.Min(item.Left - margin, x1);
                y1 = Math.Min(item.Top - margin, y1);

                x2 = Math.Max(item.Left + item.ItemWidth + margin, x2);
                y2 = Math.Max(item.Top + item.ItemHeight + margin, y2);
            }

            return new(new(x1, y1), new Point(x2, y2));
        }

        private FullyCreatedConnectorInfo GetFullConnectorInfo(Guid connectorId, DesignerItemViewModelBase dataItem, ConnectorOrientation connectorOrientation) => connectorOrientation switch
        {
            ConnectorOrientation.Top => dataItem.TopConnector,
            ConnectorOrientation.Left => dataItem.LeftConnector,
            ConnectorOrientation.Right => dataItem.RightConnector,
            ConnectorOrientation.Bottom => dataItem.BottomConnector,
            _ => throw new(
            string.Format("Found invalid persisted Connector Orientation for Connector Id: {0}", connectorId)),
        };

        private DesignerItemViewModelBase GetConnectorDataItem(IDiagramViewModel DiagramViewModel, Guid conectorDataItemId) 
            => DiagramViewModel.Items.OfType<DesignerItemViewModelBase>().Single(x => x.Id == conectorDataItemId);

        private bool ItemsToDeleteHasConnector(List<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector) 
            => itemsToRemove.Contains(connector.DataItem);
    }
}
