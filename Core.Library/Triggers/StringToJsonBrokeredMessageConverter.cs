﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Azure.WebJobs.Host.Converters;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace Microsoft.Azure.WebJobs.ServiceBus.Triggers
{
    internal class StringToJsonBrokeredMessageConverter : IConverter<string, Message>
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Message Convert(string input)
        {
            Message message = new Message(Encoding.UTF8.GetBytes(input));
            message.ContentType = ContentTypes.ApplicationJson;
            return message;
        }
    }
}
