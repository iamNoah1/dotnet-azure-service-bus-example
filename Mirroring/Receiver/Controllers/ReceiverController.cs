using Azure.Messaging.ServiceBus;
using AzureServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace Receiver.Controllers;

[ApiController]
[Route("[controller]")]
public class ReceiverController : ControllerBase
{
    private readonly ILogger<ReceiverController> logger;

    private ServiceBusManager manager;

    public ReceiverController(ILogger<ReceiverController> logger)
    {
        this.logger = logger;
    }

    [HttpPost("failone")]
    public async Task<IActionResult> FailProcessingOneMessage()
    {
        (string connectionString, string topicName, string subscriptionName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        string message = await manager.ReceiveOne(topicName, subscriptionName, false);
        logger.LogInformation("try to process message: " + message);

        return Ok();
    }

    [HttpPost("succeedone")]
    public async Task<IActionResult> SucceedProcessingOneMessage()
    {
        (string connectionString, string topicName, string subscriptionName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        string message = await manager.ReceiveOne(topicName, subscriptionName, true);
        logger.LogInformation("try to process message: " + message);

        return Ok();
    }

    [HttpPost("processall")]
    public async Task<IActionResult> SucceedProcessingAllMessages()
    {
        (string connectionString, string topicName, string subscriptionName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        manager.RegisterMessageHandler(topicName, subscriptionName, MessageHandler);
        manager.RegisterErrorHandler(topicName, subscriptionName, ErrorHandler);

        await manager.StartProcessing(topicName);

        await Task.Delay(10000);

        return Ok();
    }

    [HttpPost("stopProcessing")]
    public async Task<IActionResult> StopProccessing()
    {
        (string connectionString, string topicName, string subscriptionName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        await manager.StopProcessing(topicName, subscriptionName);

        return Ok();
    }

    private (string, string, string) GetServiceBusParams()
    {
        string? connectionString = System.Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
        if (String.IsNullOrEmpty(connectionString) || String.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("need to set connection string via env variable SERVICE_BUS_CONNECTION_STRING !");
        }

        string? topicName = System.Environment.GetEnvironmentVariable("TOPIC_NAME");
        if (String.IsNullOrEmpty(topicName) || String.IsNullOrWhiteSpace(topicName))
        {
            throw new Exception("need to set topic name, messages should be grabbed from, via env variable TOPIC_NAME!");
        }

        string? subscriptionName = System.Environment.GetEnvironmentVariable("SUBSCRIPTION_NAME");
        if (String.IsNullOrEmpty(subscriptionName) || String.IsNullOrWhiteSpace(subscriptionName))
        {
            throw new Exception("need to set the subscription name, messages should be grabbed from, via env variable SUBSCRIPTION_NAME!");
        }

        return (connectionString, topicName, subscriptionName);
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        logger.LogInformation("try to process message: " + body);

        await args.CompleteMessageAsync(args.Message);
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.ErrorSource);
        Console.WriteLine(args.FullyQualifiedNamespace);
        Console.WriteLine(args.EntityPath);
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

}
