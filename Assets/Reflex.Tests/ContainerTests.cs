//using System;
//using FluentAssertions;
//using Reflex.Core;
//using Reflex.Exceptions;
//using NUnit.Framework;
//using Microsoft.Extensions.DependencyInjection;

//namespace Reflex.Tests
//{
//    internal class ContainerTests
//    {
//        private interface IValuable
//        {
//            int Value { get; set; }
//        }

//        private class Valuable : IValuable
//        {
//            public int Value { get; set; }
//        }

//        private interface IClassWithDependency
//        {
//        }

//        private class ClassWithDependency : IClassWithDependency
//        {
//			private readonly IValuable _valuable;

//			public ClassWithDependency(IValuable valuable)
//            {
//				_valuable = valuable;
//			}
//        }

//        [Test]
//        public void Resolve_ValueTypeSingleton_ShouldReturn42()
//        {
//            var container = new ServiceCollection()
//                .AddInstance(42, typeof(int))
//                .Build();

//            container.GetService<int>().Should().Be(42);
//        }

//		[Test]
//		public void Resolve_ValueTypeSingleton_ShouldThrow()
//		{
//			var container = new ServiceCollection()
//				.AddInstance(42, typeof(int))
//                .RemoveInstance(42, typeof(int))
//				.Build();

//			Action resolve = () => container.GetService<int>();

//            resolve.Should().Throw<UnknownContractException>();
//		}

//		[Test]
//        public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
//        {
//            var container = new ServiceCollection().Build();
//            Action resolve = () => container.GetService<int>();
//            resolve.Should().Throw<UnknownContractException>();
//        }

//        [Test]
//        public void Resolve_AsTransient_ShouldReturnAlwaysANewInstance()
//        {
//            var container = new ServiceCollection()
//                .AddTransient(typeof(Valuable), typeof(IValuable))
//                .Build();

//            container.GetService<IValuable>().Value = 123;
//            container.GetService<IValuable>().Value.Should().Be(default(int));
//        }

//        [Test]
//        public void Resolve_AsSingleton_ShouldReturnAlwaysSameInstance()
//        {
//            var container = new ServiceCollection()
//                .AddSingleton<IValuable, Valuable>()
//                .Build();

//            container.GetService<IValuable>().Value = 123;
//            container.GetService<IValuable>().Value.Should().Be(123);
//        }

//        [Test]
//        public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
//        {
//            var container = new ServiceCollection().Build();
//            Action resolve = () => container.GetService<IValuable>();
//            resolve.Should().Throw<UnknownContractException>();
//        }

//        [Test]
//        public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowUnknownContractException()
//        {
//            var container = new ServiceCollection()
//                .AddTransient(typeof(ClassWithDependency), typeof(IClassWithDependency))
//                .Build();

//            Action resolve = () => container.GetService<IClassWithDependency>();
//            resolve.Should().Throw<UnknownContractException>();
//        }

//        [Test]
//        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowUnknownContractException()
//        {
//            var container = new ServiceCollection()
//                .AddSingleton(typeof(ClassWithDependency), typeof(IClassWithDependency))
//                .Build();

//            Action resolve = () => container.GetService<IClassWithDependency>();
//            resolve.Should().Throw<UnknownContractException>();
//        }

//        [Test]
//        public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
//        {
//            var container = new ServiceCollection()
//                .AddTransient(typeof(int), typeof(int))
//                .Build();

//        	container.GetService<int>().Should().Be(default);
//        }

//        private struct MyStruct
//        {
//            public readonly int Value;

//            public MyStruct(int value)
//            {
//                this.Value = value;
//            }
//        }

//        [Test]
//        public void Resolve_ValueTypeAsTransient_CustomConstructor_ValueShouldReturn42()
//        {
//            var container = new ServiceCollection()
//                .AddInstance(42, typeof(int))
//                .AddTransient(typeof(MyStruct), typeof(MyStruct))
//                .Build();

//            container.GetService<MyStruct>().Value.Should().Be(42);
//        }

//        private interface ISetup<T>
//        {
//            void Setup(ref T instance);
//        }

//        private class IntSetup : ISetup<int>
//        {
//            public void Setup(ref int instance)
//            {
//                instance = 42;
//            }
//        }

//        private class StringSetup : ISetup<string>
//        {
//            public void Setup(ref string instance)
//            {
//                instance = "abc";
//            }
//        }

//        private class Xing
//        {
//            public int Int;
//            public string String;

//            public Xing(ISetup<int> intSetup, ISetup<string> stringSetup)
//            {
//                Int = default;
//                String = string.Empty;
//                intSetup.Setup(ref Int);
//                stringSetup.Setup(ref String);
//            }
//        }

//        [Test]
//        public void Resolve_ClassWithGenericDependency_WithNormalDefinition_ValuesShouldBe42AndABC()
//        {
//            var container = new ServiceCollection()
//                .AddTransient(typeof(Xing), typeof(Xing))
//                .AddTransient(typeof(IntSetup), typeof(ISetup<int>))
//                .AddTransient(typeof(StringSetup), typeof(ISetup<string>))
//                .Build();

//        	var instance = container.Construct<Xing>();
//        	instance.Int.Should().Be(42);
//        	instance.String.Should().Be("abc");
//        }

//        private class SomeSingleton
//        {
//            public static bool ConstructorCalled;

//            public SomeSingleton()
//            {
//                ConstructorCalled = true;
//            }
//        }

//        [Test]
//        public void Bind_LazySingleton_ThenInvokeInstantiateNonLazySingletons_ShouldNotRunConstructor()
//        {
//            new ServiceCollection().AddSingleton<SomeSingleton>().BuildServiceProvider();
//            SomeSingleton.ConstructorCalled = false;
//            SomeSingleton.ConstructorCalled.Should().BeFalse();
//        }

//        [Test]
//        public void AddInstance_WithoutContract_ShouldBindToItsType()
//        {
//            var container = new ServiceCollection()
//                .AddInstance(42)
//                .Build();

//            container.GetService<int>().Should().Be(42);
//        }

//        [Test]
//        public void ResolveAll_WithoutMatch_ShouldReturnEmptyEnumerable()
//        {
//            var container = new ServiceCollection().Build();
//            container.GetServices<IDisposable>().Should().BeEmpty();
//        }

//        public class StartableSingleton : IStartable
//        {
//            public static Action OnConstructed;
//            public static Action OnStarted;

//            public StartableSingleton()
//            {
//                OnConstructed?.Invoke();
//            }

//            public bool WasStarted { get; private set; }

//            public void Start()
//            {
//                WasStarted = true;
//                OnStarted?.Invoke();
//            }
//        }

//        [Test]
//        public void NonStartableSingleton_ShouldNotBeConstructedAfterContainerBuild()
//        {
//            var callbackAssertion = new CallbackAssertion();
//            StartableSingleton.OnConstructed = callbackAssertion;

//            var container = new ServiceCollection()
//                .AddSingleton(typeof(StartableSingleton))
//                .Build();

//            callbackAssertion.ShouldNotHaveBeenCalled();
//        }

//        [Test]
//        public void StartableSingleton_ShouldBeConstructedAfterContainerBuild()
//        {
//            var callbackAssertion = new CallbackAssertion();
//            StartableSingleton.OnConstructed = callbackAssertion;

//            var container = new ServiceCollection()
//                .AddSingleton(typeof(StartableSingleton), typeof(IStartable))
//                .Build();

//            callbackAssertion.ShouldHaveBeenCalledOnce();
//        }

//        [Test]
//        public void StartableSingleton_ShouldBeStartedAfterContainerBuild()
//        {
//            var container = new ServiceCollection()
//                .AddSingleton<IStartable, StartableSingleton>()
//                .Build();

//            var startable = container.GetService<StartableSingleton>().WasStarted.Should().BeTrue();
//        }

//        [Test]
//        public void All_OnParentShouldNotBeAffectedByScoped()
//        {
//            var container = new ServiceCollection().AddInstance(1).Build();
//            string.Join(",", container.GetServices<int>()).Should().Be("1");
//            var scoped = container.Scope("", descriptor => { descriptor.AddInstance(2); });
//            string.Join(",", container.GetServices<int>()).Should().Be("1");
//        }

//        [Test]
//        public void HasBindingReturnFalseWhenBindingIsNotDefined()
//        {
//            var container = new ServiceCollection().Build();
//            container.HasBinding<int>().Should().BeFalse();
//        }

//        [Test]
//        public void HasBindingReturnTrueWhenBindingIsDefined()
//        {
//            var container = new ServiceCollection().AddInstance(42).Build();
//            container.HasBinding<int>().Should().BeTrue();
//        }

//        [Test]
//        public void IStartable_Start_ShouldNotBeInvokedAgainAfterScoping()
//        {
//            var callbackAssertion = new CallbackAssertion();
//            StartableSingleton.OnStarted = callbackAssertion;

//            var container = new ServiceCollection()
//                .AddSingleton(typeof(StartableSingleton), typeof(IStartable))
//                .Build();

//            var scoped = container.Scope("");

//            callbackAssertion.ShouldHaveBeenCalledOnce();
//        }
//    }
//}