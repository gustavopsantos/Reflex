using System;

namespace Reflex
{
    internal class Binding
    {
        internal Type Concrete;
        internal BindingScope Scope;
        internal Func<object> Method;
    }
}