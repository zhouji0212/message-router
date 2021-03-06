﻿using System.Collections.Generic;
using System.Linq;
using MessageRouter.Infrastructure;
using MessageRouter.Models;
using NetMQ.Sockets;

namespace MessageRouter.NetMQ
{
    internal class PubSubConnection : IConnection
    {
        PublisherSocket PublisherSocket { get; }
        SubscriberSocket SubscriberSocket { get; }

        public PubSubConnection(PublisherSocket publisherSocket, SubscriberSocket subscriberSocket)
        {
            PublisherSocket = publisherSocket;
            SubscriberSocket = subscriberSocket;
        }

        public void SendMessage(SerializedMessage message) => PublisherSocket.SendMessage(message);

        public bool TryReceiveMessage(out SerializedMessage message) => SubscriberSocket.TryReceiveMessage(out message);

        public void Connect(IEnumerable<string> routeNames)
        {
            routeNames
                .ToList()
                .ForEach(SubscriberSocket.Subscribe);
        }

        public void Disconnect()
        {
            PublisherSocket?.Close();
            PublisherSocket?.Dispose();

            SubscriberSocket?.Close();
            SubscriberSocket?.Dispose();
        }
    }
}