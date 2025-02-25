using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Reflectors;

namespace Reflex.Caching
{
    internal static class TypeConstructionInfoCache
    {
        private static readonly Dictionary<Type, TypeConstructionInfo> _dictionary = new();

        internal static TypeConstructionInfo Get(Type type)
        {
            if (!_dictionary.TryGetValue(type, out var info))
            {
                info = Generate(type);
                _dictionary.Add(type, info);
            }
        
            return info;
        }
        
        private static TypeConstructionInfo Generate(Type type)
        {
            if (type.TryGetConstructors(out var constructors))
            {
                var constructor = constructors.FirstOrDefault(c => Attribute.IsDefined(c, typeof(ReflexConstructorAttribute))); // Try to get a constructor that defines ReflexConstructor

                if (constructor == null)
                {
                    constructor = constructors.MaxBy(ctor => ctor.GetParameters().Length); // Gets the constructor with most arguments
                }
                
                var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
                return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateActivator(type, constructor, parameters), parameters);
            }

            // Should we add this complexity yo be able to inject value types?
            return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateDefaultActivator(type), Type.EmptyTypes);
        }
    }
}