using System;
using Microsoft.Extensions.DependencyInjection;
using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;

namespace Reflex.Benchmark
{
    internal class NestedBenchmarkReflex : MonoProfiler
    {
        private IServiceProvider _serviceProvider;

        private void Start()
        {
            _serviceProvider = new ServiceCollection()
                .AddTransient<IA,A>()
                .AddTransient<IB,B>()
                .AddTransient<IC,C>()
                .AddTransient<ID,D>()
                .AddTransient<IE,E>()
                .BuildServiceProvider();
        }

        protected override int Order => 0;

        protected override void Sample()
        {
            _serviceProvider.GetService<IA>();
        }
    }
}