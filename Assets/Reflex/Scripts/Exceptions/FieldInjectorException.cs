using System;

namespace Reflex
{
    internal sealed class FieldInjectorException : Exception
    {
        public FieldInjectorException(Exception e) : base(e.Message)
        {
        }
    }
}