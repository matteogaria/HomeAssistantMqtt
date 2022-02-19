using Newtonsoft.Json;

namespace HomeAssistantMqtt.Models
{
    public class DiscoveryMessage
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("unique_id")]
        public string? UniqueId { get; set; }

        [JsonProperty("schema")]
        public string? Schema { get; set; }

        [JsonProperty("cmd_t")]
        public string? CommandTopic { get; set; }

        [JsonProperty("stat_t")]
        public string? StateTopic { get; set; }
    }

    public class DimmerLatchDiscoveryMessage : DiscoveryMessage
    {
        [JsonProperty("brightness")]
        public bool? Brightness { get; set; }
    }

    public class SensorDiscoveryMessage : DiscoveryMessage
    {
        // unit_of_measurement": "°C", "value_template": "{{ value_json.temperature
        [JsonProperty("unit_of_measurement")]
        public string MeasurementUnit { get; set; }
        [JsonProperty("value_template")]
        public string? ValueTemplate { get; set; }
        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }
    }
}
