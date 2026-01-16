using Reflex.Delegates;

namespace Reflex.Caching
{
    internal sealed class TypeConstructionInfo
    {
        public readonly ObjectActivator ObjectActivator;
        public readonly MemberParamInfo[] ConstructorParameterData;

        public TypeConstructionInfo(ObjectActivator objectActivator, MemberParamInfo[] constructorParameterData)
        {
            ObjectActivator = objectActivator;
            ConstructorParameterData = constructorParameterData;
        }
    }
}