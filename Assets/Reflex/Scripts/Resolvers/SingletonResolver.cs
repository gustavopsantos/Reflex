using System;
using Reflex.Scripts;

namespace Reflex
{
    internal class SingletonResolver : Resolver
    {
        private object _instance;
        private readonly Type _concrete;

        public SingletonResolver(Type concrete, object instance)
        {
            _instance = instance;
            _concrete = concrete;
        }

        internal override object Resolve(IContainer container)
        {
            if (_instance == null)
            {
                _instance = container.Construct(_concrete);
            }

            return _instance;
        }
    }
}