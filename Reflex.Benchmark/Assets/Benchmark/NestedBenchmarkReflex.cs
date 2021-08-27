using Benchmark.NestedModel;
using Benchmark.Utilities;
using Reflex;

namespace Benchmark
{
    public class NestedBenchmarkReflex : MonoProfiler
    {
        private readonly IContainer _container = new Container();

        private void Start()
        {
            _container.Bind<IA>().To<A>().AsTransient();
            _container.Bind<IB>().To<B>().AsTransient();
            _container.Bind<IC>().To<C>().AsTransient();
            _container.Bind<ID>().To<D>().AsTransient();
            _container.Bind<IE>().To<E>().AsTransient();
        }

        protected override int Order => 0;

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}