namespace Reflex
{
    public class BindingContractDefinition<TContract>
    {
        private readonly Container _container;

        internal BindingContractDefinition(Container container)
        {
            _container = container;
        }

        public BindingScopeDefinition To<TConcrete>() where TConcrete : TContract
        {
            var binding = new Binding
            {
                Contract = typeof(TContract),
                Concrete = typeof(TConcrete)
            };

            _container.Bindings.Add(typeof(TContract), binding);
            return new BindingScopeDefinition(binding);
        }
    }
}