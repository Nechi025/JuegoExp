using System;
using System.Collections.Generic;

namespace Core.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public static void Register<T>(T service) where T : IService
        {
            _services[typeof(T)] = service;
        }

        public static T Get<T>() where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new InvalidOperationException(
                $"[ServiceLocator] Service of type '{typeof(T).Name}' is not registered.");
        }

        public static bool TryGet<T>(out T service) where T : IService
        {
            if (_services.TryGetValue(typeof(T), out var s) && s is T typed)
            {
                service = typed;
                return true;
            }
            service = default;
            return false;
        }

        public static void Unregister<T>() where T : IService
        {
            _services.Remove(typeof(T));
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}
