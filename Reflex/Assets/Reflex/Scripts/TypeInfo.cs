using System;

namespace Reflex
{
    internal struct TypeInfo
    {
        public Type Type;
        public Type[] ConstructorParameters;
        public DynamicObjectActivator Activator;
    }
}