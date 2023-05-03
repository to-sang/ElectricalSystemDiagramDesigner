using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainApplication
{
    public partial class MainWindowViewModel
    {
        private readonly CommandBindingCollection _CommandBindings = new();

        public CommandBindingCollection CommandBindings => _CommandBindings;
        public MainWindowViewModel()
        {
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.New, ExecuteCreateNewDiagramCommand));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Open, ExecuteOpenCommand));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Save, ExecuteSaveCommand, SaveCmdCanExecute));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.SaveAs, ExecuteSaveAsCommand, SaveCmdCanExecute));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Copy, ExecuteCopySelectedItemsCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Paste, ExecutePasteCommand, PasteCmdCanExecute));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Cut, ExecuteCutSelectedItemsCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Delete, ExecuteDeleteSelectedItemsCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(Group, ExecuteGroupOrUngroupCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignTop, ExecuteAlignTopCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignVerticalCenters, ExecuteAlignVerticalCentersCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignBottom, ExecuteAlignBottomCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignLeft, ExecuteAlignLeftCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignHorizontalCenters, ExecuteAlignHorizontalCentersCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(AlignRight, ExecuteAlignRightCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(DistributeHorizontal, ExecuteDistributeHorizontalCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(DistributeVertical, ExecuteDistributeVerticalCommand, HasItemsSelected));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.SelectAll, ExecuteSelectAllCommand));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Undo, ExecuteUndoCommand, CanUndoExecute));
            //this.AddCommandBinding(new CommandBinding(ApplicationCommands.Redo, ExecuteRedoCommand, CanRedoExecute));
            //this.AddCommandBinding(new CommandBinding(Import, ExecuteImportCommand));
        }

        private void AddCommandBinding(CommandBinding binding)
        {
            CommandManager.RegisterClassCommandBinding(GetType(), binding);
            CommandBindings.Add(binding);
        }
    }
}
