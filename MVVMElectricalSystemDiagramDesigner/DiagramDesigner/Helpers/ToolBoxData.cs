using System;

namespace DiagramDesigner.Helpers
{
    public class ToolBoxData
    {
        public string ImageUrl { get; private set; }
        public Type Type { get; private set; }
        public string ToolTip { get; set; }

        public ToolBoxData(string imageUrl, Type type, string toolTip)
        {
            this.ImageUrl = imageUrl;
            this.Type = type;
            this.ToolTip = toolTip;
        }
    }
}
