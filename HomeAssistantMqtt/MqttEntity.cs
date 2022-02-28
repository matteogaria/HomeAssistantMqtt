using System;

using NLog;
using Newtonsoft.Json;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public class MqttEntity
    {
        protected static readonly Logger log = LogManager.GetCurrentClassLogger();
        public string Route { get; }
        public MqttEntityMapping Mapping { get; }
        public MqttManager Mqtt { get; }
        public MqttEntity(MqttEntityMapping mapping, MqttManager mqtt)
        {
            Route = mapping.Route;
            Mapping = mapping;
            Mqtt = mqtt;
        }
    }
    public class MqttEntity<T> : MqttEntity where T : class
    {
        public IDevice<T> Device { get; }

        public MqttEntity(IDevice<T> device, MqttEntityMapping mapping, MqttManager mqtt) : base(mapping, mqtt)
        {
            Device = device;

            device.StatusNotification = (status) =>
                Mqtt.PublishMessage($"{Route}/state", JsonConvert.SerializeObject(status));

            Mqtt.RegisterRoute(Route + "/set", (message) =>
            {
                try
                {
                    T cmd = JsonConvert.DeserializeObject<T>(message);
                    device.CommandNotification(cmd);
                }
                catch (Exception ex)
                {
                    log.Warn(ex, $"{Route}/set: Invalid payload received");
                }
            });

            if (mapping.UpdateOnStart)
            {
                T status = device.GetStatus();
                Mqtt.PublishMessage($"{Route}/state", JsonConvert.SerializeObject(status));
            }

            if (mapping.Publish)
            {
                DiscoveryMessage message = device.GetDiscoveryMessage();
                message.Name = Mapping.Name;
                message.UniqueId = Mapping.DeviceId;
                message.CommandTopic = $"{Route}/set";
                message.StateTopic = $"{Route}/state";
                message.Schema = "json";
                mqtt.PublishDeviceDiscovery(mapping.ComponentClass, message);
            }
        }
    }
}
