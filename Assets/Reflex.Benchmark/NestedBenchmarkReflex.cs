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
                .AddTransient(typeof(A), typeof(IA))
                .AddTransient(typeof(B), typeof(IB))
                .AddTransient(typeof(C), typeof(IC))
                .AddTransient(typeof(D), typeof(ID))
                .AddTransient(typeof(E), typeof(IE))
                .Build();
        }

        protected override int Order => 0;

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}