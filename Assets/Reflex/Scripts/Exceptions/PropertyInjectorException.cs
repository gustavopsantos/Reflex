using System;

namespace Reflex
{
    internal sealed class PropertyInjectorException : Exception
    {
        public PropertyInjectorException(Exception e) : base(e.Message)
        {
        }
    }
}