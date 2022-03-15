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
                IpAddress = "10.39.24.141",
                Username = "device",
                Password = "device"
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

            // Sample sensor
            MqttEntityDescriptor sensorDescriptor = new MqttEntityDescriptor
            {
                ComponentClass = "sensor",
                EntityId = "sampleSensor",
                Name = "MQTT Sample Sensor",
                Publish = true,
                Route = "sample/sensor",
                UpdateOnStart = true,
            };

            ManagedMqttClient mqttClient = MqttDevice.CreateMqttClient(broker);
            MqttDevice mqttDevice = new MqttDevice(mqttClient, deviceDescriptor);
            _ = new MqttEntity<LightMessage>(new Light(), lightDescriptor, mqttDevice);
            _ = new MqttEntity(new Sensor(), sensorDescriptor, mqttDevice);

            Console.ReadLine();
        }
    }
}
