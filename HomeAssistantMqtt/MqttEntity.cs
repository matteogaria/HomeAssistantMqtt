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
        public IEntity Entity { get; }
        public MqttEntityDescriptor Mapping { get; }
        public MqttDevice Mqtt { get; }
        public MqttEntity(IEntity entity, MqttEntityDescriptor mapping, MqttDevice mqtt, string[] extraRoutes = null)
        {
            Route = mapping.Route;
            ExtraRoutes = extraRoutes;
            Entity = entity;
            Mapping = mapping;
            Mqtt = mqtt;

            entity.StatusNotification = (status) =>
                        Mqtt.PublishMessage($"{Route}/state", JsonConvert.SerializeObject(status));

            if (extraRoutes != null)
                foreach (string route in extraRoutes)
                    Mqtt.RegisterRoute($"{Route}/{route}", (msg) => entity.MessageNotification(route, msg));

            if (mapping.UpdateOnStart)
            {
                object status = entity.GetStatus();
                Mqtt.PublishMessage($"{Route}/state", JsonConvert.SerializeObject(status));
            }

            if (mapping.Publish)
            {
                DiscoveryMessage message = entity.GetDiscoveryMessage();
                message.Name = Mapping.Name;
                message.UniqueId = Mapping.EntityId;
                message.CommandTopic = $"{Route}/set";
                message.StateTopic = $"{Route}/state";
                message.Schema = "json";
                mqtt.PublishDeviceDiscovery(mapping.ComponentClass, message);
            }
        }
    }
    public class MqttEntity<T> : MqttEntity where T : class
    {
        public MqttEntity(IJsonEntity<T> entity, MqttEntityDescriptor mapping, MqttDevice mqtt, string[] extraRoutes = null) : base(entity, mapping, mqtt, extraRoutes)
        {
            Mqtt.RegisterRoute(Route + "/set", (message) =>
            {
                try
                {
                    T cmd = JsonConvert.DeserializeObject<T>(message);
                    entity.CommandNotification(cmd);
                }
                catch (Exception ex)
                {
                    log.Warn(ex, $"{Route}/set: Invalid payload received");
                }
            });      
        }
    }
}
