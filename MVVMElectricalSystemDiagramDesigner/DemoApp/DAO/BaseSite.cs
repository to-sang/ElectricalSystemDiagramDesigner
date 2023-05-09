using System;

namespace DemoApp
{
    public class BaseSite
    {
        private int? _id;

        public int? ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public BaseSite()
        {
            ID = 0;
            Name = string.Empty;
            Description = string.Empty;
        }

        public BaseSite(int iD, string name, string description)
        {
            ID = iD;
            Name = name ?? throw new(nameof(name));
            Description = description ?? throw new(nameof(description));
        }
    }
}
