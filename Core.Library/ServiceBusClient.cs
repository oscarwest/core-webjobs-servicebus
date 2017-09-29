// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Core.Library
{
    internal static class ServiceBusClient
    {
        internal static string GetNamespaceName(Uri address)
        {
            if (address == null && !address.IsAbsoluteUri && address.HostNameType != UriHostNameType.Dns)
            {
                return null;
            }

            return GetLeastSignificantSubdomain(address.Host);
        }

        private static string GetLeastSignificantSubdomain(string host)
        {
            if (String.IsNullOrEmpty(host))
            {
                return null;
            }

            int separatorIndex = host.IndexOf('.');

            if (separatorIndex <= 0)
            {
                return null;
            }

            return host.Substring(0, separatorIndex);
        }
    }
}
