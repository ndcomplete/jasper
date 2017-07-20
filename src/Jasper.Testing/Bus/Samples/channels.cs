﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Jasper.Bus;
using Jasper.Bus.Runtime;

namespace Jasper.Testing.Bus.Samples
{
    // SAMPLE: SampleSettings
    public class SampleSettings
    {
        public Uri Pinger { get; set; } =
            "lq.tcp://localhost:2352/pinger".ToUri();

        public Uri Ponger { get; set; } =
            "lq.tcp://localhost:2353/ponger".ToUri();
    }
    // ENDSAMPLE

    // SAMPLE: PingApp
    public class PingApp : JasperBusRegistry
    {
        public PingApp(SampleSettings settings)
        {
            // Configuring PingApp to send PingMessage's
            // to the PongApp
            Messages.SendMessage<PingMessage>()
                .To(settings.Pinger);

            // Listen for incoming messages from "Pinger"
            Channels.ListenForMessagesFrom(settings.Pinger);
        }
    }

    public class PongApp : JasperBusRegistry
    {
        public PongApp(SampleSettings settings)
        {
            // Listen for incoming messages from "Ponger"
            Channels.ListenForMessagesFrom(settings.Ponger);
        }
    }
    // ENDSAMPLE

    public class PingMessage
    {
    }

    public class PongMessage
    {
    }

    // SAMPLE: ControlChannelApp
    public class ControlChannelApp : JasperBusRegistry
    {
        public ControlChannelApp(AppSettings settings)
        {
            Channels[settings.Control]
                .UseAsControlChannel()
                .DeliveryFastWithoutGuarantee();
        }
    }
    // ENDSAMPLE

    // SAMPLE: ListeningApp
    public class ListeningApp : JasperBusRegistry
    {
        public ListeningApp(SampleSettings settings)
        {
            // Listen for incoming messages from "Pinger"
            Channels.ListenForMessagesFrom(settings.Pinger);
        }
    }
    // ENDSAMPLE

    // SAMPLE: PersistentMessageChannels
    public class AppSettings
    {
        // This channel handles "fire and forget"
        // control messages
        public Uri Control { get; set; }
            = new Uri("lq.tcp://localhost:2345/control");


        // This channel handles normal business
        // processing messages
        public Uri Transactions { get; set; }
            = new Uri("lq.tcp://localhost:2346/transactions");
    }

    public class BigApp : JasperBusRegistry
    {
        public BigApp(AppSettings settings)
        {
            // Declare that the "Control" channel
            // use the faster, but unsafe transport mechanism
            Channels[settings.Control]
                .DeliveryFastWithoutGuarantee()
                .UseAsControlChannel();


            Channels[settings.Transactions]
                // This is the default, but you can
                // still configure it explicitly
                .DeliveryGuaranteed();

        }
    }
    // ENDSAMPLE

    // SAMPLE: sending-messages-for-static-routing
    public class SendingExample
    {
        public async Task SendPingsAndPongs(IServiceBus bus)
        {
            // Publish a message
            await bus.Send(new PingMessage());

            // Request/Reply
            var pong = await bus.Request<PongMessage>(new PingMessage());
        }
    }
    // ENDSAMPLE

    // SAMPLE: StaticRoutingApp
    public class StaticRoutingApp : JasperBusRegistry
    {
        public StaticRoutingApp(AppSettings settings)
        {
            // Explicitly add a single message type
            Messages.SendMessage<PingMessage>()
                .To(settings.Transactions);

            // Publish any types matching the supplied filter
            // to this channel
            Messages.SendMessages("Message suffix", type => type.Name.EndsWith("Message"))
                .To(settings.Transactions);

            // Publish any message type contained in the assembly
            // to this channel, by supplying a type contained
            // within that assembly
            Messages.SendMessagesFromAssemblyContaining<PingMessage>()
                .To(settings.Transactions);

            // Publish any message type contained in the named
            // assembly to this channel
            Messages.SendMessagesFromAssembly(Assembly.Load(new AssemblyName("MyMessageLibrary")))
                .To(settings.Transactions);

            // Publish any message type contained in the
            // namespace given to this channel
            Messages.SendMessagesInNamespace("MyMessageLibrary")
                .To(settings.Transactions);

            // Publish any message type contained in the namespace
            // of the type to this channel
            Messages.SendMessagesInNamespaceContaining<PingMessage>()
                .To(settings.Transactions);
        }
    }
    // ENDSAMPLE
}
