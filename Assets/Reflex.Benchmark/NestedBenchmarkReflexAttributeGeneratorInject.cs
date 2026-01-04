using JetBrains.Annotations;
using Reflex.Attributes;
using Reflex.Benchmark.NestedModel;
using Reflex.Benchmark.Utilities;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Injectors;

namespace Reflex.Benchmark
{
    [SourceGeneratorInjectable]
    public partial class NestedBenchmarkReflexAttributeGeneratorInject : MonoProfiler
    {
        [Inject, UsedImplicitly] private IA _dependency;

        private Container _container;

        private void Start()
        {
            _container = new ContainerBuilder()
                .RegisterType(typeof(A), new []{typeof(IA)}, Lifetime.Transient, Resolution.Lazy )
                .RegisterType(typeof(B), new []{typeof(IB)}, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(C), new []{typeof(IC)}, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(D), new []{typeof(ID)}, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(E), new []{typeof(IE)}, Lifetime.Transient, Resolution.Lazy)
                .Build();
        }

        protected override void Sample()
        {
            AttributeInjector.Inject(this, _container);
        }
    }
}