using System;
using System.Linq;
using System.Reflection;

namespace Reflex
{
    internal static class TypeExtensions
    {
        internal static bool TryGetConstructors(this Type type, out ConstructorInfo[] constructors)
        {
            constructors = type.GetConstructors();
            return constructors.Length > 0;
        }
        
        internal static string GetFormattedName(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetFormattedName));
                return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}<{genericArguments}>";
            }

            return type.Name;
        }
    }
}