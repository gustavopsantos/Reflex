using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;

namespace Reflex.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly List<FieldInfo> _fields = new();
        private static readonly List<PropertyInfo> _properties = new();
        private static readonly List<MethodInfo> _methods = new();
        private static readonly Dictionary<Type, TypeAttributeInfo> _dictionary = new();
        
        internal static TypeAttributeInfo Get(Type type)
        {
            if (!_dictionary.TryGetValue(type, out var info))
            {
                _fields.Clear();
                _properties.Clear();
                _methods.Clear();
                Generate(type);
                info = new TypeAttributeInfo(_fields.ToArray(), _properties.ToArray(), _methods.ToArray());
                _dictionary.Add(type, info);
            }
    
            return info;
        }
        
        private static void Generate(Type type)
        {
            var fields = type
                .GetFields(Flags)
                .Where(f => f.IsDefined(typeof(InjectAttribute)));

            var properties = type
                .GetProperties(Flags)
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)));

            var methods = type
                .GetMethods(Flags)
                .Where(m => m.IsDefined(typeof(InjectAttribute)));
            
            _fields.AddRange(fields);
            _properties.AddRange(properties);
            _methods.AddRange(methods);

            if (type.BaseType != null)
            {
                Generate(type.BaseType);
            }
        }
    }
}