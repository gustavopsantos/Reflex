using System;

namespace Reflex
{
    public class InstanceResolver : IResolver
    {
        public Type Concrete { get; }
        
        private readonly object _instance;
        public int Resolutions { get; private set; }

        public InstanceResolver(object instance)
        {
            Concrete = instance.GetType();
            _instance = instance;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            return _instance;
        }
    }
}