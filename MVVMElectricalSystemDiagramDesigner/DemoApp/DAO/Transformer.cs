using System;
using System.Collections.Generic;

namespace DemoApp
{
    public class Transformer : BaseSite
    {

        public int NumberOfWinding { get; set; }
        public string Label { get; set; }
        public string Path => Substation.Name + '/' + Name;
        public bool IsSelected { get; set; }

        public Substation Substation { get; set; }

        public HashSet<Fan> Fans { get; set; }

        public Transformer()
        {
            NumberOfWinding = 0;
            Substation = new Substation();
            Fans = new();
            IsSelected = false;
        }

        public Transformer(int iD, string name, string description, int numberOfWinding, Substation substation, bool isSelected, HashSet<Fan> fans) : base(iD, name, description)
        {
            NumberOfWinding = numberOfWinding;
            Substation = substation ?? throw new(nameof(substation));
            IsSelected = isSelected;
            Fans = fans ?? throw new(nameof(fans));
        }
    }
}
