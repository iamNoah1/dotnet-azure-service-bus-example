using Azure.Messaging.ServiceBus;

namespace AzureServiceBus;

public class ServiceBusManager
{
    string connectionString;

    ServiceBusClient client;
    ServiceBusSender sender;
    ServiceBusReceiver receiver;

    ServiceBusProcessor processor;

    public ServiceBusManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private void init()
    {
        if (null == client)
        {
            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            client = new ServiceBusClient(connectionString, clientOptions);
        }
    }

    public async Task SendMessage(string queue, string message)
    {
        init();

        sender = client.CreateSender(queue);

        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
        {
            throw new Exception($"The message {message} is too large to fit in the batch.");
        }

        try
        {
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"Message {message} has sent to queue/topic");
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }

    public async Task<string> ReceiveOne(string queue, bool complete)
    {
        init();
        receiver = client.CreateReceiver(queue);

        try
        {
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (complete)
            {
                await receiver.CompleteMessageAsync(receivedMessage);
            }
            return receivedMessage.Body.ToString();
        }
        finally
        {
            await receiver.DisposeAsync();
        }
    }

    public async Task<string> ReceiveOne(string topic, string subscription, bool complete)
    {
        init();
        receiver = client.CreateReceiver(topic, subscription);

        try
        {
            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
            if (complete)
            {
                await receiver.CompleteMessageAsync(receivedMessage);
            }
            return receivedMessage.Body.ToString();
        }
        finally
        {
            await receiver.DisposeAsync();
        }
    }

    public void RegisterMessageHandler(string queue, Func<ProcessMessageEventArgs, Task> messageHandler)
    {
        init();
        //TODO vielleicht müssen wir ihn hier immer neu erzeugen. Weil sonst vielleicht bei Startprocessing ein alter benutzt wird. Also einer von einer anderen Queue
        if (null == processor)
        {
            processor = client.CreateProcessor(queue);
        }

        processor.ProcessMessageAsync += messageHandler;
    }

    public void RegisterMessageHandler(string topic, string subscription, Func<ProcessMessageEventArgs, Task> messageHandler)
    {
        init();
        //TODO vielleicht müssen wir ihn hier immer neu erzeugen. Weil sonst vielleicht bei Startprocessing ein alter benutzt wird. Also einer von einer anderen Queue
        if (null == processor)
        {
            processor = client.CreateProcessor(topic, subscription);
        }

        processor.ProcessMessageAsync += messageHandler;
    }

    public void RegisterErrorHandler(string queue, Func<ProcessErrorEventArgs, Task> errorHandler)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(queue);
        }

        processor.ProcessErrorAsync += errorHandler;
    }

    public void RegisterErrorHandler(string topic, string subscription, Func<ProcessErrorEventArgs, Task> errorHandler)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(topic, subscription);
        }

        processor.ProcessErrorAsync += errorHandler;
    }


    public async Task StartProcessing(string queue)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(queue);
        }
        await processor.StartProcessingAsync();
    }

    public async Task StartProcessing(string topic, string subscription)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(topic, subscription);
        }
        await processor.StartProcessingAsync();
    }

    public async Task StopProcessing(string queue)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(queue);
        }
        await processor.StopProcessingAsync();
    }

    public async Task StopProcessing(string topic, string subscription)
    {
        init();
        if (null == processor)
        {
            processor = client.CreateProcessor(topic, subscription);
        }
        await processor.StopProcessingAsync();
    }

}

