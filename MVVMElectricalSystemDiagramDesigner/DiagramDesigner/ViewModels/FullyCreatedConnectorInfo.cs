using System;

namespace DiagramDesigner
{
    public class FullyCreatedConnectorInfo : ConnectorInfoBase
    {
        private bool showConnectors = false;

        public FullyCreatedConnectorInfo(DesignerItemViewModelBase dataItem, ConnectorOrientation orientation)
            : base(orientation)
        {
            this.DataItem = dataItem;
        }

        public FullyCreatedConnectorInfo() { }
        public DesignerItemViewModelBase DataItem { get; set; }
        public Guid ItemId { get { return DataItem.Id; } set { } }
        public bool ShowConnectors
        {
            get
            {
                return showConnectors;
            }
            set
            {
                if (showConnectors != value)
                {
                    showConnectors = value;
                    NotifyChanged("ShowConnectors");
                }
            }
        }
    }
}
