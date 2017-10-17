// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Azure.WebJobs.Sdk.Core.Triggers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Azure.WebJobs.Sdk.Core.Config
{
    /// <summary>
    /// Extension configuration provider used to register ServiceBus triggers and binders
    /// </summary>
    internal class ServiceBusExtensionConfig : IExtensionConfigProvider
    {
        private ServiceBusConfiguration _serviceBusConfig;

        /// <summary>
        /// Creates a new <see cref="ServiceBusExtensionConfig"/> instance.
        /// </summary>
        /// <param name="serviceBusConfig">The <see cref="ServiceBusConfiguration"></see> to use./></param>
        public ServiceBusExtensionConfig(ServiceBusConfiguration serviceBusConfig)
        {
            if (serviceBusConfig == null)
            {
                throw new ArgumentNullException("serviceBusConfig");
            }

            _serviceBusConfig = serviceBusConfig;
        }

        /// <summary>
        /// Gets the <see cref="ServiceBusConfiguration"/>
        /// </summary>
        public ServiceBusConfiguration Config
        {
            get
            {
                return _serviceBusConfig;
            }
        }

        /// <inheritdoc />
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // get the services we need to construct our binding providers
            INameResolver nameResolver = context.Config.GetService<INameResolver>();
            IExtensionRegistry extensions = context.Config.GetService<IExtensionRegistry>();

            // register our trigger binding provider
            ServiceBusTriggerAttributeBindingProvider triggerBindingProvider = new ServiceBusTriggerAttributeBindingProvider(nameResolver, _serviceBusConfig);
            extensions.RegisterExtension<ITriggerBindingProvider>(triggerBindingProvider);
        }
    }
}
