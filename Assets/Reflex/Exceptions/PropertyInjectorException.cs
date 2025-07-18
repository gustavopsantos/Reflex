using System;
using System.Reflection;

namespace Reflex.Exceptions
{
    internal sealed class PropertyInjectorException : Exception
    {
        public PropertyInjectorException(PropertyInfo property, Exception e) : base(BuildMessage(property, e))
        {
        }
        
        private static string BuildMessage(PropertyInfo property, Exception e)
        {
            var propertyDescription = $"'{property.DeclaringType.Name}.{property.Name}'";
            return $"Could not inject property {propertyDescription}, exception: {e}";
        }
    }
}