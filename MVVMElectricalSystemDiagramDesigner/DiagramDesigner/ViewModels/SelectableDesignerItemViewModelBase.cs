using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner
{

    public interface ISelectItems
    {
        SimpleCommand SelectItemCommand { get; }
    }


    public abstract class SelectableDesignerItemViewModelBase : INPCBase, ISelectItems
    {
        private bool isSelected;

        public SelectableDesignerItemViewModelBase(Guid id, IDiagramViewModel parent)
        {
            this.Id = id;
            this.Parent = parent;
            Init();
        }

        public SelectableDesignerItemViewModelBase()
        {
            Init();
        }
        public List<SelectableDesignerItemViewModelBase> SelectedItems => Parent.SelectedItems;
        
        public IDiagramViewModel Parent { get; set; }
        public SimpleCommand SelectItemCommand { get; private set; }
        public Guid Id { get; set; }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    Default.HasChangedAction();
                    NotifyChanged("IsSelected");
                }
            }
        }

        private void ExecuteSelectItemCommand(object param) => SelectItem((bool)param, !IsSelected);
        

        private void SelectItem(bool newselect, bool select)
        {
            if (newselect)
                foreach (var designerItemViewModelBase in Parent.SelectedItems.ToList())
                    designerItemViewModelBase.isSelected = false;

            IsSelected = select;
        }

        private void Init()
        {
            SelectItemCommand = new(ExecuteSelectItemCommand);
        }

    }
}
