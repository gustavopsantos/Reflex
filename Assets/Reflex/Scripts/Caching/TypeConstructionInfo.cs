using System;
using Reflex.Scripts.Delegates;

namespace Reflex.Scripts.Caching
{
    internal class TypeConstructionInfo
    {
        public readonly ObjectActivator ObjectActivator;
        public readonly Type[] ConstructorParameters;

        public TypeConstructionInfo(ObjectActivator objectActivator, Type[] constructorParameters)
        {
            ObjectActivator = objectActivator;
            ConstructorParameters = constructorParameters;
        }
    }
}