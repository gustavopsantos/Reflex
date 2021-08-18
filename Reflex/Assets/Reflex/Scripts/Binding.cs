using System;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal struct Binding
    {
        //4 bytes value on x64 (instance)
        internal int ConcreteHashCode;
        //1 byte value on x64 (instance)
        internal BindingScope Scope;
        //4 bytes ptr on x64 (instance)
        internal Func<object> Method;
    }
}