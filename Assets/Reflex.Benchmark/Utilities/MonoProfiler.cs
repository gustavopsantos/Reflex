using System;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Reflex.Benchmark.Utilities
{
    public abstract class MonoProfiler : MonoBehaviour
    {
        [SerializeField]
        NestedBenchmarkConfig Config;
        public int _iterations => Config.Iterations;

        private const int SampleCount = 64;

        [SerializeField] public string _identifier;
        [SerializeField] public Text _resultOutput;
        private readonly RingBuffer<long> _samples = new(SampleCount);
        private readonly Dictionary<long, string> _stringPool = new();

        protected abstract void Sample();

        private void Update()
        {
            // Sample and measure
            var before = Stopwatch.GetTimestamp();
            Profiler.BeginSample(_identifier);
            for (var i = 0; i < _iterations; i++)
            {
                Sample();
            }
            Profiler.EndSample();
            var after = Stopwatch.GetTimestamp();
            var elapsedTicks = after - before;
            var elapsedMilliseconds = elapsedTicks / TimeSpan.TicksPerMillisecond;
            _samples.Push(elapsedMilliseconds);

            // Present result
            var average = Average(_samples);
            if (!_stringPool.TryGetValue(average, out var output))
            {
                output = $"{_identifier}: {average}";
                _stringPool.Add(average, output);
            }

            _resultOutput.text = output;
        }

        private static long Average(RingBuffer<long> buffer)
        {
            long total = 0;

            for (var i = 0; i < buffer.Length; i++)
            {
                total += buffer[i];
            }

            return total / buffer.Length;
        }
    }
}