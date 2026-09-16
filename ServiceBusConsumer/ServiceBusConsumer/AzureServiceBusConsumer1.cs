using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace AzureServiceBusConsumer;

public sealed class AzureServiceBusConsumer1
    : IAzureServiceBusConsumer
{
    private readonly ServiceBusClient _client;
    private readonly AzureServiceBusOptions _options;

    public AzureServiceBusConsumer1(IConfiguration configuration)
    {
        _options = configuration
            .GetSection("AzureServiceBus")
            .Get<AzureServiceBusOptions>()
            ?? throw new InvalidOperationException(
                "AzureServiceBus configuration is missing.");

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure Service Bus connection string is missing.");
        }

        _client = new ServiceBusClient(
            _options.ConnectionString);
    }

    public async Task ConsumeFromQueueAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine();
        Console.WriteLine(
            $"Listening to Queue: {_options.QueueName}");

        Console.WriteLine(
            "Press Ctrl+C to stop.");

        Console.WriteLine();

        await using var processor =
            _client.CreateProcessor(
                _options.QueueName,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

        processor.ProcessMessageAsync += ProcessMessageAsync;

        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(
            cancellationToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when Ctrl+C is pressed.
        }

        await processor.StopProcessingAsync();
    }

    public async Task ConsumeFromTopicAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine();
        Console.WriteLine(
            $"Listening to Topic: {_options.TopicName}");

        Console.WriteLine(
            $"Subscription: {_options.TopicSubscriptionName}");

        Console.WriteLine(
            "Press Ctrl+C to stop.");

        Console.WriteLine();

        await using var processor =
            _client.CreateProcessor(
                _options.TopicName,
                _options.TopicSubscriptionName,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

        processor.ProcessMessageAsync += ProcessMessageAsync;

        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(
            cancellationToken);

        try
        {
            await Task.Delay(
                Timeout.Infinite,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when Ctrl+C is pressed.
        }

        await processor.StopProcessingAsync();
    }

    private static async Task ProcessMessageAsync(
        ProcessMessageEventArgs args)
    {
        var message = args.Message;

        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("======================================");
        Console.WriteLine("MESSAGE RECEIVED");
        Console.WriteLine("======================================");

        Console.WriteLine($"MessageId : {message.MessageId}");
        Console.WriteLine($"Subject   : {message.Subject}");
        Console.WriteLine($"Content   : {message.Body}");
        Console.WriteLine($"Enqueued  : {message.EnqueuedTime}");

        Console.ResetColor();

        Console.WriteLine();

        // Tell Azure Service Bus that the message
        // was successfully processed.
        await args.CompleteMessageAsync(message);
    }

    private static Task ProcessErrorAsync(
        ProcessErrorEventArgs args)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine("======================================");
        Console.WriteLine("SERVICE BUS ERROR");
        Console.WriteLine("======================================");

        Console.WriteLine($"Error : {args.Exception.Message}");
        Console.WriteLine($"Source: {args.ErrorSource}");

        Console.ResetColor();

        return Task.CompletedTask;
    }
}