using UnityEngine;
using UnityEngine.Profiling;

namespace Benchmark.Utilities
{
    public abstract class MonoProfiler : MonoBehaviour
    {
        [SerializeField] public int _iterations;
        [SerializeField] public string _identifier;

        protected abstract void Sample();

        private void Update()
        {
            Profiler.BeginSample(_identifier);
            for (int i = 0; i < _iterations; i++) Sample();
            Profiler.EndSample();
        }
    }
}