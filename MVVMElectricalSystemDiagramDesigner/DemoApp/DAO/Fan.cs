using System.Collections.Generic;
using System;

namespace DemoApp
{
    public class Fan : BaseSite
    {
        public Transformer Transformer { get; set; }
        public string Label { get; set; }
        public string Path => Transformer.Substation.Name + '/' + Transformer.Name + '/' + Name;
        public bool IsSelected { get; set; }
        public Fan()
        {
            Transformer = new Transformer();
        }

        public Fan(int iD, string name, string description, string label, bool isSelected, Transformer transformer) : base(iD, name, description)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
            Label = label ?? throw new ArgumentNullException(nameof(label));
            IsSelected = isSelected;
        }
    }
}
