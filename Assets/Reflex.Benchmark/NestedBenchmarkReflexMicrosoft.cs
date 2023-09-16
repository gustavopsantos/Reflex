using System;
using Microsoft.Extensions.DependencyInjection;
using Reflex.Microsoft.Benchmark.NestedModel;
using Reflex.Microsoft.Benchmark.Utilities;

namespace Reflex.Microsoft.Benchmark
{
	internal class NestedBenchmarkReflexMicrosoft : MonoProfiler
	{
		private IServiceProvider _serviceProvider;

		protected override int Order => 3;

		protected override void Sample()
		{
			_serviceProvider.GetService<IA>();
		}

		private void Start()
		{
			_serviceProvider = new ServiceCollection()
				.AddTransient<IA, A>()
				.AddTransient<IB, B>()
				.AddTransient<IC, C>()
				.AddTransient<ID, D>()
				.AddTransient<IE, E>()
				.BuildServiceProvider();
		}
	}
}