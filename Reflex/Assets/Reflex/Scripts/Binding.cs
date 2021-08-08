using System;

namespace Reflex
{
    public class Binding
    {
        public Type Concrete;
        public BindingScope Scope;
        public Func<object> Method;
    }
}