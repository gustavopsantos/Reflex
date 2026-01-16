using System;

namespace Reflex.Caching
{
    internal readonly struct MemberParamInfo
    {
        public readonly Type ParameterType;
        public readonly bool HasDefaultValue;
        public readonly object DefaultValue;
        
        public MemberParamInfo(Type parameterType, bool hasDefaultValue, object defaultValue) : this()
        {
            ParameterType = parameterType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
        }

        public MemberParamInfo(Type parameterType)
        {
            ParameterType = parameterType;
            HasDefaultValue = false;
            DefaultValue = null;
        }
    }
}