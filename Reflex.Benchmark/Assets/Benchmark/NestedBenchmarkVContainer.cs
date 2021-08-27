using Benchmark.NestedModel;
using Benchmark.Utilities;
using UnityEngine;
using VContainer;

namespace Benchmark
{
    public class NestedBenchmarkVContainer : MonoProfiler
    {
        private readonly ContainerBuilder _containerBuilder = new ContainerBuilder();
        private IObjectResolver _objectResolver;

        private static readonly InitOnlyProperty<Shader> StandardShader = new InitOnlyProperty<Shader>(() => Shader.Find("Standard"));
        
        protected override int Order => 1;

        private void Start()
        {
            Debug.Log(StandardShader.Value.name);
            _containerBuilder.Register<IA, A>(Lifetime.Transient);
            _containerBuilder.Register<IB, B>(Lifetime.Transient);
            _containerBuilder.Register<IC, C>(Lifetime.Transient);
            _containerBuilder.Register<ID, D>(Lifetime.Transient);
            _containerBuilder.Register<IE, E>(Lifetime.Transient);
            _objectResolver = _containerBuilder.Build();
        }

        protected override void Sample()
        {
            _objectResolver.Resolve<IA>();
        }
    }
}