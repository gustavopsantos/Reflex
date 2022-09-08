using System;
using System.Diagnostics;
using Domain.Generics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Benchmark.Utilities
{
	public abstract class MonoProfiler : MonoBehaviour
	{
		[SerializeField, Min(1)] public int _iterations;
		[SerializeField] public string _identifier;
		private readonly Stopwatch _stopwatch = new Stopwatch();
		private string _ticks;
		private Rect _area;

		private const int SampleCount = 32;

		private readonly RingBuffer<long> _samples = new RingBuffer<long>(SampleCount);

		private readonly Lazy<GUIStyle> Style = new Lazy<GUIStyle>(() => new GUIStyle("label")
		{
			fontSize = 48,
			alignment = TextAnchor.MiddleCenter
		});

		protected abstract int Order { get; }

		protected abstract void Sample();

		private void Awake()
		{
			var height = (float) Screen.height / 3;
			_area = new Rect(0, Order * height, Screen.width, height);
		}

		private void Update()
		{
			_stopwatch.Restart();
			Profiler.BeginSample(_identifier);
			for (int i = 0; i < _iterations; i++) Sample();
			Profiler.EndSample();
			_stopwatch.Stop();
			_samples.Push(_stopwatch.ElapsedMilliseconds);
		}

		private void OnGUI()
		{
			GUI.Label(_area, $"{_identifier}: {Average(_samples)}", Style.Value);
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