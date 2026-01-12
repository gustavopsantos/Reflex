using System;

namespace Reflex.Caching
{
    internal readonly struct MethodParamInfo
    {
        public readonly Type ParameterType;
        public readonly bool HasDefaultValue;
        public readonly object DefaultValue;
        
        public MethodParamInfo(Type parameterType, bool hasDefaultValue, object defaultValue) : this()
        {
            ParameterType = parameterType;
            HasDefaultValue = hasDefaultValue;
            DefaultValue = defaultValue;
        }

        public MethodParamInfo(Type parameterType)
        {
            ParameterType = parameterType;
            HasDefaultValue = false;
            DefaultValue = null;
        }
    }
}