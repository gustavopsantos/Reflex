using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Reflex.Tests
{
	internal class ContainerDescriptorTests
	{
		private interface IValuable
		{
			int Value { get; set; }
		}

		[Test]
		public void AddInstance_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
		{
			ServiceCollection builder = new ServiceCollection();
			Action addInstance = () => builder.AddSingleton<IValuable>(serviceProvider => new Valuable());
			addInstance.Should().NotThrow();
		}

		[Test]
		public void AddTransient_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
		{
			ServiceCollection builder = new ServiceCollection();
			Action addInstance = () => builder.AddTransient<IValuable, Valuable>();
			addInstance.Should().NotThrow();
		}

		[Test]
		public void AddSingleton_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
		{
			ServiceCollection builder = new ServiceCollection();
			Action addInstance = () => builder.AddSingleton<IValuable, Valuable>();
			addInstance.Should().NotThrow();
		}

		private class Valuable : IValuable
		{
			public int Value { get; set; }
		}
	}
}