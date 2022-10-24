# Mirroring

The focus of the example applications in this directory is to show how a mirroring pattern can be implemented. Mirroring pattern means, that a message is sent to multipe subscribers, where each one of the subscribers (not only one, compared to the worker pattern) will process the message. 

This project not only shows how to implement the mirroring pattern, it can be used to play around and understand how the service bus behaves. 

## Prerequisites 
* A topic exists inside the Service Bus Instance. From a publishing point if view, the code to publish a message to a queue does not differ to the code publishing a message to a topic
* An environment variable named `TOPIC_NAME` should be set to the name of the topic
* An environment variable named `SUBSCRIPTION_NAME` should be set to the name of the subscription

## Run 
* `dotnet run --project Publisher` and call the publishling endpoint to sent a message to the topic
* `dotnet run --project Receiver` and use the different endpoints to play around

## Publisher Endpoints
* POST `host/dataingress?topicName=<topicname>` with some body payload as json - sends the json body payload as string message to the topic

## Receiver Endpoints