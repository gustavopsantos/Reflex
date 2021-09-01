using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using Benchmark.NestedModel;
using UnityEngine.Profiling;

public class Factory : MonoBehaviour
{
	private const int Iterations = 10000;

	private B _b;
	private C _c;
	private D _d;
	private E _e;

	private ConstructorInfo _aConstructorInfo;

	private void Start()
	{
		_e = new E();
		_d = new D(_e);
		_c = new C(_d);
		_b = new B(_c);
		_aConstructorInfo = typeof(A).GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
	}

	private void Update()
	{
		BenchmarkInstantiateNew();
		BenchmarkInstantiateActivator();
		BenchmarkInstantiateUninitializedObject();
	}

	private void BenchmarkInstantiateNew()
	{
		Profiler.BeginSample(nameof(BenchmarkInstantiateNew));
		for (int i = 0; i < Iterations; i++)
		{
			var instance = new A(_b);
		}

		Profiler.EndSample();
	}

	private void BenchmarkInstantiateActivator()
	{
		Profiler.BeginSample(nameof(BenchmarkInstantiateActivator));
		for (int i = 0; i < Iterations; i++)
		{
			var instance = Activator.CreateInstance(typeof(A), _b);
		}

		Profiler.EndSample();
	}

	private void BenchmarkInstantiateUninitializedObject()
	{
		Profiler.BeginSample(nameof(BenchmarkInstantiateUninitializedObject));
		for (int i = 0; i < Iterations; i++)
		{
			var instance = FormatterServices.GetUninitializedObject(typeof(A));
			_aConstructorInfo.Invoke(instance, new object[] {_b});
		}

		Profiler.EndSample();
	}
}