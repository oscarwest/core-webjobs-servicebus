// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Azure.WebJobs.Sdk.Core.Config;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Azure.WebJobs.Sdk.Core.Listeners
{
    internal class ServiceBusQueueListenerFactory : IListenerFactory
    {
        private readonly string _queueName;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ServiceBusConfiguration _config;

        public ServiceBusQueueListenerFactory(string queueName, ITriggeredFunctionExecutor executor, ServiceBusConfiguration config)
        {
            _queueName = queueName;
            _executor = executor;
            _config = config;
        }

        public async Task<IListener> CreateAsync(CancellationToken cancellationToken)
        {
            ServiceBusTriggerExecutor triggerExecutor = new ServiceBusTriggerExecutor(_executor);
            return new ServiceBusListener(_queueName, triggerExecutor, _config);
        }
    }
}
