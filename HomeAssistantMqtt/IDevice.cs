using System;

using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt
{
    public interface IDevice<TMSG> where TMSG : class
    {
        void CommandNotification(TMSG command);
        TMSG GetStatus();
        Action<TMSG> StatusNotification { get; set; }
        DiscoveryMessage GetDiscoveryMessage();
    }
}
