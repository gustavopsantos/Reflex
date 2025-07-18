using System;
using System.Reflection;

namespace Reflex.Exceptions
{
    internal sealed class FieldInjectorException : Exception
    {
        public FieldInjectorException(FieldInfo field, Exception e) : base(BuildMessage(field, e))
        {
        }
        
        private static string BuildMessage(FieldInfo field, Exception e)
        {
            var fieldDescription = $"'{field.DeclaringType.Name}.{field.Name}'";
            return $"Could not inject field {fieldDescription}, exception: {e}";
        }
    }
}