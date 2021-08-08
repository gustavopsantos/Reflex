namespace Reflex
{
    public class BindingScopeDefinition
    {
        private readonly Binding[] _bindings;

        public BindingScopeDefinition(params Binding[] bindings)
        {
            _bindings = bindings;
        }


        public void AsTransient()
        {
            foreach (var binding in _bindings)
            {
                binding.Scope = BindingScope.Transient;
            }
        }

        public void AsSingleton()
        {
            foreach (var binding in _bindings)
            {
                binding.Scope = BindingScope.Singleton;
            }
        }
    }
}