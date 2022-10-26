using AzureServiceBus;

string? connectionString = System.Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
if (String.IsNullOrEmpty(connectionString) || String.IsNullOrWhiteSpace(connectionString)) {
    throw new Exception("need to set connection string via env variable SERVICE_BUS_CONNECTION_STRING !");
}

string? queueName = System.Environment.GetEnvironmentVariable("QUEUE_NAME");
if (String.IsNullOrEmpty(queueName) || String.IsNullOrWhiteSpace(queueName)) {
    throw new Exception("need to set queue name, messages should be written to, via env variable QUEUE_NAME!");
}

ServiceBusManager manager = new ServiceBusManager(connectionString);

Console.WriteLine("Writing some messages to the queue");

for (int i = 0; i < 10; i++) {
    await manager.SendMessage(queueName, "Message Nr: " + i);

    await Task.Delay(500); //look busy
}

Console.WriteLine("Done");

