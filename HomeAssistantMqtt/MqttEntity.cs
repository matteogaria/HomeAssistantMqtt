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
        public string[] ExtraRoutes { get; }
        public MqttEntityDescriptor Mapping { get; }
        public MqttDevice Mqtt { get; }
        public MqttEntity(MqttEntityDescriptor mapping, MqttDevice mqtt, string[] extraRoutes = null)
        {
            Route = mapping.Route;
            ExtraRoutes = extraRoutes;
            Mapping = mapping;
            Mqtt = mqtt;
        }
    }
    public class MqttEntity<T> : MqttEntity where T : class
    {
        public IDevice<T> Device { get; }

        public MqttEntity(IDevice<T> device, MqttEntityDescriptor mapping, MqttDevice mqtt, string[] extraRoutes = null) : base(mapping, mqtt, extraRoutes)
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

            if(extraRoutes != null)
                foreach(string route in extraRoutes)
                    Mqtt.RegisterRoute($"{Route}/{route}", (msg) => device.MessageNotification(route, msg));

            if (mapping.UpdateOnStart)
            {
                T status = device.GetStatus();
                Mqtt.PublishMessage($"{Route}/state", JsonConvert.SerializeObject(status));
            }

            if (mapping.Publish)
            {
                DiscoveryMessage message = device.GetDiscoveryMessage();
                message.Name = Mapping.Name;
                message.UniqueId = Mapping.EntityId;
                message.CommandTopic = $"{Route}/set";
                message.StateTopic = $"{Route}/state";
                message.Schema = "json";
                mqtt.PublishDeviceDiscovery(mapping.ComponentClass, message);
            }
        }
    }
}
