using Reflex.Core;
using UnityEngine;
using UnityEngine.Profiling;

public sealed class AttributeInjectionBenchmark : MonoBehaviour
{
    [SerializeField] private int _iterations = 1000;

    private readonly Container _container = new ContainerDescriptor("").AddInstance(42).Build();
    
    private void Update()
    {
        Profiler.BeginSample("AttributeInjectionBenchmark");
        for (int i = 0; i < _iterations; i++)
        {
            var service = _container.Construct<AttributeInjectedService>();
        }
        Profiler.EndSample();
    }
}