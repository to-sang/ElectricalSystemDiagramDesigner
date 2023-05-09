using System.Collections.Generic;
using System.Linq;
using DiagramDesigner;
using System.Collections.ObjectModel;
using System;

namespace DemoApp
{
    public class GroupingDesignerItemViewModel : DesignerItemViewModelBase, IDiagramViewModel
    {

        private readonly ObservableCollection<SelectableDesignerItemViewModelBase> items = new();

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, int angle)
            : base(id, parent, left, top, angle) => Init();

        public GroupingDesignerItemViewModel() : base() => Init();

        public GroupingDesignerItemViewModel(Guid id, IDiagramViewModel parent, double left, double top, double itemWidth, double itemHeight, int angle) 
            : base(id, parent, left, top, itemWidth, itemHeight, angle) => Init();

        public SimpleCommand AddItemCommand { get; private set; }
        public SimpleCommand RemoveItemCommand { get; private set; }
        public SimpleCommand ClearSelectedItemsCommand { get; private set; }
        public SimpleCommand CreateNewDiagramCommand { get; private set; }

        public ObservableCollection<SelectableDesignerItemViewModelBase> Items => items;

        new public List<SelectableDesignerItemViewModelBase> SelectedItems => Items.Where(x => x.IsSelected).ToList();
        public List<DesignerItemViewModelBase> SelectedDesignerItems => Items.Where(x => x.IsSelected && x.GetType().IsSubclassOf(typeof(DesignerItemViewModelBase))).Cast<DesignerItemViewModelBase>().ToList();

        private void ExecuteAddItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase item)
            {
                item.Parent = this;
                items.Add(item);
            }
        }

        private void ExecuteRemoveItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase item)
                items.Remove(item);
        }

        private void ExecuteClearSelectedItemsCommand(object parameter)
        {
            foreach (SelectableDesignerItemViewModelBase item in Items)
                item.IsSelected = false;
        }

        private void ExecuteCreateNewDiagramCommand(object parameter)
        {
            Items.Clear();
        }


        private void Init()
        {
            AddItemCommand = new(ExecuteAddItemCommand);
            RemoveItemCommand = new(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new(ExecuteCreateNewDiagramCommand);

            this.ShowConnectors = false;
        }
    }
}
