using AzureServiceBusConsole;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();

IAzureServiceBusProducer producer =
    new AzureServiceBusProducer(configuration);

Console.Clear();

Console.WriteLine("======================================");
Console.WriteLine("   Azure Service Bus Console Producer");
Console.WriteLine("======================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Select destination:");
    Console.WriteLine("1. Queue");
    Console.WriteLine("2. Topic");
    Console.WriteLine("3. Exit");
    Console.Write("Enter your choice: ");

    var choice = Console.ReadLine();

    if (choice == "3")
    {
        Console.WriteLine("Exiting...");
        break;
    }

    if (choice != "1" && choice != "2")
    {
        Console.WriteLine("Invalid choice.");
        continue;
    }

    Console.WriteLine();
    Console.Write("Enter message: ");

    var message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine("Message cannot be empty.");
        continue;
    }

    try
    {
        if (choice == "1")
        {
            await producer.SendToQueueAsync(message);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Message successfully sent to Queue.");
            Console.ResetColor();
        }
        else
        {
            await producer.PublishToTopicAsync(message);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Message successfully published to Topic.");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Failed to send message.");
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
    }
}