using Reflex.Attributes;
using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;
using Reflex.Core;
using Reflex.Injectors;

namespace Reflex.Benchmark
{
    [SourceGeneratorInjectable]
    public partial class NestedBenchmarkReflexAttributeGeneratorInject : MonoProfiler
    {
        [Inject]
        IA Dependency;

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

        protected override void Sample()
        {
            AttributeInjector.Inject(this, _container);
        }
    }
}