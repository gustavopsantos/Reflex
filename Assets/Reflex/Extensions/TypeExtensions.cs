using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflex.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsEnumerable(this Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GenericTypeArguments.Single();
                return true;
            }

            elementType = default;
            return false;
        }
        
        internal static bool TryGetConstructors(this Type type, out ConstructorInfo[] constructors)
        {
            constructors = type.GetConstructors();
            return constructors.Length > 0;
        }
        
        internal static string GetName(this Type type)
        {
            if (type.IsGenericType)
            {
                var outerTypeName = type.Name!.Split('`').First();
                var innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
                return $"{outerTypeName}<{innerTypeNames}>";
            }

            return type.Name;
        }
        
        internal static string GetFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                var outerTypeName = type.FullName!.Split('`').First();
                var innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
                return $"{outerTypeName}<{innerTypeNames}>";
            }

            return type.FullName;
        }
    }
}