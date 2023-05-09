using System;
using System.Collections.Generic;

namespace DemoApp
{
    public class Substation : BaseSite
    {

		public HashSet<VoltageLevel> VoltageLevels { get; set; }
		public HashSet<Transformer> Transformers { get; set; }

        public Substation() : base()
        {
            VoltageLevels = new HashSet<VoltageLevel>();
            Transformers = new HashSet<Transformer>();
        }

        public Substation(int iD, string name, string description, HashSet<VoltageLevel> voltageLevels, HashSet<Transformer> transformers) : base(iD, name, description)
        {
            VoltageLevels = voltageLevels ?? throw new(nameof(voltageLevels));
            Transformers = transformers ?? throw new(nameof(transformers));
        }
    }
}
