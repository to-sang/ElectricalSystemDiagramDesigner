using System;
using System.Collections.Generic;
using System.Linq;
using DiagramDesigner;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;
using System.Globalization;
using Microsoft.Win32;
//using Newtonsoft.Json;

namespace DemoApp
{
    public partial class Window1ViewModel
    {

        #region Routed Command

        public static RoutedCommand Group = new();
        public static RoutedCommand BringForward = new();
        public static RoutedCommand BringToFront = new();
        public static RoutedCommand SendBackward = new();
        public static RoutedCommand SendToBack = new();
        public static RoutedCommand AlignTop = new();
        public static RoutedCommand AlignVerticalCenters = new();
        public static RoutedCommand AlignBottom = new();
        public static RoutedCommand AlignLeft = new();
        public static RoutedCommand AlignHorizontalCenters = new();
        public static RoutedCommand AlignRight = new();
        public static RoutedCommand DistributeHorizontal = new();
        public static RoutedCommand DistributeVertical = new();
        public static RoutedCommand SelectAll = new();
        public static RoutedCommand Import = new();

        #endregion

        #region Create New Diagram Command

        private void ExecuteCreateNewDiagramCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //ensure that itemsToRemove is cleared ready for any new changes within a session
            itemsToRemove = new();
            fileName = MyTitle = null;
            DiagramViewModel.CreateNewDiagramCommand.Execute(null);
        }

        #endregion

        #region Open Command

        private void ExecuteOpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "JSON File (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Open diagram"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
                MyTitle = openFileDialog.SafeFileName;
                string contentDiagram = File.ReadAllText(fileName); 
                itemsToRemove = new();
                DiagramViewModel.CreateNewDiagramCommand.Execute(null);
                MessageBox.Show("Function will be added in the future", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Save Command

        private string fileName;

        private string myTitle;
        public string MyTitle
        {
            get => "Diagram Builder\t\t" + myTitle;
            set
            {
                myTitle = value;
                NotifyChanged("MyTitle");
            }
        }

        private void ExecuteSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (fileName == null)
                ExecuteSaveAsCommand(sender, e);
            else
                File.WriteAllText(fileName, SaveContent());

        }
        private string SaveContent()
        {
            string content = "[\n\t{\n\t\t\"name\": \"diagram test\",\n\t\t\"diagram\": {\n\t\t\t\"devices\": [\n";
            foreach (var item in DiagramViewModel.Items.OfType<DesignerItemViewModelBase>())
                content += item.ToJSON() + ",\n";
            if (DiagramViewModel.Items.OfType<DesignerItemViewModelBase>().Count() > 0)
                content = content.Substring(0, content.Length - 2);
            content += "\n\t\t\t],\n\t\t\t\"connections\": [";
            foreach (var connection in DiagramViewModel.Items.OfType<ConnectorViewModel>())
                content += connection.ToJSON() + ",\n";
            if (DiagramViewModel.Items.OfType<ConnectorViewModel>().Count() > 0)
                content = content.Substring(0, content.Length - 2);
            content += "\n\t\t\t]\n\t\t}\n\t}\n]";
            return content;
        }
        private void ExecuteSaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "JSON File (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Save diagram"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                fileName = saveFileDialog.FileName;
                MyTitle = saveFileDialog.SafeFileName;
                File.WriteAllText(fileName, SaveContent());
            }    
        }
        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.DiagramViewModel.Items.Count > 0;
        }

        #endregion

        #region Copy Seleted Items Command

        private void ExecuteCopySelectedItemsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        #endregion

        #region Cut Seleted Items Command

        private void ExecuteCutSelectedItemsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
            Default.redoActions.Clear();
        }

        #endregion

        #region Paste Command

        private readonly List<Guid> undoPaste = new();
        private int cntPaste;

        private void ExecutePasteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromClipBoard();
            Default.redoActions.Clear();

            if (root == null)
                return;
            RedoPasteCommand(root);
        }

        private void UndoPasteCommand(List<Guid> idList, XElement root)
        {
            foreach (Guid id in idList)
                this.DiagramViewModel.Items.Remove(this.DiagramViewModel.Items.Where(x => x.Id.Equals(id)).Single());
            Default.redoActions.Push(() => RedoPasteCommand(root, idList));
            Default.HasChangedAction();
        }

        private void RedoPasteCommand(XElement root, List<Guid> redoPaste = null)
        {
            undoPaste.Clear();
            cntPaste = 0;
            DiagramViewModel.ClearSelectedItemsCommand.Execute(null);
            Dictionary<Guid, Guid> mappingOldToNewIDs = new();
            IEnumerable<XElement> groupsXML = root.Elements("Copied").Elements("Group");

            double offsetX = double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement groupXML in groupsXML)
            {
                Guid oldID = new(groupXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                if (redoPaste != null)
                    newID = redoPaste[cntPaste++];
                mappingOldToNewIDs.Add(oldID, newID);
                GroupingDesignerItemViewModel itemVM = (GroupingDesignerItemViewModel)DeserializeDesignerItems(groupXML, newID, offsetX, offsetY);
                AddXMLDataToViewModel(new List<XElement> { groupXML }, mappingOldToNewIDs, 0, 0, redoPaste: redoPaste, groupVM: itemVM);
                DiagramViewModel.AddItemCommand.Execute(itemVM);
                undoPaste.Add(itemVM.Id);
            }

            AddXMLDataToViewModel(root.Elements("Copied"), mappingOldToNewIDs, offsetX, offsetY, redoPaste: redoPaste, diagramVM: DiagramViewModel);

            XElement curXElement = new(root);

            List<Guid> ahuhu = new(undoPaste.Count());
            undoPaste.ForEach(x => ahuhu.Add(x));
            Default.undoActions.Push(() => UndoPasteCommand(ahuhu, curXElement));

            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
            Default.HasChangedAction();
        }

        private void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Selected Items Command

        private void ExecuteDeleteSelectedItemsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
            Default.redoActions.Clear();
        }

        #endregion

        #region Group / Ungroup Command

        private void ExecuteGroupOrUngroupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Default.redoActions.Clear();
            if (DiagramViewModel.SelectedItems.Count > 0)
            {
                // if only one selected item is a Grouping item -> ungroup
                if (DiagramViewModel.SelectedItems[0] is GroupingDesignerItemViewModel && DiagramViewModel.SelectedItems.Count == 1)
                {
                    GroupingDesignerItemViewModel groupObject = DiagramViewModel.SelectedItems[0] as GroupingDesignerItemViewModel;
                    foreach (var item in groupObject.Items)
                    {

                        if (item is DesignerItemViewModelBase tmp)
                        {
                            tmp.Top += groupObject.Top;
                            tmp.Left += groupObject.Left;
                        }
                        DiagramViewModel.AddItemCommand.Execute(item);
                        item.Parent = DiagramViewModel;
                    }

                    // "cut" connections between DiagramItems and the Group
                    List<SelectableDesignerItemViewModelBase> GroupedItemsToRemove = new();
                    foreach (var connector in DiagramViewModel.Items.OfType<ConnectorViewModel>())
                    {
                        if (groupObject == connector.SourceConnectorInfo.DataItem)
                            GroupedItemsToRemove.Add(connector);

                        if (groupObject == ((FullyCreatedConnectorInfo)connector.SinkConnectorInfo).DataItem)
                            GroupedItemsToRemove.Add(connector);
                    }
                    GroupedItemsToRemove.Add(groupObject);
                    foreach (var selectedItem in GroupedItemsToRemove)
                        DiagramViewModel.RemoveItemCommand.Execute(selectedItem);   

                }
                else if (DiagramViewModel.SelectedItems.Count > 1)
                {
                    double margin = 15;
                    Rect rekt = GetBoundingRectangle(DiagramViewModel.SelectedItems, margin);

                    GroupingDesignerItemViewModel groupItem = new(Guid.NewGuid(), this.DiagramViewModel, rekt.Left, rekt.Top, 0)
                    {
                        ItemWidth = rekt.Width,
                        ItemHeight = rekt.Height
                    };
                    foreach (var item in DiagramViewModel.SelectedItems)
                    {
                        if (item is DesignerItemViewModelBase tmp)
                        {
                            tmp.Top -= rekt.Top;
                            tmp.Left -= rekt.Left;
                        }
                        groupItem.Items.Add(item);
                        item.Parent = groupItem;

                    }

                    // "cut" connections between DiagramItems which are going to be grouped and
                    // Diagramitems which are not going to be grouped
                    List<SelectableDesignerItemViewModelBase> GroupedItemsToRemove = DiagramViewModel.SelectedItems;
                    List<SelectableDesignerItemViewModelBase> connectionsToAlsoRemove = new();

                    foreach (var connector in DiagramViewModel.Items.OfType<ConnectorViewModel>())
                    {
                        if (ItemsToDeleteHasConnector(GroupedItemsToRemove, connector.SourceConnectorInfo))
                            connectionsToAlsoRemove.Add(connector);

                        if (ItemsToDeleteHasConnector(GroupedItemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                            connectionsToAlsoRemove.Add(connector);

                    }
                    GroupedItemsToRemove.AddRange(connectionsToAlsoRemove);
                    foreach (var selectedItem in GroupedItemsToRemove)
                        DiagramViewModel.RemoveItemCommand.Execute(selectedItem);

                    DiagramViewModel.SelectedItems.Clear();
                    this.DiagramViewModel.Items.Add(groupItem);
                }
            }
        }


        #endregion

        #region Alignment

        private Dictionary<Guid, Point> undoAlignment = new();

        private void UndoAlignmentCommand(Dictionary<Guid, Point> previousAlignment, string alignment)
        {
            List<Guid> redoAlignment = new();
            foreach (var item in previousAlignment)
            {
                ((DesignerItemViewModelBase)DiagramViewModel.Items.Where(it => it.Id.Equals(item.Key)).Single()).Top = item.Value.Y;
                ((DesignerItemViewModelBase)DiagramViewModel.Items.Where(it => it.Id.Equals(item.Key)).Single()).Left = item.Value.X;
                redoAlignment.Add(item.Key);
            }
            Default.redoActions.Push(() => RedoAlignmentCommand(redoAlignment, alignment));
        }

        private void RedoAlignmentCommand(List<Guid> redoAlignment, string alignment)
        {
            undoAlignment = new();
            foreach (SelectableDesignerItemViewModelBase itemVMBase in DiagramViewModel.SelectedItems)
                itemVMBase.IsSelected = false;
            foreach (var itemID in redoAlignment)
                DiagramViewModel.Items.Where(x => x.Id.Equals(itemID)).Single().IsSelected = true;
            DiagramViewModel.SelectedDesignerItems.ForEach(x => undoAlignment.Add(x.Id, new(x.Left, x.Top)));

            switch (alignment)
            {
                case "top":
                    double top = DiagramViewModel.SelectedDesignerItems.Min(x => x.Top + x.ItemHeight / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Top = top - x.ItemHeight / 2);
                    break;
                case "left":
                    double left = DiagramViewModel.SelectedDesignerItems.Min(x => x.Left + x.ItemWidth / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Left = left - x.ItemWidth / 2);
                    break;
                case "right":
                    double right = DiagramViewModel.SelectedDesignerItems.Max(x => x.Left + x.ItemWidth / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Left = right - x.ItemWidth / 2);
                    break;
                case "centerHorizontal":
                    double minLeft = DiagramViewModel.SelectedDesignerItems.Min(x => x.Left + x.ItemWidth / 2);
                    double maxLeft = DiagramViewModel.SelectedDesignerItems.Max(x => x.Left + x.ItemWidth / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Left = (maxLeft + minLeft) / 2 - x.ItemWidth / 2);
                    break;
                case "bottom":
                    double bottom = DiagramViewModel.SelectedDesignerItems.Max(x => x.Top + x.ItemHeight / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Top = bottom - x.ItemHeight / 2);
                    break;
                case "centerVertical":
                    double minTop = DiagramViewModel.SelectedDesignerItems.Min(x => x.Top + x.ItemHeight / 2);
                    double maxTop = DiagramViewModel.SelectedDesignerItems.Max(x => x.Top + x.ItemHeight / 2);
                    DiagramViewModel.SelectedDesignerItems.ForEach(x => x.Top = (maxTop + minTop) / 2 - x.ItemHeight / 2);
                    break;
                case "distributeHorizontal":
                    if (DiagramViewModel.SelectedDesignerItems.Count() <= 1)
                        return;
                    double curLeft = DiagramViewModel.SelectedDesignerItems.Min(x => x.Left + x.ItemWidth / 2);
                    double maxxLeft = DiagramViewModel.SelectedDesignerItems.Max(x => x.Left + x.ItemWidth / 2);
                    double distance = (maxxLeft - curLeft) / (DiagramViewModel.SelectedDesignerItems.Count() - 1);
                    int cnt = 0;
                    DiagramViewModel.SelectedDesignerItems.OrderBy(x => x.Left + x.ItemWidth / 2).ToList().ForEach(x => x.Left = curLeft + (cnt++) * distance - x.ItemWidth / 2);
                    break;
                case "distributeVertical":
                    if (DiagramViewModel.SelectedDesignerItems.Count() <= 1)
                        return;
                    double curTop = DiagramViewModel.SelectedDesignerItems.Min(x => x.Top + x.ItemHeight / 2);
                    double maxxTop = DiagramViewModel.SelectedDesignerItems.Max(x => x.Top + x.ItemHeight / 2);
                    double distancee = (maxxTop - curTop) / (DiagramViewModel.SelectedDesignerItems.Count() - 1);
                    int cntt = 0;
                    DiagramViewModel.SelectedDesignerItems.OrderBy(x => x.Top + x.ItemHeight / 2).ToList().ForEach(x => x.Top = curTop + (cntt++) * distancee - x.ItemHeight / 2);
                    break;
            }


            Dictionary<Guid, Point> alignments = undoAlignment.ToDictionary(entry => entry.Key, entry => entry.Value);
            Default.undoActions.Push(() => UndoAlignmentCommand(alignments, alignment));
        }

        #region AlignTop Command

        private void ExecuteAlignTopCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "top");
            Default.redoActions.Clear();
        }

        #endregion

        #region AlignVerticalCenters Command

        private void ExecuteAlignVerticalCentersCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "centerVertical");
            Default.redoActions.Clear();
        }

        #endregion

        #region AlignBottom Command

        private void ExecuteAlignBottomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "bottom");
            Default.redoActions.Clear();
        }

        #endregion

        #region AlignLeft Command

        private void ExecuteAlignLeftCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "left");
            Default.redoActions.Clear();
        }

        #endregion

        #region AlignHorizontalCenters Command

        private void ExecuteAlignHorizontalCentersCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "centerHorizontal");
            Default.redoActions.Clear();
        }

        #endregion

        #region AlignRight Command

        private void ExecuteAlignRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "right");
            Default.redoActions.Clear();
        }

        #endregion

        #region Distribute Horizontal Command

        private void ExecuteDistributeHorizontalCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "distributeHorizontal");
            Default.redoActions.Clear();
        }

        #endregion

        #region Distribute Vertical Command

        private void ExecuteDistributeVerticalCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RedoAlignmentCommand(DiagramViewModel.SelectedDesignerItems.Select(x => x.Id).ToList(), "distributeVertical");
            Default.redoActions.Clear();
        }

        #endregion

        #endregion

        #region SelectAll Command

        private void ExecuteSelectAllCommand(object sender, ExecutedRoutedEventArgs e)
        {
            for (int i = 0; i < DiagramViewModel.Items.Count(); ++i)
                DiagramViewModel.Items[i].IsSelected = true;
        }

        #endregion

        #region Undo

        private void ExecuteUndoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Action command = Default.undoActions.Pop();
            command();

        }

        private void CanUndoExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Default.undoActions.Count() > 0;
        }

        #endregion

        #region Redo

        private void ExecuteRedoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Action command = Default.redoActions.Pop();
            command();
        }

        private void CanRedoExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Default.redoActions.Count() > 0;
        }

        #endregion

        #region Import

        private void ExecuteImportCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "XML File (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Import data"
            };
            if (dialog.ShowDialog() == true)
            {
                BaseSiteServices.Instance();
                try
                {
                    XElement xmlData = XElement.Load(new StringReader(File.ReadAllText(dialog.FileName))).Element("Site");

                    foreach(XElement sub in xmlData.Elements("Substation"))
                    {
                        Substation substation = new(int.Parse(sub.Attribute("_id").Value), 
                            sub.Attribute("Name").Value, sub.Attribute("Description").Value, new(), new());
                        BaseSiteServices.AddBaseSite(substation);
                        foreach(XElement voltage in sub.Elements("VoltageLevel"))
                        {
                            VoltageLevel voltageLevel = new(int.Parse(voltage.Attribute("_id").Value),
                                voltage.Attribute("Name").Value, voltage.Attribute("Description").Value, substation, new());
                            BaseSiteServices.AddBaseSite(voltageLevel);
                            substation.VoltageLevels.Add(voltageLevel);
                            foreach (XElement bayy in voltage.Elements("Bay"))
                            {
                                Bay bay = new(int.Parse(bayy.Attribute("_id").Value),
                                    bayy.Attribute("Name").Value, bayy.Attribute("Description").Value, voltageLevel, new());
                                BaseSiteServices.AddBaseSite(bay);
                                voltageLevel.Bays.Add(bay);
                                foreach (XElement deviceChild in bayy.Elements())
                                {
                                    Device device = new(int.Parse(deviceChild.Attribute("_id").Value),
                                    deviceChild.Attribute("Name").Value, deviceChild.Attribute("Description").Value, deviceChild.Name.LocalName switch
                                    {
                                        "CircuitBreaker" => "Circuit Breaker",
                                        "DisconnectorSwitch" => "Disconnector Switch",
                                        "EarthSwitch" => "Earth Switch",
                                        "Busbar" => "BusBar",
                                        "Source" => "Source",
                                        "Transformer" => "Transformer",
                                        "Generator" => "Generator",
                                        "Fan" => "Fan",
                                        "LineIndicator" => "Line Indicator",
                                        "Lockout" => "Trip Lockout Replay",
                                        _ => "Undefined",
                                    }, deviceChild.Attribute("Label") != null ? deviceChild.Attribute("Label").Value : string.Empty, false, bay);
                                    BaseSiteServices.AddBaseSite(device);
                                    bay.Devices.Add(device);
                                }
                            }
                        }
                        foreach (XElement trans in sub.Elements("PowerTransformer"))
                        {
                            Transformer transformer = new(int.Parse(trans.Attribute("_id").Value), trans.Attribute("Name").Value,
                                trans.Attribute("Description").Value, int.Parse(trans.Attribute("NumberOfWinding").Value), substation, false, new());
                            BaseSiteServices.AddBaseSite(transformer);
                            substation.Transformers.Add(transformer);
                            foreach (XElement fann in trans.Elements("Fan"))
                            {
                                Fan fan = new(int.Parse(fann.Attribute("_id").Value),
                                    fann.Attribute("Name").Value, fann.Attribute("Description").Value, fann.Attribute("Label") != null ? fann.Attribute("Label").Value : string.Empty, false, transformer);
                                BaseSiteServices.AddBaseSite(fan);
                                transformer.Fans.Add(fan);
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show("Import data from XML File successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach(var item in DiagramViewModel.Items)
                {
                    Device device = BaseSiteServices.FindBaseSiteByID<Device>(Default.IdInt(item.Id));
                    if (device != null) device.IsSelected = true;
                }
                Default.HasChangedAction();
            }
        }

        #endregion

        #region Helper Methods

        private void HasItemsSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.DiagramViewModel.SelectedItems.Count() > 0;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<SelectableDesignerItemViewModelBase> selectedDesignerItems =
                DiagramViewModel.SelectedItems;

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems, "Copied");

            XElement root = new("Root");
            root.Add(designerItemsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private XElement SerializeDesignerItems(IEnumerable<SelectableDesignerItemViewModelBase> designerItems, XName xElementName, GroupingDesignerItemViewModel groupVM = null)
        {
            XElement serializedItems = new(xElementName);

            if (groupVM != null)
                serializedItems.Add(new XElement("ID", groupVM.Id),
                                    new XElement("Height", groupVM.ItemHeight),
                                    new XElement("Width", groupVM.ItemWidth),
                                    new XElement("Left", groupVM.Left),
                                    new XElement("Top", groupVM.Top),
                                    new XElement("Angle", groupVM.Angle));

            //Device
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<BusbarDesignerItemViewModel>(), "Busbar");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<DisconnectorSwitchDesignerItemViewModel>(), "DisconnectorSwitch");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<CircuitBreakerDesignerItemViewModel>(), "CircuitBreaker");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<GroundDesignerItemViewModel>(), "Ground");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<SourceDesignerItemViewModel>(), "Source");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<TransformerDesignerItemViewModel>(), "Transformer");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<FanDesignerItemViewModel>(), "Fan");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<GeneratorDesignerItemViewModel>(), "Generator");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<TripLockoutReplayDesignerItemViewModel>(), "TripLockoutReplay");
            AddDevicesToSerializedItems(serializedItems, designerItems.OfType<LineIndicatorDesignerItemViewModel>(), "LineIndicator");

            // Group
            serializedItems.Add(from item in
                                    (from item in designerItems
                                     where item as GroupingDesignerItemViewModel != null
                                     select item as GroupingDesignerItemViewModel)
                                select SerializeDesignerItems(item.Items, "Group", item));

            //Connector
            serializedItems.Add(from item in
                                    (from item in designerItems
                                     where item as ConnectorViewModel != null
                                     select item as ConnectorViewModel)
                                select new XElement("Connector",
                                                    new XElement("ID", item.Id),
                                                    new XElement("SourceConnector",
                                                    new XElement("ID", item.SourceConnectorInfo.DataItem.Id),
                                                    new XElement("Orientation", item.SourceConnectorInfo.Orientation)),
                                                    new XElement("SinkConnector",
                                                    new XElement("ID", ((FullyCreatedConnectorInfo)item.SinkConnectorInfo).DataItem.Id),
                                                    new XElement("Orientation", item.SinkConnectorInfo.Orientation))));

            return serializedItems;
        }

        private void AddDevicesToSerializedItems<T>(XElement serializedItems, IEnumerable<T> designerItems, string type) where T: DesignerItemViewModelBase
        {
            serializedItems.Add(from item in designerItems
                                select new XElement("Device",
                                                    new XAttribute("Type", type),
                                                    new XElement("ID", item.Id),
                                                    new XElement("Height", item.ItemHeight),
                                                    new XElement("Width", item.ItemWidth),
                                                    new XElement("Left", item.Left),
                                                    new XElement("Top", item.Top),
                                                    new XElement("Angle", item.Angle)));
        }
        private DesignerItemViewModelBase DeserializeDesignerItems(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItemViewModelBase itemVM;
            if (itemXML.Attribute("Type") == null)
                itemVM = new GroupingDesignerItemViewModel();
            else
                itemVM = itemXML.Attribute("Type").Value switch
                {
                    "Busbar" => new BusbarDesignerItemViewModel(),
                    "DisconnectorSwitch" => new DisconnectorSwitchDesignerItemViewModel(),
                    "CircuitBreaker" => new CircuitBreakerDesignerItemViewModel(),
                    "Ground" => new GroundDesignerItemViewModel(),
                    "Source" => new SourceDesignerItemViewModel(),
                    "Transformer" => new TransformerDesignerItemViewModel(),
                    "Generator" => new GeneratorDesignerItemViewModel(),
                    "Fan" => new FanDesignerItemViewModel(),
                    "LineIndicator" => new LineIndicatorDesignerItemViewModel(),
                    "TripLockoutReplay" => new TripLockoutReplayDesignerItemViewModel(),
                    _ => null
                };
            itemVM.Id = id;
            itemVM.Parent = DiagramViewModel;
            itemVM.Angle = Convert.ToInt16(itemXML.Element("Angle").Value);
            itemVM.Left = Convert.ToDouble(itemXML.Element("Left").Value) + OffsetX;
            itemVM.Top = Convert.ToDouble(itemXML.Element("Top").Value) + OffsetY;
            itemVM.ItemWidth = Convert.ToDouble(itemXML.Element("Width").Value);
            itemVM.ItemHeight = Convert.ToDouble(itemXML.Element("Height").Value);
            itemVM.IsSelected = true;
            return itemVM;
        }

        private ConnectorViewModel DeserializeConnector(XElement itemXML, Dictionary<Guid, Guid> mappingOldToNewIDs, IDiagramViewModel viewModel, bool undoDelete, Guid newID = new Guid())
        {
            DesignerItemViewModelBase sourceItem, sinkItem;
            Guid connectionID = Guid.NewGuid();
            if (!newID.Equals(Guid.Empty))
                connectionID = newID;
            if (undoDelete)
            {
                sourceItem = GetConnectorDataItem(viewModel, new Guid(itemXML.Element("SourceConnector").Element("ID").Value));
                sinkItem = GetConnectorDataItem(viewModel, new Guid(itemXML.Element("SinkConnector").Element("ID").Value));
                connectionID = new Guid(itemXML.Element("ID").Value);
            }    
            else
            {
                if (mappingOldToNewIDs.ContainsKey(new Guid(itemXML.Element("SourceConnector").Element("ID").Value)))
                    sourceItem = GetConnectorDataItem(viewModel, mappingOldToNewIDs[new Guid(itemXML.Element("SourceConnector").Element("ID").Value)]);
                else
                    return null;

                if (mappingOldToNewIDs.ContainsKey(new Guid(itemXML.Element("SinkConnector").Element("ID").Value)))
                    sinkItem = GetConnectorDataItem(viewModel, mappingOldToNewIDs[new Guid(itemXML.Element("SinkConnector").Element("ID").Value)]);
                else
                    return null;
            }

            ConnectorOrientation sourceConnectorOrientation = (ConnectorOrientation)Enum.Parse(typeof(ConnectorOrientation), itemXML.Element("SourceConnector").Element("Orientation").Value);
            FullyCreatedConnectorInfo sourceConnectorInfo = GetFullConnectorInfo(connectionID, sourceItem, sourceConnectorOrientation);

            ConnectorOrientation sinkConnectorOrientation = (ConnectorOrientation)Enum.Parse(typeof(ConnectorOrientation), itemXML.Element("SinkConnector").Element("Orientation").Value);
            FullyCreatedConnectorInfo sinkConnectorInfo = GetFullConnectorInfo(connectionID, sinkItem, sinkConnectorOrientation);

            return new(connectionID, DiagramViewModel, sourceConnectorInfo, sinkConnectorInfo);
        }

        private void DeleteCurrentSelection()
        {
            itemsToRemove = DiagramViewModel.SelectedItems;
            List<SelectableDesignerItemViewModelBase> connectionsToAlsoRemove = new();

            foreach (var connector in DiagramViewModel.Items.OfType<ConnectorViewModel>())
            {
                if (ItemsToDeleteHasConnector(itemsToRemove, connector.SourceConnectorInfo))
                    connectionsToAlsoRemove.Add(connector);

                if (ItemsToDeleteHasConnector(itemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                    connectionsToAlsoRemove.Add(connector);

            }
            itemsToRemove.AddRange(connectionsToAlsoRemove);

            XElement designerItemsXML = SerializeDesignerItems(itemsToRemove, "Copied");

            XElement root = new("Root");
            root.Add(designerItemsXML);

            foreach (var selectedItem in itemsToRemove)
            {
                DiagramViewModel.RemoveItemCommand.Execute(selectedItem);
                if (selectedItem is not ConnectorViewModel)
                {
                    int id = Default.IdInt(selectedItem.Id);
                    Device device = BaseSiteServices.FindBaseSiteByID<Device>(id);
                    Transformer transformer = BaseSiteServices.FindBaseSiteByID<Transformer>(id);
                    if (device != null)
                        device.IsSelected = false;
                    if (transformer != null)
                        transformer.IsSelected = false;
                }
            }

            Default.undoActions.Push(() => UndoDeleteCommand(root)); 
            Default.HasChangedAction();
        }

        private void UndoDeleteCommand(XElement root)
        {
            undoPaste.Clear();
            foreach (SelectableDesignerItemViewModelBase itemVMBase in DiagramViewModel.SelectedItems)
                itemVMBase.IsSelected = false;

            IEnumerable<XElement> groupsXML = root.Elements("Copied").Elements("Group");

            foreach (XElement groupXML in groupsXML)
            {
                Guid ID = new(groupXML.Element("ID").Value);
                GroupingDesignerItemViewModel itemVM = (GroupingDesignerItemViewModel)DeserializeDesignerItems(groupXML, ID, 0, 0);
                AddXMLDataToViewModel(new List<XElement> { groupXML }, undoDelete: true, groupVM: itemVM);
                DiagramViewModel.AddItemCommand.Execute(itemVM);
                undoPaste.Add(itemVM.Id);
            }

            AddXMLDataToViewModel(root.Elements("Copied"), undoDelete: true, diagramVM: DiagramViewModel);

            XElement curXElement = new(root);

            List<Guid> ahuhu = new(undoPaste.Count());
            undoPaste.ForEach(x => ahuhu.Add(x));
            Default.redoActions.Push(() => RedoDeleteCommand(ahuhu, curXElement));
            Default.HasChangedAction();
        }

        private void RedoDeleteCommand(List<Guid> idDelete, XElement root)
        {
            foreach (Guid id in idDelete)
                this.DiagramViewModel.Items.Remove(this.DiagramViewModel.Items.Where(x => x.Id.Equals(id)).Single());
            Default.undoActions.Push(() => UndoDeleteCommand(root));
            Default.HasChangedAction();
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private void AddXMLDataToViewModel(IEnumerable<XElement> root, Dictionary<Guid, Guid> mappingOldToNewIDs = null, double offsetX = 0, 
            double offsetY = 0, bool undoDelete = false, List<Guid> redoPaste = null, GroupingDesignerItemViewModel groupVM = null, DiagramViewModel diagramVM = null)
        {
            IEnumerable<XElement> itemsXML = root.Elements("Device");
            IEnumerable<XElement> connectorsXML = root.Elements("Connector");

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                if (redoPaste != null)
                    newID = redoPaste[cntPaste++];
                if (undoDelete)
                {
                    newID = oldID;
                    int id = Default.IdInt(oldID);
                    Device device = BaseSiteServices.FindBaseSiteByID<Device>(id);
                    Transformer transformer = BaseSiteServices.FindBaseSiteByID<Transformer>(id);
                    if (device != null)
                        device.IsSelected = true;
                    if (transformer != null)
                        transformer.IsSelected = true;
                }
                else
                    mappingOldToNewIDs.Add(oldID, newID);
                DesignerItemViewModelBase itemVM = DeserializeDesignerItems(itemXML, newID, offsetX, offsetY);
                if (groupVM == null)
                {
                    diagramVM.AddItemCommand.Execute(itemVM);
                    undoPaste.Add(itemVM.Id);
                }
                else
                    groupVM.AddItemCommand.Execute(itemVM);
            }

            IDiagramViewModel viewModel = groupVM;
            viewModel ??= diagramVM;

            foreach (XElement connectorXML in connectorsXML)
            {
                ConnectorViewModel itemVM = redoPaste switch
                {
                    null => DeserializeConnector(connectorXML, mappingOldToNewIDs, viewModel, undoDelete),
                    _ => DeserializeConnector(connectorXML, mappingOldToNewIDs, viewModel, undoDelete, redoPaste[cntPaste++])
                };
                if (itemVM != null)
                    if (groupVM == null)
                    {
                        diagramVM.AddItemCommand.Execute(itemVM);
                        undoPaste.Add(itemVM.Id);
                    }
                    else
                    {
                        groupVM.AddItemCommand.Execute(itemVM);
                        undoPaste.Add(itemVM.Id);
                    }
            }
        }

        #endregion


    }
}
