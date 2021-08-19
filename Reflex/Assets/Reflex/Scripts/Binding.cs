using System;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    //17 bytes, with alignment can be 24 
    //~80 bytes with included types
    internal struct Binding
    {
        //8 bytes value on x64 (instance)
        internal int ConcreteHashCode;
        //1 byte value on x64 (instance)
        internal BindingScope Scope;
        //8 bytes ptr on x64 (instance)
        internal Func<object> Method;
        
        //Heavy memory footprint
        //TODO Maybe replace FromMethod with FromInterface?
        //Func<>
        //8 bytes object header
        //8 bytes _methodBase
        //8 bytes _methodPtr
        //8 bytes _methodPtrAux
        //8 bytes _target
        //8 bytes _invocationCount
        //8 bytes _invocationList
    }
}