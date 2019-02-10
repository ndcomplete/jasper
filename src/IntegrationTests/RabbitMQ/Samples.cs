﻿using System;
using Jasper;
using Jasper.Messaging.Runtime;
using Jasper.RabbitMQ;
using RabbitMQ.Client;

namespace IntegrationTests.RabbitMQ
{
    // SAMPLE: AppListeningToRabbitMQ
    public class AppListeningToRabbitMQ : JasperRegistry
    {
        public AppListeningToRabbitMQ()
        {
            // Port is optional if you're using the default RabbitMQ
            // port of 5672, but shown here for completeness
            Transports.ListenForMessagesFrom("rabbitmq://rabbitserver:5672/messages");
        }
    }
    // ENDSAMPLE

    // SAMPLE: AppPublishingToRabbitMQ
    public class AppPublishingToRabbitMQ : JasperRegistry
    {
        public AppPublishingToRabbitMQ()
        {
            Publish.AllMessagesTo("rabbitmq://rabbitserver:5672/messages");
        }
    }
    // ENDSAMPLE


    // SAMPLE: CustomizedRabbitMQApp
    public class CustomizedRabbitMQApp : JasperRegistry
    {
        public CustomizedRabbitMQApp()
        {
            Settings.Alter<RabbitMqSettings>(settings =>
            {
                // Retrieve the Jasper "agent" by the full Uri:
                var endpoint = settings.ForEndpoint("rabbitmq://server1/queue1");

                // Customize the underlying ConnectionFactory for security mechanisms,
                // timeouts, and many other settings
                endpoint.Broker.ConnectionFactory.ContinuationTimeout = TimeSpan.FromSeconds(5);


                // Customize or change how Jasper maps Envelopes to and from
                // the RabbitMQ properties
                endpoint.EnvelopeMapping = new CustomEnvelopeMapping();
            });
        }
    }

    public class CustomEnvelopeMapping : DefaultEnvelopeMapper
    {
        public override Envelope ReadEnvelope(byte[] data, IBasicProperties props)
        {
            // Customize the mappings from RabbitMQ headers to
            // Jasper's Envelope values
            return base.ReadEnvelope(data, props);
        }

        public override void WriteFromEnvelope(Envelope envelope, IBasicProperties properties)
        {
            // Customize how Jasper Envelope objects are mapped to
            // the outgoing RabbitMQ message structure
            base.WriteFromEnvelope(envelope, properties);
        }
    }

    // ENDSAMPLE
}
