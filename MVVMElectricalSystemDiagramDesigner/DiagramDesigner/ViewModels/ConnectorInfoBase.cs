namespace DiagramDesigner
{
    public abstract class ConnectorInfoBase : INPCBase
    {
        private double connectorWidth = 8;
        private double connectorHeight = 8;

        public ConnectorInfoBase(ConnectorOrientation orientation)
        {
            this.Orientation = orientation;
        }

        protected ConnectorInfoBase()
        {
        }

        public ConnectorOrientation Orientation { get; set; }

        public double ConnectorWidth
        {
            get { return connectorWidth; }
            set
            {
                connectorWidth = value;
                NotifyChanged("ConnectorWidth");
            }
        }

        public double ConnectorHeight
        {
            get { return connectorHeight; }
            set { connectorHeight = value; NotifyChanged("ConnectorHeight"); }
        }
    }
}
