using Reflex.Delegates;

namespace Reflex.Caching
{
    internal sealed class TypeConstructionInfo
    {
        public readonly ObjectActivator ObjectActivator;
        public readonly MethodParamInfo[] ConstructorParameterData;

        public TypeConstructionInfo(ObjectActivator objectActivator, MethodParamInfo[] constructorParameterData)
        {
            ObjectActivator = objectActivator;
            ConstructorParameterData = constructorParameterData;
        }
    }
}