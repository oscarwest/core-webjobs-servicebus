﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Azure.WebJobs.Sdk.Core.Config
{
    /// <summary>
    /// Extension methods for ServiceBus integration
    /// </summary>
    public static class ServiceBusJobHostConfigurationExtensions
    {
        /// <summary>
        /// Enables use of ServiceBus job extensions
        /// </summary>
        /// <param name="config">The <see cref="JobHostConfiguration"/> to configure.</param>
        public static void UseServiceBus(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration();

            config.UseServiceBus(serviceBusConfig);
        }

        /// <summary>
        /// Enables use of ServiceBus job extensions
        /// </summary>
        /// <param name="config">The <see cref="JobHostConfiguration"/> to configure.</param>
        /// <param name="serviceBusConfig">The <see cref="ServiceBusConfiguration"></see> to use./></param>
        public static void UseServiceBus(this JobHostConfiguration config, ServiceBusConfiguration serviceBusConfig)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (serviceBusConfig == null)
            {
                throw new ArgumentNullException("serviceBusConfig");
            }

            ServiceBusExtensionConfig extensionConfig = new ServiceBusExtensionConfig(serviceBusConfig);

            IExtensionRegistry extensions = config.GetService<IExtensionRegistry>();
            extensions.RegisterExtension<IExtensionConfigProvider>(extensionConfig);
        }
    }
}
