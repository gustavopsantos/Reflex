using System;
using NUnit.Framework;
using FluentAssertions;

namespace Reflex.Tests
{
	public class ContainerTests
	{
		private interface IValuable
		{
			int Value { get; set; }
		}

		private class Valuable : IValuable
		{
			public int Value { get; set; }
		}

		private interface IClassWithDependency
		{
		}

		private class ClassWithDependency : IClassWithDependency
		{
			private readonly IValuable _valuable;

			public ClassWithDependency(IValuable valuable)
			{
				_valuable = valuable;
			}
		}

		[Test]
		public void Resolve_ValueTypeSingleton_ShouldReturn42()
		{
			Container container = new Container(string.Empty);
			container.BindInstance(42);
			container.Resolve<int>().Should().Be(42);
		}

		[Test]
		public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
		{
			Container container = new Container(string.Empty);
			Action resolve = () => container.Resolve<int>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_AsTransient_ShouldReturnAlwaysANewInstance()
		{
			Container container = new Container(string.Empty);
			container.BindTransient<IValuable, Valuable>();
			container.Resolve<IValuable>().Value = 123;
			container.Resolve<IValuable>().Value.Should().Be(default(int));
		}

		[Test]
		public void Resolve_AsSingleton_ShouldReturnAlwaysSameInstance()
		{
			Container container = new Container(string.Empty);
			container.BindSingleton<IValuable, Valuable>();
			container.Resolve<IValuable>().Value = 123;
			container.Resolve<IValuable>().Value.Should().Be(123);
		}

		[Test]
		public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container(string.Empty);
			Action resolve = () => container.Resolve<IValuable>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container(string.Empty);
			container.BindTransient<IClassWithDependency, ClassWithDependency>();
			Action resolve = () => container.Resolve<IClassWithDependency>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container(string.Empty);
			container.BindSingleton<IClassWithDependency, ClassWithDependency>();
			Action resolve = () => container.Resolve<IClassWithDependency>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_FromMethod_ShouldExecuteBindedMethod()
		{
			Container container = new Container(string.Empty);
			container.BindFunction<IValuable>(() => new Valuable {Value = 42});
			container.Resolve<IValuable>().Value.Should().Be(42);
		}

		[Test]
		public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
		{
			Container container = new Container(string.Empty);
			container.BindTransient<int, int>();
			container.Resolve<int>().Should().Be(default);
		}

		[Test]
		public void Resolve_StringAsTransient_ShouldReturnDefault()
		{
			Container container = new Container(string.Empty);
			container.BindTransient<string, string>();
			container.Resolve<string>().Should().Be(default);
		}
		
		private struct MyStruct
		{
			public readonly int Value;

			public MyStruct(int value)
			{
				this.Value = value;
			}
		}
        
		[Test]
		public void Resolve_ValueTypeAsTransient_CustomConstructor_ValueShouldReturn42()
		{
			Container container = new Container(string.Empty);
			container.BindInstance(42);
			container.BindTransient<MyStruct, MyStruct>();
			container.Resolve<MyStruct>().Value.Should().Be(42);
		}

		private interface ISetup<T>
		{
			void Setup(ref T instance);
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
		
		[Test]
		public void Resolve_ClassWithGenericDependency_WithNormalDefinition_ValuesShouldBe42AndABC()
		{
			Container container = new Container(string.Empty);
			container.BindTransient<Xing, Xing>();
			container.BindTransient<ISetup<int>, IntSetup>();
			container.BindTransient<ISetup<string>, StringSetup>();
			var instance = container.Construct<Xing>();
			instance.Int.Should().Be(42);
			instance.String.Should().Be("abc");
		}
		
		private class SomeSingleton
		{
			public static bool ConstructorCalled;
			
			public SomeSingleton()
			{
				ConstructorCalled = true;
			}
		}
		
		[Test]
		public void Bind_LazySingleton_ThenInvokeInstantiateNonLazySingletons_ShouldNotRunConstructor()
		{
			Container container = new Container(string.Empty);
			SomeSingleton.ConstructorCalled = false;
			container.BindSingleton<SomeSingleton, SomeSingleton>();
			SomeSingleton.ConstructorCalled.Should().BeFalse();
		}
	}
}