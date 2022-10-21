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
        (string connectionString, string queueName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        string message = await manager.receiveOne(queueName, false);
        logger.LogInformation("try to process message: " + message);

        return Ok();
    }

    [HttpPost("succeedone")]
    public async Task<IActionResult> SucceedProcessingOneMessage()
    {
        (string connectionString, string queueName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        string message = await manager.receiveOne(queueName, true);
        logger.LogInformation("try to process message: " + message);

        return Ok();
    }

    [HttpPost("processall")]
    public async Task<IActionResult> SucceedProcessingAllMessages()
    {
        (string connectionString, string queueName) = GetServiceBusParams();
        manager = new ServiceBusManager(connectionString);

        manager.registerMessageHandler(queueName, MessageHandler);
        manager.registerErrorHandler(queueName, ErrorHandler);

        await manager.StartProcessing(queueName);

        await Task.Delay(10000);

        return Ok();
    }

    private (string, string) GetServiceBusParams()
    {
        string? connectionString = System.Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
        if (String.IsNullOrEmpty(connectionString) || String.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("need to set connection string via env variable SERVICE_BUS_CONNECTION_STRING !");
        }

        string? queueName = System.Environment.GetEnvironmentVariable("QUEUE_NAME");
        if (String.IsNullOrEmpty(queueName) || String.IsNullOrWhiteSpace(queueName))
        {
            throw new Exception("need to set queue name, messages should be written to, via env variable QUEUE_NAME!");
        }

        return (connectionString, queueName);
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        logger.LogInformation("try to process message: " + body);

        await args.CompleteMessageAsync(args.Message);
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.ErrorSource);
        // the fully qualified namespace is available
        Console.WriteLine(args.FullyQualifiedNamespace);
        // as well as the entity path
        Console.WriteLine(args.EntityPath);
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

}
