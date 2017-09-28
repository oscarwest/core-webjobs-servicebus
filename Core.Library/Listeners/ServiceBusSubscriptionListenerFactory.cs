// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.ServiceBus;

namespace Microsoft.Azure.WebJobs.ServiceBus.Listeners
{
    internal class ServiceBusSubscriptionListenerFactory : IListenerFactory
    {
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ServiceBusConfiguration _config;

        public ServiceBusSubscriptionListenerFactory(string topicName, string subscriptionName, ITriggeredFunctionExecutor executor, ServiceBusConfiguration config)
        {
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _executor = executor;
            _config = config;
        }

        public async Task<IListener> CreateAsync(CancellationToken cancellationToken)
        {
            string entityPath = $"{_topicName}/Subscriptions/{_subscriptionName}";

            ServiceBusTriggerExecutor triggerExecutor = new ServiceBusTriggerExecutor(_executor);
            return new ServiceBusListener(entityPath, triggerExecutor, _config);
        }
    }
}
