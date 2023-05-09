using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
namespace DiagramDesigner
{
    public class DiagramViewModel : INPCBase, IDiagramViewModel
    {
        private readonly ObservableCollection<SelectableDesignerItemViewModelBase> items = new();

        public DiagramViewModel()
        {
            AddItemCommand = new(ExecuteAddItemCommand);
            RemoveItemCommand = new(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new(ExecuteCreateNewDiagramCommand);

        }

        public SimpleCommand AddItemCommand { get; private set; }
        public SimpleCommand RemoveItemCommand { get; private set; }
        public SimpleCommand ClearSelectedItemsCommand { get; private set; }
        public SimpleCommand CreateNewDiagramCommand { get; private set; }

        public ObservableCollection<SelectableDesignerItemViewModelBase> Items => items;

        public List<SelectableDesignerItemViewModelBase> SelectedItems
        {
            get => Items.Where(x => x.IsSelected).ToList();
        }

        public List<DesignerItemViewModelBase> SelectedDesignerItems { get => Items.Where(x => x.IsSelected && x.GetType().IsSubclassOf(typeof(DesignerItemViewModelBase))).Cast<DesignerItemViewModelBase>().ToList(); }

        private void ExecuteAddItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase item)
            {
                item.Parent = this;
                Items.Add(item);
            }
        }

        private void ExecuteRemoveItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase item)
                Items.Remove(item);
        }

        private void ExecuteClearSelectedItemsCommand(object parameter)
        {
            foreach (SelectableDesignerItemViewModelBase item in Items)
                item.IsSelected = false;
        }

        private void ExecuteCreateNewDiagramCommand(object parameter) => Items.Clear();
    }
}
