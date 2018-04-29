﻿using System;
using System.Collections.Generic;
using System.Linq;
using NetmqRouter.Helpers;
using NetmqRouter.Infrastructure;
using NetmqRouter.Models;

namespace NetmqRouter.BusinessLogic
{
    internal class DataContractBuilder : IDataContractBuilder, IDataContractAccess
    {
        private readonly List<Route> _routes = new List<Route>();
        private readonly List<RouteSubsriber> _subscribers = new List<RouteSubsriber>();
        private readonly List<Serializer> _serializers  = new List<Serializer>();

        public IReadOnlyList<Route> Routes => _routes;
        public IReadOnlyList<RouteSubsriber> Subscribers => _subscribers;
        public IReadOnlyList<Serializer> Serializers => _serializers;

        public void RegisterSerializer<T>(ISerializer<T> serializer)
        {
            var resultSerializer = Serializer.FromTypeSerializer(serializer);
            RegisterSerializer(resultSerializer);
        }

        public void RegisterGeneralSerializer(Type targetType, IGeneralSerializer serializer)
        {
            var resultSerializer = Serializer.FromGeneralSerializer(targetType, serializer);
            RegisterSerializer(resultSerializer);
        }

        private void RegisterSerializer(Serializer serializer)
        {
            if(_serializers.Any(x => x.TargetType == serializer.TargetType))
                throw new NetmqRouterException($"Serializer for type {serializer.TargetType} is already registered!");

            _serializers.Add(serializer);
        }

        public void RegisterRoute(Route route)
        {
            if(!_serializers.Any(x => route.DataType.IsSameOrSubclass(x.TargetType)))
                throw new NetmqRouterException($"Can not register route with type {route.DataType} because there is no serializer for it.");

            if(_routes.Any(x => x.Name == route.Name))
                throw new NetmqRouterException($"Route with name {route.Name} is already registered.");

            _routes.Add(route);
        }

        public void RegisterSubscriber(RouteSubsriber routeSubsriber)
        {
            if(!_routes.Contains(routeSubsriber.Incoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route (incoming) type and thereferore can not be registered.");

            if(routeSubsriber.Outcoming != null && !_routes.Contains(routeSubsriber.Outcoming))
                throw new NetmqRouterException($"Subscriber refers to not existing route type (outcoming) and thereferore can not be registered.");

            _subscribers.Add(routeSubsriber);
        }
    }
}