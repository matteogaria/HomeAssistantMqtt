
namespace HomeAssistantMqtt.Models
{
    public class MqttDeviceMapping
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string ComponentClass { get; set; }
        public bool Publish { get; set; }
        public bool UpdateOnStart { get; set; }
    }
}
