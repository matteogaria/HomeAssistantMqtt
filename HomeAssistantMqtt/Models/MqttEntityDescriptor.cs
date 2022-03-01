
namespace HomeAssistantMqtt.Models
{
    public class MqttEntityDescriptor
    {
        public string EntityId { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string ComponentClass { get; set; }
        public bool Publish { get; set; }
        public bool UpdateOnStart { get; set; }
    }
}
