using System;

using MQTTnet.Extensions.ManagedClient;
using HomeAssistantMqtt.Models;

namespace HomeAssistantMqtt.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Sample broker configuration
            MqttBrokerConfiguration broker = new MqttBrokerConfiguration
            {
                ClientId = "sampleClient",
                IpAddress = "192.168.1.100",
                Username = "sampleUser",
                Password = "samplePassword"
            };

            // Sample device
            MqttDeviceDescriptor deviceDescriptor = new MqttDeviceDescriptor
            {
                Id = "ha-mqtt",
                Name = "HomeAssistantMQTT",
                Manufactorer = "SomeManufactorer",
                Model = "SomeModel",
                SoftwareVersion = "8.4.7.2"
            };

            // Sample light
            MqttEntityDescriptor lightDescriptor = new MqttEntityDescriptor
            {
                ComponentClass = "light",
                EntityId = "samplelight",
                Name = "MQTT Sample Light",
                Publish = true,
                Route = "sample/light",
                UpdateOnStart = true,
            };

            ManagedMqttClient mqttClient = MqttDevice.CreateMqttClient(broker);
            MqttDevice mqttDevice = new MqttDevice(mqttClient, deviceDescriptor);

            MqttEntity<LightMessage> sampleLightEntity = new MqttEntity<LightMessage>(new Light(), lightDescriptor, mqttDevice);


            Console.ReadLine();
        }
    }
}
