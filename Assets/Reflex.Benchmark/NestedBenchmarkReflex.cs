using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;
using Reflex.Core;

namespace Reflex.Benchmark
{
    internal class NestedBenchmarkReflex : MonoProfiler
    {
        private Container _container;

        private void Start()
        {
            _container = new ContainerBuilder()
                .Add(Transient.FromType(typeof(A), new[] { typeof(IA) }))
                .Add(Transient.FromType(typeof(B), new[] { typeof(IB) }))
                .Add(Transient.FromType(typeof(C), new[] { typeof(IC) }))
                .Add(Transient.FromType(typeof(D), new[] { typeof(ID) }))
                .Add(Transient.FromType(typeof(E), new[] { typeof(IE) }))
                .Build();
        }

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}