using System;
using Mono.Cecil;

namespace Reflex.Weaving.Extensions
{
    public static class MethodReferenceExtensions
    {
        public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] args)
        {
            if (args.Length == 0)
            {
                return method;
            }

            if (method.GenericParameters.Count != args.Length)
            {
                throw new ArgumentException("Invalid number of generic typearguments supplied");
            }

            var genericTypeRef = new GenericInstanceMethod(method);
            foreach (var arg in args)
            {
                genericTypeRef.GenericArguments.Add(arg);
            }

            return genericTypeRef;
        }
    }
}