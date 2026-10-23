#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace MergeGridGame.Core
{
    public class DIContainer
    {
        private readonly Dictionary<Type, object> _services;

        public DIContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public void Register<T>(T service)
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            Debug.LogError($"Service of type {type} is not registered in DIContainer.");
            return default;
        }

        public void Clear()
        {
            _services.Clear();
        }
    }
}
