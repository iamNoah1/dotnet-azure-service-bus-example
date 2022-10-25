# Dotnet Azure Service Bus Example

This repository contains projects that show examples on how to work an Azure Service Bus withing dotnet 6 applications. We have two use cases. 

* [Mirroring](/Mirroring/readme.md), meaning send a message to a queue (or more specific a topic) and having multiple subscribers receiving that message simulatanously. What happens under the hood, is that messages entering a topic are cloned among subscribers. 

* [Worker](/Worker/readme.md) pattern, where a message is sent to a queue and having only one of multiple potential subsribers procesing that message. 

Note that for both scenarios a message is only removed from a queue or topic + subscription, if a subscriber successfully processed the message (completing is the more specific term, with relates better to the code part). Otherwise the message will be put back to the queue or topic + subscription so that the a subscriber can try again. 

## Prerequisites
* Dotnet installed
* A Service Bus instance either on Azure or locally using Azurite for example
* Set `SERVICE_BUS_CONNECTION_STRING` to the connection string of your Service Bus

## Additional Information
* https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/servicebus/Azure.Messaging.ServiceBus/samples
* https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues
* https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions 
* https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview