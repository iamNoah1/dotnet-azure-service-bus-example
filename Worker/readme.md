# Worker 

The focus of the example applications in this directory is to show how a worker pattern can be implemented. Worker pattern means, that a message from a queue is processed only by one of possibly multiple subscribers of a queue. Only if the message was processed successfully by one of the subscribers (workers) the message is removed from the queue. Otherwise another worker will pick up the message and try to process it. 

This project not only shows how to implement the worker pattern, it can be used to play around and understand how the service bus behaves. 

## Prerequisites 
* A queue exists inside the Service Bus Instance 
* An environment variable named `QUEUE_NAME` should be set to the name of queue 

## Run 
* `dotnet run --project Publisher` to push some messages to the queue
* `dotnet run --project Receiver` to start the receiver service. See next chapter for available endpoints. 

## Receiver Endpoints
* POST `host/receiver/failone` - Grabs a message, but mocks failing to process that message by not completing it
* POST `host/receiver/succeedone` - Grabs a message and succeeds processing that message
* POST `host/receiver/processall` - Grab all available messages and succeds processing them 