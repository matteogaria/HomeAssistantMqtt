using System;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public interface IEntity
    {
        // this method is called when a route defined into the extra routes is called
        // message interpretation is left at your implementation
        void MessageNotification(string topic, string msg);
        // this must return the current status of the entity, used at startup to  
        // initialize MQTT values
        object GetStatus();
        // an handler is attached to this by the MqttEntity. Call this method
        // from your code to notify a status change    
        Action<object> StatusNotification { get; set; }
        // create your discovery message here, it'll be used at startup to publish
        // the entity for discovery.
        DiscoveryMessage GetDiscoveryMessage();
    }

    public interface IJsonEntity<TMSG> : IEntity where TMSG : class
    {
        // this method will be invoked by the MqttEntity to send commands.
        // Implement command management into this method or call your 
        // commands from here
        void CommandNotification(TMSG command);
    }
}
