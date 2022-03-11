# HomeAssistantMQTT
A library to simplify integration of .NET software with Home Assistant using MQTT

## Overview
This is a simple library that helps integration of a piece of software with an MQTT broker.
It's built to take advantage of HA's MQTT Discovery, but it is completely indipendent.

## Important Note
This is not a complete software, it's just a quick library that I've written in my free time so it's not complete and can contain some shortcuts in the code, such as magic numbers or assumptions for routes and so on.
I have tested this library only with HA light and sensor entities, it's very likely that adjustments must be made for other entities.
So, that's not for beginners, a basic understanding of MQTT, HA mqtt discovery, HA device/entity concepts and C# language is needed.

## References
Home Assistant MQTT: https://www.home-assistant.io/integrations/mqtt/
MQTTnet: https://github.com/dotnet/MQTTnet

## Usage

### MQTT Connection
First, you need to extabilish a connection with a broker. This library makes use of **MQTTnet**, and wants a *ManagedMqttClient* object to begin with.
The **MqttDevice** class has a static method that helps you creating the client, that uses basic user/password login without TLS, so use it only for development or for LAN usage.
Please refer to **MQTTnet** documentation to how to manage all the aspects related to the connection with the broker.
Following chapters assumes that you have extabilished the connection and have a valid *ManagedMqttClient* object available

### MqttDevice
The MqttDevice is the parent of your entities.
It's constructor wants:
- A *ManagedMqttClient* already connected
- A MqttDeviceDescriptor that contains the basic information about your device. See the class properties to valorize it

### MqttEntity\<T>
The MqttEntity represents the entity that will be visible from Home Assistant.
For the moment it's hardcoded to work with *jsonschema* and it's assumed that your entity uses by default the following topics:
- **state**: for transmitting the status
- **set**: for receiving commands
**T** is the type of the message received by the set topic.
Extra routes can be subscribed, messages received from those routes are passed to your implementation without any processing.

The constructor wants:
- *IDevice\<T>*: the implementation of your entity, 
- *MqttEntityDescriptor*: informations about the entity, will be used into the Discovery message for HA to autodetect it
- *MqttDevice:* the parent device
- (optional) *string[]*: other routes to subscribe

### IDevice<T> *interface*
This is the interface that your code must implement to work with HomeAssistantMQTT
It is kept as simple as possible:
```C#
    public interface IDevice<TMSG> where TMSG : class
    {
    // this method will be invoked by the MqttEntity to send commands.
    // Implement command management into this method or call your 
    // commands from here
        void CommandNotification(TMSG command);
    // this method is called when a route defined into the extra routes is called
    // message interpretation is left at your implementation
        void MessageNotification(string topic, string msg);
    // this must return the current status of the entity, used at startup to  
    // initialize MQTT values
        TMSG GetStatus();
    // an handler is attached to this by the MqttEntity. Call this method
    // from your code to notify a status change    
        Action<TMSG> StatusNotification { get; set; }
    // create your discovery message here, it'll be used at startup to publish
    // the entity for discovery.
        DiscoveryMessage GetDiscoveryMessage();
    }
```
### DiscoveryMessage
Inherit from this to create your Mqtt discovery message, following the Home Assistant guidelines. Be careful to use the proper naming of properties.

Don't valorize any of the properties of the base class: they'll be overwritten by MqttEntity and MqttDevice. It's not a perfect implementation, but it works for now.

Classes for lights and sensor's discovery are already provided

