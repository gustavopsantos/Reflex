using System;
using System.Linq;
using Reflex.Caching;
using Reflex.Extensions;

namespace Reflex.Exceptions
{
    internal sealed class ConstructorInjectorException : Exception
    {
        public ConstructorInjectorException(Type type, Exception exception, MemberParamInfo[] constructorParameters) : base(BuildMessage(type, exception, constructorParameters))
        {
        }

        private static string BuildMessage(Type type, Exception exception, MemberParamInfo[] constructorParameters)
        {
            var constructorSignature = $"{type.Name} ({string.Join(", ", constructorParameters.Select(t => t.ParameterType.Name))})";
            return $"{exception.Message} occurred while instantiating object type '{type.GetFullName()}' using constructor {constructorSignature}";
        }
    }
}