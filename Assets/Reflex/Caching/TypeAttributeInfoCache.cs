using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;

namespace Reflex.Caching
{
    internal static class TypeAttributeInfoCache
    {
        private static readonly Dictionary<Type, TypeAttributeInfo> _dictionary = new();
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        internal static TypeAttributeInfo Get(Type type)
        {
            if (!_dictionary.TryGetValue(type, out var info))
            {
                info = Generate(type);
                _dictionary.Add(type, info);
            }
    
            return info;
        }
        
        private static TypeAttributeInfo Generate(Type type)
        {
            var fields = type
                .GetFields(Flags)
                .Where(f => f.IsDefined(typeof(InjectAttribute)))
                .ToArray();
            
            var properties = type
                .GetProperties(Flags)
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)))
                .ToArray();
            
            var methods = type
                .GetMethods(Flags)
                .Where(m => m.IsDefined(typeof(InjectAttribute)))
                .ToArray();

            return new TypeAttributeInfo(fields, properties, methods);
        }
    }
}