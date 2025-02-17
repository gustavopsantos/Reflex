using System;
using System.Linq;
using Reflex.Extensions;

namespace Reflex.Exceptions
{
    internal sealed class ConstructorInjectorException : Exception
    {
        public ConstructorInjectorException(Type type, Exception exception, Type[] constructorParameters) : base(BuildMessage(type, exception, constructorParameters))
        {
        }

        private static string BuildMessage(Type type, Exception exception, Type[] constructorParameters)
        {
            var constructorSignature = $"{type.Name} ({string.Join(", ", constructorParameters.Select(t => t.Name))})";
            return $"{exception.Message} occurred while instantiating object type '{type.GetFullName()}' using constructor {constructorSignature}";
        }
    }
}