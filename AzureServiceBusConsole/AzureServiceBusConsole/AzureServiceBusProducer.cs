using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusConsole
{
    public sealed class AzureServiceBusProducer : IAzureServiceBusProducer
    {
        private readonly ServiceBusClient _client;
        private readonly AzureServiceBusOptions _options;

        public AzureServiceBusProducer(IConfiguration configuration)
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

        public async Task SendToQueueAsync(
            string message,
            CancellationToken cancellationToken = default)
        {
            await using var sender =
                _client.CreateSender(_options.QueueName);

            var serviceBusMessage = new ServiceBusMessage(message)
            {
                ContentType = "text/plain",
                Subject = "ConsoleMessage"
            };

            await sender.SendMessageAsync(
                serviceBusMessage,
                cancellationToken);
        }

        public async Task PublishToTopicAsync(
            string message,
            CancellationToken cancellationToken = default)
        {
            await using var sender =
                _client.CreateSender(_options.TopicName);

            var serviceBusMessage = new ServiceBusMessage(message)
            {
                ContentType = "text/plain",
                Subject = "ConsoleMessage"
            };

            await sender.SendMessageAsync(
                serviceBusMessage,
                cancellationToken);
        }
    }
}
