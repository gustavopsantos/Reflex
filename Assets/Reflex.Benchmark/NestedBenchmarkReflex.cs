using Reflex.Core;
using Reflex.Microsoft.Benchmark.NestedModel;
using Reflex.Microsoft.Benchmark.Utilities;

namespace Reflex.Microsoft.Benchmark
{
	internal class NestedBenchmarkReflex : MonoProfiler
	{
		private Container _container;

		protected override int Order => 0;

		protected override void Sample()
		{
			_container.Single<IA>();
		}

		private void Start()
		{
			_container = new ContainerDescriptor("")
				.AddTransient(typeof(A), typeof(IA))
				.AddTransient(typeof(B), typeof(IB))
				.AddTransient(typeof(C), typeof(IC))
				.AddTransient(typeof(D), typeof(ID))
				.AddTransient(typeof(E), typeof(IE))
				.Build();
		}
	}
}