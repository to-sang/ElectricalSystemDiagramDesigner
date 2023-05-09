using System.Collections.Generic;
using DiagramDesigner.Helpers;

namespace DemoApp
{
    public class ToolBoxViewModel
    {
        public ToolBoxViewModel()
        {
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Source_Toolbox.png", typeof(SourceDesignerItemViewModel), "Source"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Ground_Toolbox.png", typeof(GroundDesignerItemViewModel), "Ground"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Busbar_Toolbox.png", typeof(BusbarDesignerItemViewModel), "Busbar"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Transformer3.png", typeof(TransformerDesignerItemViewModel), "Transformer"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/DisconnectorSwitch.png", typeof(DisconnectorSwitchDesignerItemViewModel), "Disconnector Switch"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/EarthSwitch.png", typeof(EarthSwitchDesignerItemViewModel), "Earth Switch"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/CircuitBreaker.png", typeof(CircuitBreakerDesignerItemViewModel), "Circuit Breaker"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Fan.png", typeof(FanDesignerItemViewModel), "Fan"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/Generator.png", typeof(GeneratorDesignerItemViewModel), "Generator"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/LineIndicator.png", typeof(LineIndicatorDesignerItemViewModel), "Line Indicator"));
            ToolBoxItems.Add(new("pack://application:,,,/DemoApp;component/Images/Devices/TripLockoutReplay_Toolbox.png", typeof(TripLockoutReplayDesignerItemViewModel), "Trip Lockout Replay"));
        }

        public List<ToolBoxData> ToolBoxItems { get; } = new();
    }
}
