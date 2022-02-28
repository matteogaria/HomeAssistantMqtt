using Newtonsoft.Json;

namespace HomeAssistantMqtt.Models
{
    public class MqttDevice
    {
        [JsonProperty("identifiers")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sw_version")]
        public string SoftwareVersion { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("manufactorer")]
        public string Manufactorer { get; set; }
    }
}
