using System;

namespace Reflex
{
    internal class SingletonResolver : IResolver
    {
        private object _instance;
        private readonly Type _concrete;

        public SingletonResolver(Type concrete)
        {
            _concrete = concrete;
        }

        public object Resolve(Container container)
        {
            if (_instance == null)
            {
                _instance = container.Construct(_concrete);
            }

            return _instance;
        }
    }
}