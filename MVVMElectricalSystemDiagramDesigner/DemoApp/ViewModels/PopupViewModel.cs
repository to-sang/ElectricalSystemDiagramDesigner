using DiagramDesigner;
using System.Collections.ObjectModel;
using System.Linq;

namespace DemoApp
{
    public class PopupViewModel : INPCBase
    {
        public ObservableCollection<Substation> Substations => new(BaseSiteServices.FindAllBaseSitesType<Substation>());

        public ObservableCollection<VoltageLevel> VoltageLevels => _SelectedSubstation == null ? new() : new(_SelectedSubstation.VoltageLevels);

        public ObservableCollection<Bay> Bays => _SelectedVoltageLevel == null ? new() : new(_SelectedVoltageLevel.Bays);

        public ObservableCollection<BaseSite> Devices => DeviceType.Equals("Transformer") ? _SelectedSubstation == null ? new() 
            : new(_SelectedSubstation.Transformers.Where(item => !item.IsSelected)) 
            : DeviceType.Equals("Fan") ? _SelectedSubstation == null ? new() 
            : new(BaseSiteServices.FindAllBaseSitesType<Fan>().Where(item => _SelectedSubstation.Transformers.Contains(item.Transformer) && !item.IsSelected)) 
            : _SelectedBay == null ? new() 
            : new(_SelectedBay.Devices.Where(item => item.Type.Equals(DeviceType) && !item.IsSelected));

        public bool SubstationEnable => Substations.Count > 0;
        public bool VoltageLevelEnable => SelectedSubstation != null;
        public bool BayEnable => SelectedVoltageLevel != null;
        public bool DeviceEnable => (SelectedBay != null) || ((DeviceType.Equals("Transformer") || DeviceType.Equals("Fan")) && SelectedSubstation != null);

        public string DeviceType { get; set; }

        private Substation _SelectedSubstation;

        public Substation SelectedSubstation
        {
            get => _SelectedSubstation;
            set { _SelectedSubstation = value; NotifyChanged("SelectedSubstation", "VoltageLevelEnable", "VoltageLevels", "DeviceEnable", "Devices"); }
        }

        private VoltageLevel _SelectedVoltageLevel;

        public VoltageLevel SelectedVoltageLevel
        {
            get => _SelectedVoltageLevel;
            set { _SelectedVoltageLevel = value; NotifyChanged("SelectedVoltageLevel", "BayEnable" ,"Bays"); }
        }

        private Bay _SelectedBay;

        public Bay SelectedBay
        {
            get => _SelectedBay;
            set { _SelectedBay = value; NotifyChanged("SelectedBay", "DeviceEnable", "Devices"); }
        }

        private BaseSite _SelectedDevice;

        public BaseSite SelectedDevice
        {
            get => _SelectedDevice;
            set { _SelectedDevice = value; NotifyChanged("SelectedDevice"); }
        }


        public PopupViewModel(string deviceType)
        {
            DeviceType = deviceType;
        }
    }
}
