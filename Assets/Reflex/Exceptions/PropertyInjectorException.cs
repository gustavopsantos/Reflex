using System;
using System.Reflection;

namespace Reflex.Exceptions
{
    internal sealed class PropertyInjectorException : Exception
    {
        public PropertyInjectorException(PropertyInfo property, Exception innerException) : base(BuildMessage(property, innerException))
        {
        }
        
        private static string BuildMessage(PropertyInfo property, Exception innerException)
        {
            var propertyDescription = $"'{property.DeclaringType.Name}.{property.Name}'";
            return $"Could not inject property {propertyDescription}, inner exception: {innerException}";
        }
    }
}