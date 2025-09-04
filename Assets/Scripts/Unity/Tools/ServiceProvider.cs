using System;
using System.Collections.Generic;

namespace Scripts.Unity.Tools
{
    public class ServiceProvider
    {
        private Dictionary<Type, object> _services = new();

        public void AddService<T>(T service) where T : class
        {
            _services.Add(service.GetType(), service);
        }

        public T GetSerivce<T>() => (T)_services[typeof(T)];

        public void Clear()
        {
            _services.Clear();
        }
    }
}
