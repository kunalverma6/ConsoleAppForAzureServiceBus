using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusConsole
{
    public sealed class AzureServiceBusOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string QueueName { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;
    }
}
