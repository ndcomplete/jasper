﻿using System;
using System.Threading;
using Jasper.Bus.Logging;
using Jasper.Bus.Runtime;
using Jasper.Bus.Transports;
using Jasper.Bus.Transports.Configuration;
using Jasper.Bus.Transports.Receiving;
using Jasper.Bus.Transports.Sending;
using Jasper.Bus.WorkerQueues;
using Marten;

namespace Jasper.Marten.Persistence
{
    public class MartenBackedMessagePersistence : IPersistence
    {
        private readonly IDocumentStore _store;
        private readonly CompositeLogger _logger;
        private readonly BusSettings _settings;

        public MartenBackedMessagePersistence(IDocumentStore store, CompositeLogger logger, BusSettings settings)
        {
            _store = store;
            _logger = logger;
            _settings = settings;
        }

        public ISendingAgent BuildSendingAgent(Uri destination, ISender sender, CancellationToken cancellation)
        {
            return new MartenBackedSendingAgent(destination, _store, sender, cancellation, _logger);
        }

        public ISendingAgent BuildLocalAgent(Uri destination, IWorkerQueue queues)
        {
            return new LocalSendingAgent(destination, queues, _store);
        }

        public IListener BuildListener(IListeningAgent agent, IWorkerQueue queues)
        {
            return new MartenBackedListener(agent, queues, _store, _logger, _settings);
        }

        public void ClearAllStoredMessages()
        {
            _store.Advanced.Clean.DeleteDocumentsFor(typeof(Envelope));
        }
    }
}
