using System;
using Benchmark.NestedModel;
using Benchmark.Utilities;
using Reflex;

namespace Benchmark
{
    public class NestedBenchmarkReflex : MonoProfiler
    {
        private readonly Container _container = new Container(string.Empty);

        private void Start()
        {
            _container.BindTransient<IA, A>();
            _container.BindTransient<IB, B>();
            _container.BindTransient<IC, C>();
            _container.BindTransient<ID, D>();
            _container.BindTransient<IE, E>();
        }

        protected override int Order => 0;

        protected override void Sample()
        {
            _container.Resolve<IA>();
        }
    }
}