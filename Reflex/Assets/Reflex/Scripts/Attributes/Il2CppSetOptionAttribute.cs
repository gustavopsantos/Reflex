using System;

//do not change namespace. IL2CPP compiler will not see it
namespace Unity.IL2CPP.CompilerServices 
{
    internal enum Option
    {
        NullChecks = 1,
        ArrayBoundsChecks = 2,
        DivideByZeroChecks = 3
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    internal class Il2CppSetOptionAttribute : Attribute
    {
        public Il2CppSetOptionAttribute(Option option, object value)
        {
            Option = option;
            Value  = value;
        }

        public Option Option { get; }
        public object Value  { get; }
    }
}