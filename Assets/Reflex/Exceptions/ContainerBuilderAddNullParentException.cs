using System;
using Reflex.Core;

namespace Reflex.Exceptions
{
    public class ContainerBuilderAddNullParentException : Exception
    {
        public ContainerBuilderAddNullParentException(ContainerBuilder builder) : base(BuildMessage(builder)) { }

        private static string BuildMessage(ContainerBuilder builder) => 
            $"Null parent was added to builder {builder.Name}";
    }
}