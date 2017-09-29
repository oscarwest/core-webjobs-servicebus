// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Core.Library.Triggers
{
    internal class ConverterArgumentBindingProvider<T> : IQueueTriggerArgumentBindingProvider
    {
        private readonly IAsyncConverter<Message, T> _converter;

        public ConverterArgumentBindingProvider(IAsyncConverter<Message, T> converter)
        {
            _converter = converter;
        }

        public ITriggerDataArgumentBinding<Message> TryCreate(ParameterInfo parameter)
        {
            if (parameter.ParameterType != typeof(T))
            {
                return null;
            }

            return new ConverterArgumentBinding(_converter);
        }

        internal class ConverterArgumentBinding : ITriggerDataArgumentBinding<Message>
        {
            private readonly IAsyncConverter<Message, T> _converter;

            public ConverterArgumentBinding(IAsyncConverter<Message, T> converter)
            {
                _converter = converter;
            }

            public Type ValueType
            {
                get { return typeof(T); }
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get { return null; }
            }

            public async Task<ITriggerData> BindAsync(Message value, ValueBindingContext context)
            {
                Message clone = value.Clone();
                object converted = await _converter.ConvertAsync(value, context.CancellationToken);
                IValueProvider provider = await BrokeredMessageValueProvider.CreateAsync(clone, converted, typeof(T),
                    context.CancellationToken);
                return new TriggerData(provider, null);
            }
        }
    }
}
