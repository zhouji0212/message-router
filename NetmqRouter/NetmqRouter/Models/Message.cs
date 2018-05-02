﻿using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NetmqRouter.Tests")]

namespace NetmqRouter.Models
{
    internal class Message
    {
        public readonly string RouteName;
        public readonly object Payload;

        public Message(string routeName, object payload)
        {
            RouteName = routeName;
            Payload = payload;
        }

        public override bool Equals(object obj)
        {
            return obj is Message r &&
                   r.RouteName == RouteName &&
                   r.Payload == Payload;
        }
    }
}
