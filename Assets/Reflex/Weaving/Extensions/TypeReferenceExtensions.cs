using System.Linq;
using Mono.Cecil;

namespace Reflex.Weaving.Extensions
{
    internal static class TypeReferenceExtensions
    {
        internal static bool IsEnumerable(this TypeReference typeReference, out TypeReference elementTypeReference)
        {
            if (typeReference.IsGenericInstance && typeReference.FullName.StartsWith("System.Collections.Generic.IEnumerable`1<"))
            {
                elementTypeReference = ((GenericInstanceType) typeReference).GenericArguments.Single();
                return true;
            }

            elementTypeReference = default;
            return false;
        }
    }
}
