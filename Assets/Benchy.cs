using Reflex.Core;
using UnityEngine;
using UnityEngine.Profiling;
using System.Buffers;
using Reflex.Attributes;
using Reflex.Injectors;

public class Benchy : MonoBehaviour
{
    [SerializeField] private int _iterations = 10000;
    
    private readonly Person _person = new();
    [Inject] private readonly Container _container;

    private void Update()
    {
        Profiler.BeginSample("ReflexArrayPoolPersonInjectedByMethod");
        for (int i = 0; i < _iterations; i++)
        {
            AttributeInjector.Inject(_person, _container);
        }
        Profiler.EndSample();
        
        Profiler.BeginSample("ReflexArrayPoolPersonInjectedByConstructor");
        for (int i = 0; i < _iterations; i++)
        {
            var person = ConstructorInjector.Construct(typeof(Person), _container);
        }
        Profiler.EndSample();
        
        Profiler.BeginSample("ReflexArrayPoolPersonFullyInjected");
        for (int i = 0; i < _iterations; i++)
        {
            var person = ConstructorInjector.Construct(typeof(Person), _container);
            AttributeInjector.Inject(person, _container);
        }
        Profiler.EndSample();
        
        // Profiler.BeginSample("ReflexArrayPool");
        // for (int i = 0; i < _iterations; i++)
        // {
        //     var rented = ExactArrayPool<object>.Shared.Rent(16);
        //     ExactArrayPool<object>.Shared.Return(rented);
        // }
        // Profiler.EndSample();
        //
        // Profiler.BeginSample("DotnetArrayPool");
        // for (int i = 0; i < _iterations; i++)
        // {
        //     var rented = ArrayPool<object>.Shared.Rent(16);
        //     ArrayPool<object>.Shared.Return(rented);
        // }
        // Profiler.EndSample();
    }
}
