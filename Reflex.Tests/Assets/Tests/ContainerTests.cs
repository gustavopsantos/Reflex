using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

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

		private interface IFoo<T>
		{
		}

		private class StringFoo : IFoo<string>
		{
		}

		private class ObjectFoo : IFoo<object>
		{
		}

		private abstract class Pair<T1, T2> : IPair
		{
			public Type[] Types
			{
				get { return new[] {typeof(T1), typeof(T2)}; }
			}
		}

		private interface IPair
		{
			Type[] Types { get; }
		}

		private class IntStringPair : Pair<int, string>
		{
		}

		[Test]
		public void Resolve_ValueTypeSingleton_ShouldReturn42()
		{
			Container container = new Container();
			container.BindSingleton(42);
			container.Resolve<int>().Should().Be(42);
		}

		[Test]
		public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
		{
			Container container = new Container();
			Action resolve = () => container.Resolve<int>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_SimpleBindWithoutScopeDefinition_ShouldThrowScopeNotHandledException()
		{
			Container container = new Container();
			container.Bind<IValuable>().To<Valuable>();
			Action resolve = () => container.Resolve<IValuable>();
			resolve.Should().Throw<ScopeNotHandledException>();
		}

		[Test]
		public void Resolve_GenericBindWithoutScopeDefinition_ShouldThrowScopeNotHandledException()
		{
			Container container = new Container();
			container.BindGenericContract(typeof(IFoo<>)).To(typeof(StringFoo));
			Action resolve = () => container.ResolveGenericContract<object>(typeof(IFoo<>), typeof(string));
			resolve.Should().Throw<ScopeNotHandledException>();
		}

		[Test]
		public void Resolve_GenericBindWithMultipleTypes_ShouldNotThrow()
		{
			Container container = new Container();
			container.BindGenericContract(typeof(Pair<,>)).To(typeof(IntStringPair)).AsTransient();
			Action resolve = () => container.ResolveGenericContract<object>(typeof(Pair<,>), typeof(int), typeof(string));
			resolve.Should().NotThrow();
		}

		[Test]
		public void Resolve_GenericBindWithMultipleTypes_ShouldReturnCorrectBinding()
		{
			Container container = new Container();
			container.BindGenericContract(typeof(Pair<,>)).To(typeof(IntStringPair)).AsTransient();
			var pair = container.ResolveGenericContract<IPair>(typeof(Pair<,>), typeof(int), typeof(string));
			pair.Types[0].Should().Be(typeof(int));
			pair.Types[1].Should().Be(typeof(string));
		}

		[Test]
		public void Resolve_AsTransient_ShouldReturnAlwaysANewInstance()
		{
			Container container = new Container();

			container.Bind<IValuable>().To<Valuable>().AsTransient();

			container.Resolve<IValuable>().Value = 123;
			container.Resolve<IValuable>().Value.Should().Be(default(int));
		}

		[Test]
		public void Resolve_AsSingleton_ShouldReturnAlwaysSameInstance()
		{
			Container container = new Container();
			container.Bind<IValuable>().To<Valuable>().AsSingletonLazy();
			container.Resolve<IValuable>().Value = 123;
			container.Resolve<IValuable>().Value.Should().Be(123);
		}

		[Test]
		public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container();
			Action resolve = () => container.Resolve<IValuable>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container();
			container.Bind<IClassWithDependency>().To<ClassWithDependency>().AsTransient();
			Action resolve = () => container.Resolve<IClassWithDependency>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowUnknownContractException()
		{
			Container container = new Container();
			container.Bind<IClassWithDependency>().To<ClassWithDependency>().AsSingletonLazy();
			Action resolve = () => container.Resolve<IClassWithDependency>();
			resolve.Should().Throw<UnknownContractException>();
		}

		[Test]
		public void Resolve_FromMethod_ShouldExecuteBindedMethod()
		{
			Container container = new Container();
			container.Bind<IValuable>().FromMethod(() => new Valuable {Value = 42});
			container.Resolve<IValuable>().Value.Should().Be(42);
		}

		[Test]
		public void Resolve_GenericTypeOfString_ShouldReturnImplementationWithStringAsGenericTypeArgument()
		{
			Container container = new Container();
			container.BindGenericContract(typeof(IFoo<>)).To(typeof(StringFoo)).AsSingletonLazy();
			var foo = container.ResolveGenericContract<object>(typeof(IFoo<>), typeof(string));
			foo.GetType().GetInterfaces().First().GenericTypeArguments.First().Should().Be(typeof(string));
		}

		[Test]
		public void Resolve_GenericTypeOfObject_ShouldReturnImplementationWithObjectAsGenericTypeArgument()
		{
			Container container = new Container();
			container.BindGenericContract(typeof(IFoo<>)).To(typeof(ObjectFoo)).AsSingletonLazy();
			var foo = container.ResolveGenericContract<object>(typeof(IFoo<>), typeof(object));
			foo.GetType().GetInterfaces().First().GenericTypeArguments.First().Should().Be(typeof(object));
		}

		[Test]
		public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
		{
			Container container = new Container();
			container.Bind<int>().To<int>().AsTransient();
			container.Resolve<int>().Should().Be(default);
		}

		[Test]
		public void Resolve_StringAsTransient_ShouldReturnDefault()
		{
			Container container = new Container();
			container.Bind<string>().To<string>().AsTransient();
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
			Container container = new Container();
			container.BindSingleton(42);
			container.Bind<MyStruct>().To<MyStruct>().AsTransient();
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
			Container container = new Container();
			container.Bind<Xing>().To<Xing>().AsTransient();
			container.Bind<ISetup<int>>().To<IntSetup>().AsTransient();
			container.Bind<ISetup<string>>().To<StringSetup>().AsTransient();
			var instance = container.Construct<Xing>();
			instance.Int.Should().Be(42);
			instance.String.Should().Be("abc");
		}
		
		[Test]
		public void Resolve_ClassWithGenericDependency_WithMergedDefinition_ValuesShouldBe42AndABC()
		{
			Container container = new Container();
			container.Bind<Xing>().To<Xing>().AsTransient();
			container.BindGenericContract(typeof(ISetup<>)).To(typeof(IntSetup), typeof(StringSetup)).AsTransient();
			var instance = container.Construct<Xing>();
			instance.Int.Should().Be(42);
			instance.String.Should().Be("abc");
		}


		internal class ConstructorCalledException : Exception
		{
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
		public void Bind_LazySingleton_ShouldNotRunConstructor()
		{
			Container container = new Container();
			SomeSingleton.ConstructorCalled = false;
			container.Bind<SomeSingleton>().To<SomeSingleton>().AsSingletonLazy();
			SomeSingleton.ConstructorCalled.Should().BeFalse();
		}
		
		[Test]
		public void Bind_NonLazySingleton_ShouldRunConstructor()
		{
			Container container = new Container();
			SomeSingleton.ConstructorCalled = false;
			container.Bind<SomeSingleton>().To<SomeSingleton>().AsSingletonNonLazy();
			SomeSingleton.ConstructorCalled.Should().BeTrue();
		}
	}
}