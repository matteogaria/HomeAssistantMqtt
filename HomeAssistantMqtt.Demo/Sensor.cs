using HomeAssistantMqtt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAssistantMqtt.Demo
{
    public class Sensor : IEntity
    {
        private Random rnd = new Random();
        public Action<object> StatusNotification { get; set; }

        public Sensor()
        {
            Thread t = new Thread(() =>
            {
                while (true) // NEVER use this in production code
                {
                    Thread.Sleep(30000);
                    StatusNotification?.Invoke(GetStatus());
                }
            });
            t.Start();
        }

        public DiscoveryMessage GetDiscoveryMessage()
        {
            return new SensorDiscoveryMessage
            {
                MeasurementUnit = "°C",
                DeviceClass = "temperature",
                StateClass = "measurement"
            };
        }

        public object GetStatus()
        {
            return rnd.Next(15, 25);
        }

        public void MessageNotification(string topic, string msg)
        {
        }
    }
}
