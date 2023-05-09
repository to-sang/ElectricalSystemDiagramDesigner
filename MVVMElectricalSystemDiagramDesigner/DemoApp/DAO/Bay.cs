using System;
using System.Collections.Generic;

namespace DemoApp
{
    public class Bay : BaseSite
    {
        
        public VoltageLevel VoltageLevel { get; set; }

        public HashSet<Device> Devices { get; set; }

        public Bay() : base()
        {
            Devices = new HashSet<Device>();
            VoltageLevel= new VoltageLevel();
        }

        public Bay(int iD, string name, string description, VoltageLevel voltageLevel, HashSet<Device> devices) : base(iD, name, description)
        {
            VoltageLevel = voltageLevel ?? throw new(nameof(voltageLevel));
            Devices = devices ?? throw new(nameof(devices));
        }


    }
}
