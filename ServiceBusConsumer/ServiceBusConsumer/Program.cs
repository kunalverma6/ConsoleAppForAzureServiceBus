using AzureServiceBusConsumer;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true)
    .Build();

IAzureServiceBusConsumer consumer =
    new AzureServiceBusConsumer1(configuration);

Console.Clear();

Console.WriteLine("======================================");
Console.WriteLine("   Azure Service Bus Consumer");
Console.WriteLine("======================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Select source:");
    Console.WriteLine("1. Consume from Queue");
    Console.WriteLine("2. Consume from Topic");
    Console.WriteLine("3. Exit");

    Console.Write("Enter your choice: ");

    var choice = Console.ReadLine();

    if (choice == "3")
    {
        Console.WriteLine("Exiting...");
        break;
    }

    using var cancellationTokenSource =
        new CancellationTokenSource();

    Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;

        cancellationTokenSource.Cancel();

        Console.WriteLine();
        Console.WriteLine("Stopping consumer...");
    };

    try
    {
        switch (choice)
        {
            case "1":

                await consumer.ConsumeFromQueueAsync(
                    cancellationTokenSource.Token);

                break;

            case "2":

                await consumer.ConsumeFromTopicAsync(
                    cancellationTokenSource.Token);

                break;

            default:

                Console.WriteLine(
                    "Invalid choice.");

                break;
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine(
            "Consumer stopped.");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor =
            ConsoleColor.Red;

        Console.WriteLine(
            $"Error: {ex.Message}");

        Console.ResetColor();
    }
}