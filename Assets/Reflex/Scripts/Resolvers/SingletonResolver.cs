using System;

namespace Reflex
{
    internal class SingletonResolver : IResolver
    {
        private object _instance;
        private readonly Type _concrete;
        public int Resolutions { get; private set; }

        public SingletonResolver(Type concrete)
        {
            _concrete = concrete;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            
            if (_instance == null)
            {
                _instance = container.Construct(_concrete);
            }

            return _instance;
        }
    }
}