using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusConsole
{
    public interface IAzureServiceBusProducer
    {
        Task SendToQueueAsync(
            string message,
            CancellationToken cancellationToken = default);

        Task PublishToTopicAsync(
            string message,
            CancellationToken cancellationToken = default);
    }
}
