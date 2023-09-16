using UnityEngine;
using UnityEngine.Profiling;
using Reflex.Microsoft.Attributes;
using Reflex.Microsoft.Injectors;
using System;

public class Benchy : MonoBehaviour
{
    [SerializeField] private int _iterations = 10000;
    
    private readonly Person _person = new();
    [Inject] private readonly IServiceProvider _serviceProvider;

    private void Update()
    {
        Profiler.BeginSample("ReflexArrayPoolPersonInjectedByMethod");
        for (int i = 0; i < _iterations; i++)
        {
            AttributeInjector.Inject(_person, _serviceProvider);
        }
        Profiler.EndSample();
        
        Profiler.BeginSample("ReflexArrayPoolPersonInjectedByConstructor");
        for (int i = 0; i < _iterations; i++)
        {
			_ = ConstructorInjector.Construct(typeof(Person), _serviceProvider);
		}
        Profiler.EndSample();
        
        Profiler.BeginSample("ReflexArrayPoolPersonFullyInjected");
        for (int i = 0; i < _iterations; i++)
        {
			object person = ConstructorInjector.Construct(typeof(Person), _serviceProvider);
            AttributeInjector.Inject(person, _serviceProvider);
        }
        Profiler.EndSample();
    }
}
