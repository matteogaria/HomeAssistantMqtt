using System;

using Newtonsoft.Json;
using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt.Demo
{
    public class LightMessage
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("brightness")]
        public int? Brightness { get; set; }

        [JsonIgnore]
        public bool On { get => State.ToLower() == "on"; set => State = value ? "ON" : "OFF"; }
    }

    public class Light : IJsonEntity<LightMessage>
    {
        public Action<object> StatusNotification { get; set; }

        public void CommandNotification(LightMessage command)
        {
            Console.WriteLine($"SampleLight: received command: {command?.State} brightness: {command?.Brightness}");
            StatusNotification?.Invoke(command);
        }

        // nothing to customize here
        public DiscoveryMessage GetDiscoveryMessage()
            => new BrightnessLightDiscoveryMessage { Brightness = true };

        public object GetStatus()
            => new LightMessage() { On = false, Brightness = 0 };

        public void MessageNotification(string topic, string msg)
             => Console.WriteLine($"Received from {topic} msg: {msg}");
    }
}
