﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    internal class DataContractManager : IDataContractOperations
    {
        private IDataContractAccess _dataContract;

        public DataContractManager(IDataContractAccess dataContract)
        {
            _dataContract = dataContract;
        }

        public IEnumerable<string> GetIncomingRouteNames()
        {
            return _dataContract
                .Subscribers
                .Select(x => x.Incoming.Name)
                .Distinct();
        }

        public IEnumerable<Message> CallRoute(Message message)
        {
            return _dataContract
                .Subscribers
                .Where(x => x.Incoming.Name == message.RouteName)
                .Select(x =>
                {
                    var response = x.Method(message.Payload);
                    return (x.Outcoming == null) ? null : new Message(x.Outcoming.Name, response);
                })
                .Where(x => x != null);
        }

        internal ISerializer FindSerializer(Type targetType)
        {
            return _dataContract
                .Serializers
                .FirstOrDefault(x => x.Key == targetType || targetType.IsSubclassOf(x.Key))
                .Value;
        }

        internal (ISerializer serializer, Type targetType) FindSerializer(string routeName)
        {
            var targetType = _dataContract
                .Routes
                .First(x => x.Name == routeName)
                .DataType;

            return (FindSerializer(targetType), targetType);
        }

        public SerializedMessage Serialize(Message message)
        {
            var (serializer, _) = FindSerializer(message.RouteName);
            var data = serializer.Serialize(message.Payload);
            return new SerializedMessage(message.RouteName, data);
        }

        public Message Deserialize(SerializedMessage message)
        {
            var (serializer, targetType) = FindSerializer(message.RouteName);
            var payload = serializer.Deserialize(message.Data, targetType);
            return new Message(message.RouteName, payload);
        }
    }
}