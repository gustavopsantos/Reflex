using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Reflex.Microsoft.Benchmark.Utilities
{
	public abstract class MonoProfiler : MonoBehaviour
	{
		private const int _sampleCount = 32;
		
		[SerializeField] private string _identifier;
		[SerializeField, Min(1)] private int _iterations;
		
		private Rect _area;
		private readonly Stopwatch _stopwatch = new();
		private readonly RingBuffer<long> _samples = new(_sampleCount);
		private readonly Lazy<GUIStyle> _style = new(() => new GUIStyle("label")
		{
			fontSize = 48,
			alignment = TextAnchor.MiddleCenter
		});

		protected abstract int Order { get; }

		protected abstract void Sample();

		private void Awake()
		{
			float height = (float) Screen.height / 4;
			_area = new Rect(0, Order * height, Screen.width, height);
		}

		private void Update()
		{
			_stopwatch.Restart();
			Profiler.BeginSample(_identifier);
			for (int i = 0; i < _iterations; i++)
			{
				Sample();
			}

			Profiler.EndSample();
			_stopwatch.Stop();
			_samples.Push(_stopwatch.ElapsedMilliseconds);
		}

		private void OnGUI()
		{
			GUI.Label(_area, $"{_identifier}: {Average(_samples)}", _style.Value);
		}

		private static long Average(RingBuffer<long> buffer)
		{
			long total = 0;

			for (int i = 0; i < buffer.Length; i++)
			{
				total += buffer[i];
			}

			return total / buffer.Length;
		}
	}
}