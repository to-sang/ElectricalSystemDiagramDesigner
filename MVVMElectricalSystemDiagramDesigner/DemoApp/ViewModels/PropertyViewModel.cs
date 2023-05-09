using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DemoApp
{
    public class PropertyViewModel : INPCBase
    {
        #region Declaration

        #region Designer Item

        public string DeviceType { get; private set; }

        public string DeviceName { get; private set; }

        public string DevicePath { get; private set; }

        public string Description { get; private set; }

        public double Left
        {
            get 
            {
                if (DesignerItemViewModel == null)
                    return 0;
                return DesignerItemViewModel.Left; 
            }
            set { DesignerItemViewModel.Left = value; NotifyChanged("Left"); }
        }

        public double Top
        {
            get
            {
                if (DesignerItemViewModel == null)
                    return 0;
                return DesignerItemViewModel.Top;
            }
            set { DesignerItemViewModel.Top = value; NotifyChanged("Top"); }
        }

        public double Width
        {
            get 
            {
                if (DesignerItemViewModel == null)
                    return 0;
                return DesignerItemViewModel.ItemWidth; 
            }
            set { DesignerItemViewModel.ItemWidth = value; NotifyChanged("Width"); }
        }

        public double Height
        {
            get
            {
                if (DesignerItemViewModel == null)
                    return 0;
                return DesignerItemViewModel.ItemHeight;
            }
            set { DesignerItemViewModel.ItemHeight = value; NotifyChanged("Height"); }
        }

        public int Rotation
        {
            get
            {
                if (DesignerItemViewModel == null)
                    return 0;
                return DesignerItemViewModel.Angle;
            }
            set { DesignerItemViewModel.Angle = value; NotifyChanged("Rotation"); NotifyChanged("Height"); NotifyChanged("Width"); }
        }

        private int labelSize;

        public int LabelSize
        {
            get { return labelSize; }
            set { labelSize = value; }
        }

        private string label;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public ConnectorOrientation LabelPosition { get; private set; }

        #endregion

        #region Connection

        public string SourcePath { get; private set; }

        public string SourceOrientation { get; private set; }

        public string SinkPath { get; private set; }

        public string SinkOrientation { get; private set; }

        #endregion

        #region Many Select

        public int Number { get; private set; }

        public int NumberOfType { get; private set; }

        #endregion

        public string MappingContent { get; set; }

        public Brush MappingColor { get; set; }

        public int PropertyType { get; set; }

        public static SimpleCommand MappingCommand { get; private set; }

        public List<SelectableDesignerItemViewModelBase> SelectedItems { get; set; }
        public DesignerItemViewModelBase DesignerItemViewModel { get; set; }
        public ConnectorViewModel ConnectorViewModel { get; set; }
        public bool IsImported => BaseSiteServices.IsImported;

        #endregion

        #region Constructor
        public PropertyViewModel(List<SelectableDesignerItemViewModelBase> SelectedItems)
        {
            this.SelectedItems = SelectedItems;
            MappingCommand = new(ExecuteMappingCommand);
            switch (SelectedItems.Count)
            {
                case 0:
                    PropertyType = 0;
                    break;
                case 1:
                    if (SelectedItems[0] is DesignerItemViewModelBase designerItemViewModel)
                    {
                        DesignerItemViewModel = designerItemViewModel;
                        PropertyType = 1;
                        DeviceType = GetDeviceNameFromType(DesignerItemViewModel);
                        if (!DeviceType.Equals("Transformer") && !DeviceType.Equals("Fan"))
                        {
                            Device device = BaseSiteServices.FindBaseSiteByID<Device>(Default.IdInt(DesignerItemViewModel.Id));
                            if (device != null)
                                if (!device.Type.Equals(DeviceType))
                                    device = null;
                            DeviceName = device?.Name;
                            DevicePath = device?.Path;
                            Description = device?.Description;
                            Label = device?.Label;
                            Rotation = DesignerItemViewModel.Angle;
                            LabelSize = 0;
                            LabelPosition = ConnectorOrientation.None;
                            if (device == null)
                            {
                                MappingContent = "Mapping";
                                MappingColor = new SolidColorBrush(Colors.Aqua);
                            }
                            else
                            {
                                MappingContent = "Unmap";
                                MappingColor = new SolidColorBrush(Colors.Red);
                            }
                        }
                        else if (DeviceType.Equals("Transformer"))
                        {
                            Transformer transformer = BaseSiteServices.FindBaseSiteByID<Transformer>(Default.IdInt(DesignerItemViewModel.Id));
                            DeviceName = transformer?.Name;
                            DevicePath = transformer?.Path;
                            Description = transformer?.Description;
                            Label = transformer?.Label;
                            Rotation = DesignerItemViewModel.Angle;
                            LabelSize = 0;
                            LabelPosition = ConnectorOrientation.None;
                            if (transformer == null)
                            {
                                MappingContent = "Mapping";
                                MappingColor = new SolidColorBrush(Colors.Aqua);
                            }
                            else
                            {
                                MappingContent = "Unmap";
                                MappingColor = new SolidColorBrush(Colors.Red);
                            }
                        }
                        else if (DeviceType.Equals("Fan"))
                        {
                            Fan fan = BaseSiteServices.FindBaseSiteByID<Fan>(Default.IdInt(DesignerItemViewModel.Id));
                            DeviceName = fan?.Name;
                            DevicePath = fan?.Path;
                            Description = fan?.Description;
                            Label = fan?.Label;
                            Rotation = DesignerItemViewModel.Angle;
                            LabelSize = 0;
                            LabelPosition = ConnectorOrientation.None;
                            if (fan == null)
                            {
                                MappingContent = "Mapping";
                                MappingColor = new SolidColorBrush(Colors.Aqua);
                            }
                            else
                            {
                                MappingContent = "Unmap";
                                MappingColor = new SolidColorBrush(Colors.Red);
                            }
                        }    
                        
                    }
                    if (SelectedItems[0] is ConnectorViewModel connectorViewModel)
                    {
                        ConnectorViewModel = connectorViewModel;
                        PropertyType = 2;
                        SourcePath = GetDeviceNameFromType(ConnectorViewModel.SourceConnectorInfo.DataItem);
                        SourceOrientation = ConnectorViewModel.SourceConnectorInfo.Orientation.ToString();
                        SinkPath = GetDeviceNameFromType(((FullyCreatedConnectorInfo)ConnectorViewModel.SinkConnectorInfo).DataItem);
                        SinkOrientation = ((FullyCreatedConnectorInfo)ConnectorViewModel.SinkConnectorInfo).Orientation.ToString();
                    }
                    break;
                default:
                    PropertyType = 3;
                    Number = SelectedItems.Count;
                    NumberOfType = SelectedItems.Select(x => x.GetType()).Distinct().Count();
                    break;
            }
        }

        #endregion

        #region Method

        private string GetDeviceNameFromType(DesignerItemViewModelBase type) => type switch
        {
            BusbarDesignerItemViewModel => "BusBar",
            DisconnectorSwitchDesignerItemViewModel => "Disconnector Switch",
            CircuitBreakerDesignerItemViewModel => "Circuit Breaker",
            EarthSwitchDesignerItemViewModel => "Earth Switch",
            GroundDesignerItemViewModel => "Ground",
            SourceDesignerItemViewModel => "Source",
            TransformerDesignerItemViewModel => "Transformer",
            GroupingDesignerItemViewModel => "Group",
            GeneratorDesignerItemViewModel => "Generator",
            FanDesignerItemViewModel => "Fan",
            TripLockoutReplayDesignerItemViewModel => "Trip Lockout Replay",
            LineIndicatorDesignerItemViewModel => "Line Indicator",
            _ => string.Empty
        };

        private void ExecuteMappingCommand(object parameter)
        {
            int id = Default.IdInt(DesignerItemViewModel.Id);
            if (id == -1)
            {
                PopupViewModel popupVM = new(DeviceType);
                PopupWindow popup = new()
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = popupVM
                };
                if (popup.ShowDialog() == true)
                {
                    Map(popupVM.SelectedDevice, false);
                    Default.redoActions.Clear();
                }
            }
            else
            {
                UnMap(Guid.NewGuid(), false);
                Default.redoActions.Clear();
            }
            Default.HasChangedAction();
        }

        private void UnMap(Guid preID, bool isUndo)
        {
            int id = Default.IdInt(DesignerItemViewModel.Id);
            Device device = BaseSiteServices.FindBaseSiteByID<Device>(id);
            Transformer transformer = BaseSiteServices.FindBaseSiteByID<Transformer>(id);
            Fan fan = BaseSiteServices.FindBaseSiteByID<Fan>(id);
            BaseSite baseSite = device;
            baseSite ??= transformer;
            if (isUndo)
                Default.redoActions.Push(() => Map(baseSite, false));
            else
                Default.undoActions.Push(() => Map(baseSite, true));
            if (device != null)
                device.IsSelected = false;
            if (transformer != null)
                transformer.IsSelected = false;
            if (fan != null)
                fan.IsSelected = false;
            DesignerItemViewModel.Id = preID;
            if (DesignerItemViewModel is TransformerDesignerItemViewModel transformerVM)
            {
                TransformedBitmap TempImage = new();

                TempImage.BeginInit();
                TempImage.Source = new BitmapImage(new("pack://application:,,,/DemoApp;component/Images/Devices/Transformer3.png", UriKind.RelativeOrAbsolute));

                RotateTransform transform = new(transformerVM.Angle);
                TempImage.Transform = transform;
                TempImage.EndInit();

                transformerVM.ImageSource = TempImage;
            }
            Default.HasChangedAction();
        }

        private void Map(BaseSite selectedDevice, bool isUndo)
        {
            Guid preID = DesignerItemViewModel.Id;
            if (isUndo)
                Default.redoActions.Push(() => UnMap(preID, false));
            else
                Default.undoActions.Push(() => UnMap(preID, true));
            DesignerItemViewModel.Id = Default.String2Guid(selectedDevice.ID.ToString());
            if (selectedDevice is Device device)
                device.IsSelected = true;
            if (selectedDevice is Fan fan)
                fan.IsSelected = true;
            if (selectedDevice is Transformer transformer)
            {
                transformer.IsSelected = true;
                if (DesignerItemViewModel is TransformerDesignerItemViewModel transformerVM)
                {
                    if (transformer.NumberOfWinding == 2)
                    {
                        TransformedBitmap TempImage = new();

                        TempImage.BeginInit();
                        TempImage.Source = new BitmapImage(new("pack://application:,,,/DemoApp;component/Images/Devices/Transformer2.png", UriKind.RelativeOrAbsolute));

                        RotateTransform transform = new(transformerVM.Angle);
                        TempImage.Transform = transform;
                        TempImage.EndInit();

                        transformerVM.ImageSource = TempImage;
                    }
                    if (transformer.NumberOfWinding == 3)
                    {
                        TransformedBitmap TempImage = new();

                        TempImage.BeginInit();
                        TempImage.Source = new BitmapImage(new("pack://application:,,,/DemoApp;component/Images/Devices/Transformer3.png", UriKind.RelativeOrAbsolute));

                        RotateTransform transform = new(transformerVM.Angle);
                        TempImage.Transform = transform;
                        TempImage.EndInit();

                        transformerVM.ImageSource = TempImage;
                    }
                }
            }
            Default.HasChangedAction();
        }

        #endregion


    }
}
