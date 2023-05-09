using System;
using System.Collections.Generic;

namespace DemoApp
{
    public class VoltageLevel : BaseSite
    {

        public Substation Substation { get; set; }

        public HashSet<Bay> Bays { get; set; }

        public VoltageLevel()
        {
            Bays = new HashSet<Bay>();
            Substation= new Substation();
        }

        public VoltageLevel(int iD, string name, string description, Substation substation, HashSet<Bay> bays) : base(iD, name, description)
        {
            Substation = substation ?? throw new(nameof(substation));
            Bays = bays ?? throw new(nameof(bays));
        }
    }
}
