using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using NLog;
using Newtonsoft.Json;

using MQTTnet.Extensions.ManagedClient;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public class MqttManager
    {
        protected static readonly Logger log = LogManager.GetCurrentClassLogger();

        public MqttManager(ManagedMqttClient mqttClient)
        {
            MqttClient = mqttClient;

            MqttClient.ApplicationMessageReceivedAsync += async (e) =>
            {
                await Task.Run(() =>
                {
                    log.Debug($"MQTT message on: {e.ApplicationMessage.Topic} | payload: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");

                    if (Routes.TryGetValue(e.ApplicationMessage.Topic, out Action<string> cb))
                        cb?.Invoke(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                });
            };
        }

        protected Dictionary<string, Action<string>> Routes { get; } = new Dictionary<string, Action<string>>();
        public ManagedMqttClient MqttClient { get; }

        public bool RegisterRoute(string route, Action<string> callback)
        {
            if (Routes.ContainsKey(route))
                return false;

            MqttClient.SubscribeAsync(route);

            Routes.Add(route, callback);
            return true;
        }

        public void PublishMessage(string topic, string payload)
            => Task.Run(() => MqttClient.PublishAsync(topic, payload));

        public void PublishDeviceDiscovery(string componentClass, DiscoveryMessage msg)
        {
            string json = JsonConvert.SerializeObject(msg, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            MqttClient.PublishAsync($"homeassistant/{componentClass}/{msg.UniqueId}/config", json, retain: true);
        }
    }
}
