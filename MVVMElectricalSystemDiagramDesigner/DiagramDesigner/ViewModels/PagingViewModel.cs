using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace DiagramDesigner
{
    public class PagingViewModel : INPCBase
    {
        public List<PagingItem> ListPage { get; set; } = new();
        public IDiagramViewModel DiagramViewModel => ListPage[SelectedIndex].DiagramViewModel;

        public PagingViewModel(List<PagingItem> listPage)
        {
            this.ListPage = listPage ?? throw new ArgumentNullException(nameof(listPage));
        }
        private int SelectedIndex;
        public PagingViewModel()
        {
            this.ListPage = new()
            {
                new(),
                new(),
                new()
            };
            SelectedIndex = 0;
            NotifyChanged("ListPage", "DiagramViewModel");
            Init();
        }
        public SimpleCommand AddPageCommand { get; private set; }
        private void Init()
        {
            AddPageCommand = new(ExecuteAddPageCommand);

        }
        private void ExecuteAddPageCommand(object obj)
        {
            this.ListPage.Add(new()); 
            this.ListPage[SelectedIndex].IsSelected = false; 
            SelectedIndex = this.ListPage.Count - 1;
            NotifyChanged("ListPage", "DiagramViewModel");
        }
    }

    public class PagingItem : INPCBase
    {
        static int nextID;
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyChanged("Name");
                }
            }
        }
        public int Id { get; private set; }
        private bool isSelected;
        public SimpleCommand EnableEditCommand { get; private set; }
        public SimpleCommand DisableEditCommand { get; private set; }
        public SimpleCommand TestCommand { get; private set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyChanged("IsSelected");
                }
            }
        }
        private bool isReadOnly;

        public bool IsReadOnly
        {
            get => isReadOnly;
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    NotifyChanged("IsReadOnly");
                }
            }
        }

        public IDiagramViewModel DiagramViewModel { get; set; }
        public PagingItem()
        {
            this.Id = Interlocked.Increment(ref nextID);
            this.Name = "New Diagram";
            this.IsSelected = true;
            this.IsReadOnly = true;
            this.DiagramViewModel = new DiagramViewModel();
            Init();
        }

        public PagingItem(string name, int id, IDiagramViewModel DiagramViewModel, bool isSelected, bool canEdit)
        {
            this.Name = name ?? throw new(nameof(name));
            this.Id = id;
            this.DiagramViewModel = DiagramViewModel ?? throw new(nameof(DiagramViewModel));
            this.IsSelected = isSelected;
            this.IsReadOnly = canEdit;
            Init();
        }

        private void Init()
        {
            EnableEditCommand = new((object obj) => isReadOnly, (object obj) => IsReadOnly = false);
            DisableEditCommand = new((object obj) => !isReadOnly, (object obj) => IsReadOnly = true);
            TestCommand = new((object obj) => MessageBox.Show(Name));
        }
    }
}
