namespace Reflex
{
    public class InstanceResolver : IResolver
    {
        private readonly object _instance;

        public InstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Resolve(Container container)
        {
            return _instance;
        }
    }
}