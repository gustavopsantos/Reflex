using System;
using System.Collections.Generic;
using System.Reflection;
using Reflex.Attributes;
using UnityEngine.Pool;

namespace Reflex.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, TypeAttributeInfo> _cache = new();
        
        internal static TypeAttributeInfo Get(Type type)
        {
            if (!_cache.TryGetValue(type, out var info))
            {
                info = Generate(type);
                _cache.Add(type, info);
            }
    
            return info;
        }

        internal static TypeAttributeInfo Generate(Type type)
        {
            using var pooled1 = ListPool<FieldInfo>.Get(out var fieldList);
            using var pooled2 = ListPool<PropertyInfo>.Get(out var propertyList);
            using var pooled3 = ListPool<InjectableMethodInfo>.Get(out var methodList);
            
            while (type != null && type != typeof(object))
            {
                foreach (var field in type.GetFields(Flags))
                {
                    if (field.IsDefined(typeof(InjectAttribute)))
                    {
                        fieldList.Add(field);
                    }
                }

                foreach (var property in type.GetProperties(Flags))
                {
                    if (property.CanWrite && property.IsDefined(typeof(InjectAttribute)))
                    {
                        propertyList.Add(property);
                    }
                }
                
                foreach (var method in type.GetMethods(Flags))
                {
                    if (method.IsDefined(typeof(InjectAttribute)))
                    {
                        methodList.Add(new InjectableMethodInfo(method));
                    }
                }

                type = type.BaseType;
            }

            return new TypeAttributeInfo(fieldList.ToArray(), propertyList.ToArray(), methodList.ToArray());
        }
    }
}