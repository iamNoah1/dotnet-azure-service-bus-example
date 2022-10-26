using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AzureServiceBus.Controllers;

[ApiController]
[Route("[controller]")]
public class DataIngress : ControllerBase
{
    private readonly ILogger<DataIngress> logger;

    private ServiceBusManager manager;

    public DataIngress(ILogger<DataIngress> logger)
    {
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> FeedData([FromQuery] string topicName, JObject payload)
    {
        string connectionString = System.Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");

        this.manager = new ServiceBusManager(connectionString);
        await manager.SendMessage(topicName, payload.ToString());

        return new OkObjectResult(null);
    }

}
