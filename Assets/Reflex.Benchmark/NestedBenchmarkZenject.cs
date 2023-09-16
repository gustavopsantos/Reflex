using Reflex.Microsoft.Benchmark.NestedModel;
using Reflex.Microsoft.Benchmark.Utilities;
using Zenject;

namespace Reflex.Microsoft.Benchmark
{
    internal class NestedBenchmarkZenject : MonoProfiler
    {
        private readonly DiContainer _container = new DiContainer();

        protected override int Order => 1;
        
        private void Start()
        {
            _container.Bind<IA>().To<A>().AsTransient();
            _container.Bind<IB>().To<B>().AsTransient();
            _container.Bind<IC>().To<C>().AsTransient();
            _container.Bind<ID>().To<D>().AsTransient();
            _container.Bind<IE>().To<E>().AsTransient();
        }

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}