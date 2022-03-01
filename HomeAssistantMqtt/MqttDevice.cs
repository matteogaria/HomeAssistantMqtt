using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using NLog;
using Newtonsoft.Json;

using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client;
using MQTTnet;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public class MqttDevice
    {
        protected static readonly Logger log = LogManager.GetCurrentClassLogger();

        public MqttDevice(ManagedMqttClient mqttClient, MqttDeviceDescriptor device)
        {
            MqttClient = mqttClient;
            Device = device;
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
        public MqttDeviceDescriptor Device { get; }

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
            msg.Device = Device;
            string json = JsonConvert.SerializeObject(msg, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            MqttClient.PublishAsync($"homeassistant/{componentClass}/{Device.Id}/{msg.UniqueId}/config", json, retain: true).Wait();
        }

        public static ManagedMqttClient CreateMqttClient(MqttBrokerConfiguration cfg)
        {
            var options = new ManagedMqttClientOptionsBuilder()
              .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
              .WithClientOptions(new MqttClientOptionsBuilder()
                  .WithClientId(cfg.ClientId)
                  .WithTcpServer(cfg.IpAddress)
                  .WithCredentials(cfg.Username, cfg.Password)
                  .Build())
              .Build();

            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            mqttClient.StartAsync(options).Wait();
            return mqttClient;
        }
    }
}
