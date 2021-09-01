namespace Reflex
{
    public class BindingScopeDefinition
    {
        private readonly Binding[] _bindings;
        private readonly Container _container;

        internal BindingScopeDefinition(Container container, params Binding[] bindings)
        {
            _bindings = bindings;
            _container = container;
        }

        public void AsTransient()
        {
            foreach (var binding in _bindings)
            {
                binding.Scope = BindingScope.Transient;
            }
        }

        public void AsLazySingleton()
        {
            foreach (var binding in _bindings)
            {
                binding.Scope = BindingScope.Singleton;
            }
        }

        public void AsNonLazySingleton()
        {
            AsLazySingleton();

            foreach (var binding in _bindings)
            {
                SingletonResolver.CreateAndRegisterSingletonInstance(binding.Contract, _container);
            }
        }
    }
}