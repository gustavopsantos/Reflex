using System;
using System.Reflection;

namespace Reflex.Exceptions
{
    internal sealed class FieldInjectorException : Exception
    {
        public FieldInjectorException(FieldInfo field, Exception innerException) : base(BuildMessage(field, innerException), innerException)
        {
        }
        
        private static string BuildMessage(FieldInfo field, Exception innerException)
        {
            var fieldDescription = $"'{field.DeclaringType.Name}.{field.Name}'";
            return $"Could not inject field {fieldDescription}, inner exception: {innerException}";
        }
    }
}