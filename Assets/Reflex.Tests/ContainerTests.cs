using System;
using System.Data;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Reflex.Microsoft.Tests
{
	public interface IStartable
	{
		void Start();
	}

	internal class ContainerTests
	{
		private interface IValuable
		{
			int Value { get; set; }
		}

		private interface IClassWithDependency
		{
		}

		private interface ISetup<T>
		{
			void Setup(ref T instance);
		}

		[Test]
		public void Resolve_ValueTypeSingleton_ShouldReturn42()
		{
			var container = new ServiceCollection()
				.AddSingleton<IValuable>(serviceProvider => new Valuable() { Value = 42 })
				.BuildServiceProvider();

			container.GetService<IValuable>().Value.Should().Be(42);
		}

		[Test]
		public void Resolve_ValueTypeSingleton_ShouldThrow()
		{
			var container = new ServiceCollection()
				.AddSingleton<IValuable>(serviceProvider => new Valuable() { Value = 42 })
				.Remove<IValuable>()
				.BuildServiceProvider();

			Action resolve = () => container.GetRequiredService<IValuable>();

			resolve.Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
		{
			var container = new ServiceCollection()
				.BuildServiceProvider();
			Action resolve = () => container.GetRequiredService<IValuable>();
			resolve.Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolve_AsTransient_ShouldReturnAlwaysANewInstance()
		{
			var container = new ServiceCollection()
				.AddTransient<IValuable, Valuable>()
				.BuildServiceProvider();

			container.GetService<IValuable>().Value = 123;
			container.GetService<IValuable>().Value.Should().Be(default);
		}

		[Test]
		public void Resolve_AsSingleton_ShouldReturnAlwaysSameInstance()
		{
			var container = new ServiceCollection()
				.AddSingleton<IValuable, Valuable>()
				.BuildServiceProvider();

			container.GetService<IValuable>().Value = 123;
			container.GetService<IValuable>().Value.Should().Be(123);
		}

		[Test]
		public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
		{
			var container = new ServiceCollection()
				.BuildServiceProvider();
			Action resolve = () => container.GetRequiredService<IValuable>();
			resolve.Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			var container = new ServiceCollection()
				.AddSingleton<IClassWithDependency, ClassWithDependency>()
				.BuildServiceProvider();

			Action resolve = () => container.GetService<IClassWithDependency>();
			resolve.Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			var container = new ServiceCollection()
				.AddSingleton<IClassWithDependency, ClassWithDependency>()
				.BuildServiceProvider();

			Action resolve = () => container.GetService<IClassWithDependency>();
			resolve.Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolve_ClassWithGenericDependency_WithNormalDefinition_ValuesShouldBe42AndABC()
		{
			var container = new ServiceCollection()
				.AddTransient<Xing>()
				.AddTransient<ISetup<int>, IntSetup>()
				.AddTransient<ISetup<string>, StringSetup>()
				.BuildServiceProvider();

			var instance = container.GetService<Xing>();
			instance.Int.Should().Be(42);
			instance.String.Should().Be("abc");
		}

		[Test]
		public void Bind_LazySingleton_ThenInvokeInstantiateNonLazySingletons_ShouldNotRunConstructor()
		{
			new ServiceCollection()
				.AddSingleton<SomeSingleton>()
				.BuildServiceProvider();
			SomeSingleton.ConstructorCalled = false;
			SomeSingleton.ConstructorCalled.Should().BeFalse();
		}

		[Test]
		public void AddInstance_WithoutContract_ShouldBindToItsType()
		{
			var container = new ServiceCollection()
				.AddSingleton<IValuable>(serviceProvider => new Valuable() { Value = 42 })
				.BuildServiceProvider();

			container.GetService<IValuable>().Value.Should().Be(42);
		}

		[Test]
		public void ResolveAll_WithoutMatch_ShouldReturnEmptyEnumerable()
		{
			var container = new ServiceCollection().BuildServiceProvider();
			container.GetServices<IDisposable>().Should().BeEmpty();
		}

		private class Valuable : IValuable
		{
			public int Value { get; set; }
		}

		private class ClassWithDependency : IClassWithDependency
		{
			private readonly IValuable _valuable;

			public ClassWithDependency(IValuable valuable)
			{
				_valuable = valuable;
			}
		}

		private class IntSetup : ISetup<int>
		{
			public void Setup(ref int instance)
			{
				instance = 42;
			}
		}

		private class StringSetup : ISetup<string>
		{
			public void Setup(ref string instance)
			{
				instance = "abc";
			}
		}

		private class Xing
		{
			public int Int;
			public string String;

			public Xing(ISetup<int> intSetup, ISetup<string> stringSetup)
			{
				Int = default;
				String = string.Empty;
				intSetup.Setup(ref Int);
				stringSetup.Setup(ref String);
			}
		}

		private class SomeSingleton
		{
			public static bool ConstructorCalled;

			public SomeSingleton()
			{
				ConstructorCalled = true;
			}
		}
	}
}

public static class ServiceCollectionExtensions
{
	public static IServiceCollection Remove<T>(this IServiceCollection services)
	{
		if (services.IsReadOnly)
		{
			throw new ReadOnlyException($"{nameof(services)} is read only");
		}

		var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
		if (serviceDescriptor != null) services.Remove(serviceDescriptor);

		return services;
	}
}