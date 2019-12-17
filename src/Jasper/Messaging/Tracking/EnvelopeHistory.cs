using System;
using System.Collections.Generic;
using System.Linq;
using Jasper.Messaging.Runtime;
using Jasper.Messaging.Transports;
using Jasper.Messaging.Transports.Sending;

namespace Jasper.Messaging.Tracking
{
    public class EnvelopeHistory
    {
        private readonly List<EnvelopeRecord> _records = new List<EnvelopeRecord>();

        public EnvelopeHistory(Guid envelopeId)
        {
            EnvelopeId = envelopeId;
        }

        public Guid EnvelopeId { get; }


        public object Message
        {
            get
            {
                return _records
                    .FirstOrDefault(x => x.Envelope.Message != null)?.Envelope.Message;
            }
        }

        private EnvelopeRecord lastOf(EventType eventType)
        {
            return _records.LastOrDefault(x => x.EventType == eventType);
        }

        private void markLastCompleted(EventType eventType)
        {
            var record = lastOf(eventType);
            if (record != null)
            {
                record.IsComplete = true;
            }
        }


        /// <summary>
        /// Tracks activity for coordinating the testing of a single Jasper
        /// application
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="envelope"></param>
        /// <param name="sessionTime"></param>
        /// <param name="serviceName"></param>
        /// <param name="exception"></param>
        public void RecordLocally(EnvelopeRecord record)
        {
            switch (record.EventType)
            {
                case EventType.Sent:
                    // Not tracking anything outgoing
                    // when it's testing locally
                    if (record.Envelope.Destination.Scheme != TransportConstants.Local || record.Envelope.MessageType == TransportConstants.ScheduledEnvelope)
                    {
                        record.IsComplete = true;
                    }

                    if (record.Envelope.Status == TransportConstants.Scheduled)
                    {
                        record.IsComplete = true;
                    }

                    break;

                case EventType.Received:
                    if (record.Envelope.Destination.Scheme == TransportConstants.Local)
                    {
                        markLastCompleted(EventType.Sent);
                    }

                    break;

                case EventType.ExecutionStarted:
                    // Nothing special here
                    break;



                case EventType.ExecutionFinished:
                    markLastCompleted(EventType.ExecutionStarted);
                    record.IsComplete = true;
                    break;

                case EventType.NoHandlers:
                case EventType.NoRoutes:
                case EventType.MessageFailed:
                case EventType.MessageSucceeded:
                    // The message is complete
                    foreach (var envelopeRecord in _records)
                    {
                        envelopeRecord.IsComplete = true;
                    }

                    record.IsComplete = true;

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(record.EventType), record.EventType, null);
            }

            _records.Add(record);
        }

        public void RecordCrossApplication(EnvelopeRecord record)
        {

            switch (record.EventType)
            {
                case EventType.Sent:
                    if (record.Envelope.Status == TransportConstants.Scheduled)
                    {
                        record.IsComplete = true;
                    }
                    break;

                case EventType.ExecutionStarted:
                    break;

                case EventType.Received:
                    markLastCompleted(EventType.Sent);
                    break;


                case EventType.ExecutionFinished:
                    markLastCompleted(EventType.ExecutionStarted, record.UniqueNodeId);
                    record.IsComplete = true;
                    break;

                case EventType.MessageFailed:
                case EventType.MessageSucceeded:
                    // The message is complete
                    foreach (var envelopeRecord in _records.ToArray().Where(x => x.UniqueNodeId == record.UniqueNodeId))
                    {
                        envelopeRecord.IsComplete = true;
                    }

                    record.IsComplete = true;

                    break;

                case EventType.NoHandlers:
                case EventType.NoRoutes:

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(record.EventType), record.EventType, null);
            }

            _records.Add(record);
        }

        private void markLastCompleted(EventType eventType, int uniqueNodeId)
        {
            var record = _records.LastOrDefault(x => x.EventType == eventType && x.UniqueNodeId == uniqueNodeId);
            if (record != null) record.IsComplete = true;
        }


        public bool IsComplete()
        {
            return _records.All(x => x.IsComplete);
        }

        public IEnumerable<EnvelopeRecord> Records => _records;


        public bool Has(EventType eventType)
        {
            return _records.Any(x => x.EventType == eventType);
        }

        public object MessageFor(EventType eventType)
        {
            return _records.Where(x => x.EventType == eventType)
                .LastOrDefault(x => x.Envelope.Message != null)?.Envelope.Message;
        }
    }
}
