﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Core.Library.Config;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Core.Library.Listeners
{
    internal sealed class ServiceBusListener : IListener
    {
        private readonly MessagingProvider _messagingProvider;
        private readonly string _entityPath;
        private readonly ServiceBusTriggerExecutor _triggerExecutor;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly MessageProcessor _messageProcessor;

        private MessageReceiver _receiver;
        private bool _disposed;

        public ServiceBusListener(string entityPath, ServiceBusTriggerExecutor triggerExecutor, ServiceBusConfiguration config)
        {
            _entityPath = entityPath;
            _triggerExecutor = triggerExecutor;
            _cancellationTokenSource = new CancellationTokenSource();
            _messagingProvider = config.MessagingProvider;
            _messageProcessor = config.MessagingProvider.CreateMessageProcessor(entityPath);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_receiver != null)
            {
                throw new InvalidOperationException("The listener has already been started.");
            }

            return StartAsyncCore(cancellationToken);
        }

        private Task StartAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _receiver = _messagingProvider.CreateMessageReceiver(_entityPath);
            _receiver.RegisterMessageHandler(ProcessMessageAsync, _messageProcessor.MessageOptions);

            return Task.FromResult(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_receiver == null)
            {
                throw new InvalidOperationException(
                    "The listener has not yet been started or has already been stopped.");
            }

            // Signal ProcessMessage to shut down gracefully
            _cancellationTokenSource.Cancel();

            return StopAsyncCore(cancellationToken);
        }

        private async Task StopAsyncCore(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _receiver.CloseAsync();
            _receiver = null;
        }

        public void Cancel()
        {
            ThrowIfDisposed();
            _cancellationTokenSource.Cancel();
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cancellationTokenSource")]
        public void Dispose()
        {
            if (!_disposed)
            {
                // Running callers might still be using the cancellation token.
                // Mark it canceled but don't dispose of the source while the callers are running.
                // Otherwise, callers would receive ObjectDisposedException when calling token.Register.
                // For now, rely on finalization to clean up _cancellationTokenSource's wait handle (if allocated).
                _cancellationTokenSource.Cancel();

                if (_receiver != null)
                {
                    _receiver.CloseAsync().Wait();
                    _receiver = null;
                }

                _disposed = true;
            }
        }

        private Task ProcessMessageAsync(Message message)
        {
            return ProcessMessageAsync(message, _cancellationTokenSource.Token);
        }

        internal async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            if (!await _messageProcessor.BeginProcessingMessageAsync(message, cancellationToken))
            {
                return;
            }

            FunctionResult result = await _triggerExecutor.ExecuteAsync(message, cancellationToken);

            await _messageProcessor.CompleteProcessingMessageAsync(message, result, cancellationToken);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
