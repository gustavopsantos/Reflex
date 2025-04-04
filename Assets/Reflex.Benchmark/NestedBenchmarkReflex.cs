using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Benchmark
{
    internal class NestedBenchmarkReflex : MonoProfiler
    {
        private Container _container;

        private void Start()
        {
            _container = new ContainerBuilder()
                .RegisterType(typeof(A), new[] { typeof(IA) }, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(B), new[] { typeof(IB) }, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(C), new[] { typeof(IC) }, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(D), new[] { typeof(ID) }, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(E), new[] { typeof(IE) }, Lifetime.Transient, Resolution.Lazy)
                .Build();
        }

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}