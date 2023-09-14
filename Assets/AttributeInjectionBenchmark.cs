using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using UnityEngine.Profiling;

//public sealed class AttributeInjectionBenchmark : MonoBehaviour
//{
//    [SerializeField] private int _iterations = 1000;

//    private readonly IServiceProvider _serviceProvider = new ServiceCollection().AddInstance(42).Build();
    
//    private void Update()
//    {
//        Profiler.BeginSample("AttributeInjectionBenchmark");
//        for (int i = 0; i < _iterations; i++)
//        {
//            var service = _serviceProvider.GetService<AttributeInjectedService>();
//        }
//        Profiler.EndSample();
//    }
//}