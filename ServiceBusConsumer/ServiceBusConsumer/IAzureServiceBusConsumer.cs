using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusConsumer;

public interface IAzureServiceBusConsumer
{
    Task ConsumeFromQueueAsync(
        CancellationToken cancellationToken = default);

    Task ConsumeFromTopicAsync(
        CancellationToken cancellationToken = default);
}
