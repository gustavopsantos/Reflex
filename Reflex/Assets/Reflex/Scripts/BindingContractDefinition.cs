using System;

namespace Reflex
{
    public class BindingContractDefinition<TContract>
    {
        private readonly RegisterFunction _register;

        private BindingContractDefinition()
        {
        }

        public BindingContractDefinition(RegisterFunction register)
        {
            _register = register;
        }

        public BindingScopeDefinition To<TConcrete>() where TConcrete : TContract
        {
            var binding = new Binding
            {
                Concrete = typeof(TConcrete)
            };

            _register(typeof(TContract), binding);
            return new BindingScopeDefinition(binding);
        }

        public void FromMethod(Func<TContract> method)
        {
            var binding = new Binding
            {
                Scope = BindingScope.Method,
                Method = method as Func<object>
            };

            _register(typeof(TContract), binding);
        }
    }
}