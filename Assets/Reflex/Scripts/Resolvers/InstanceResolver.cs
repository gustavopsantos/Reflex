namespace Reflex
{
    public class InstanceResolver : IResolver
    {
        private readonly object _instance;
        public int Resolutions { get; private set; }

        public InstanceResolver(object instance)
        {
            _instance = instance;
        }

        public object Resolve(Container container)
        {
            Resolutions++;
            return _instance;
        }
    }
}