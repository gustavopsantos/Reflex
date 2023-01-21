using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Scripts.Reflectors;

namespace Reflex.Scripts.Caching
{
    internal static class TypeConstructionInfoCache
    {
        private static readonly Dictionary<Type, TypeConstructionInfo> _dictionary = new Dictionary<Type, TypeConstructionInfo>();

        internal static TypeConstructionInfo Get(Type type)
        {
            if (_dictionary.TryGetValue(type, out var info))
            {
                return info;
            }

            info = Generate(type);
            _dictionary.Add(type, info);
            return info;
        }
        
        private static TypeConstructionInfo Generate(Type type)
        {
            if (type.TryGetConstructors(out var constructors))
            {
                var constructor = constructors.MaxBy(ctor => ctor.GetParameters().Length);
                var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
                return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateActivator(type, constructor, parameters), parameters);
            }

            // Should we add this complexity yo be able to inject value types?
            return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateDefaultActivator(type), Type.EmptyTypes);
        }
    }
}