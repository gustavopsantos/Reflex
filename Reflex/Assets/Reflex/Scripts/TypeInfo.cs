using System;

namespace Reflex
{
    internal class TypeInfo
    {
        public Type[] ConstructorParameters { get; }
        public DynamicObjectActivator Activator { get; }

        public TypeInfo(Type[] constructorParameters, DynamicObjectActivator activator)
        {
            Activator = activator;
            ConstructorParameters = constructorParameters;
        }
    }
}