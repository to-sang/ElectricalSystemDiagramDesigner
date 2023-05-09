using System;

namespace DemoApp
{
    public class Device : BaseSite
    {

        public string Type { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }

        public Bay Bay { get; set; }

        public Device() : base()
        {
            Type = string.Empty;
            Label = string.Empty;
            IsSelected = false;
            Bay = new Bay();
        }

        public Device(int iD, string name, string description, string type, string label, bool isSelected, Bay bay) : base(iD, name, description)
        {
            Type = type ?? throw new(nameof(type));
            Label = label ?? throw new(nameof(label));
            IsSelected = isSelected;
            Bay = bay ?? throw new(nameof(bay));
        }

        public string Path => Bay.VoltageLevel.Substation.Name + '/' + Bay.VoltageLevel.Name + '/' + Bay.Name + '/' + Name;
    }
}
