using System;
using Reflex.Scripts;

namespace Reflex
{
    internal class SingletonResolver : IResolver
    {
        private object _instance;
        private readonly Type _concrete;

        public SingletonResolver(Type concrete, object instance)
        {
            _instance = instance;
            _concrete = concrete;
        }

        public object Resolve(IContainer container)
        {
            if (_instance == null)
            {
                _instance = container.Construct(_concrete);
            }

            return _instance;
        }
    }
}