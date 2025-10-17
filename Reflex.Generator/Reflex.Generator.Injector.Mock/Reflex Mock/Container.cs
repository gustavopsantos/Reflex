using System;
using System.Collections.Generic;

namespace Reflex.Core
{
    public class Container
    {
        static Dictionary<Type, object> Dependencies;

        public void AddSingleton<T>(T instance)
        {
            var type = typeof(T);
            Dependencies[type] = instance;
        }

        public TContract Resolve<TContract>()
        {
            var type = typeof(TContract);

            if (Dependencies.TryGetValue(type, out var obj) is false)
                throw new ArgumentException($"No Dependency of Type ({type}) Found");

            return (TContract)obj;
        }

        public Container()
        {
            Dependencies = new Dictionary<Type, object>();
        }
    }
}