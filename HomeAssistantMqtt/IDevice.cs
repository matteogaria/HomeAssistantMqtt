using System;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public interface IDevice<TMSG> where TMSG : class
    {
        void CommandNotification(TMSG command);
        void MessageNotification(string topic, string msg);
        TMSG GetStatus();
        Action<TMSG> StatusNotification { get; set; }
        DiscoveryMessage GetDiscoveryMessage();
    }
}
