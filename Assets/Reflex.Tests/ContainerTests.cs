using System;
using FluentAssertions;
using Reflex.Core;
using Reflex.Exceptions;
using NUnit.Framework;

namespace Reflex.Tests
{
    internal class ContainerTests
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
            var container = new ContainerBuilder("")
                .AddSingleton(42, typeof(int))
                .Build();
            
            container.Single<int>().Should().Be(42);
        }

        [Test]
        public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder("").Build();
            Action resolve = () => container.Single<int>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_AsTransientFromType_ShouldReturnAlwaysANewInstance()
        {
            var container = new ContainerBuilder("")
                .AddTransient(typeof(Valuable), typeof(IValuable))
                .Build();
            
            container.Single<IValuable>().Value = 123;
            container.Single<IValuable>().Value.Should().Be(default(int));
        }
        
        [Test]
        public void Resolve_AsTransientFromFactory_ShouldRunFactoryAlways()
        {
            var callbackAssertion = new CallbackAssertion();
            
            string Factory(Container container)
            {
                callbackAssertion.Invoke();
                return "Hello World!";
            }
            
            var container = new ContainerBuilder("")
                .AddTransient(Factory)
                .Build();
            
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalled(3);
        }

        [Test]
        public void Resolve_AsSingletonFromType_ShouldReturnAlwaysSameInstance()
        {
            var container = new ContainerBuilder("")
                .AddSingleton(typeof(Valuable), typeof(IValuable))
                .Build();
            
            container.Single<IValuable>().Value = 123;
            container.Single<IValuable>().Value.Should().Be(123);
        }
        
        [Test]
        public void Resolve_AsSingletonFromFactory_ShouldRunFactoryOnce()
        {
            var callbackAssertion = new CallbackAssertion();
            
            string Factory(Container container)
            {
                callbackAssertion.Invoke();
                return "Hello World!";
            }
            
            var container = new ContainerBuilder("")
                .AddSingleton(Factory)
                .Build();
            
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalledOnce();
        }

        [Test]
        public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder("").Build();
            Action resolve = () => container.Single<IValuable>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder("")
                .AddTransient(typeof(ClassWithDependency), typeof(IClassWithDependency))
                .Build();
            
            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder("")
                .AddSingleton(typeof(ClassWithDependency), typeof(IClassWithDependency))
                .Build();
            
            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
        {
            var container = new ContainerBuilder("")
                .AddTransient(typeof(int), typeof(int))
                .Build();
            
        	container.Single<int>().Should().Be(default);
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
            var container = new ContainerBuilder("")
                .AddSingleton(42, typeof(int))
                .AddTransient(typeof(MyStruct), typeof(MyStruct))
                .Build();
            
            container.Single<MyStruct>().Value.Should().Be(42);
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
            var container = new ContainerBuilder("")
                .AddTransient(typeof(Xing), typeof(Xing))
                .AddTransient(typeof(IntSetup), typeof(ISetup<int>))
                .AddTransient(typeof(StringSetup), typeof(ISetup<string>))
                .Build();
            
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
            new ContainerBuilder("").AddSingleton(typeof(SomeSingleton), typeof(SomeSingleton)).Build();
            SomeSingleton.ConstructorCalled = false;
            SomeSingleton.ConstructorCalled.Should().BeFalse();
        }
        
        [Test]
        public void AddSingleton_WithoutContract_ShouldBindToItsType()
        {
            var container = new ContainerBuilder("")
                .AddSingleton(42)
                .Build();
            
            container.Single<int>().Should().Be(42);
        }
        
        [Test]
        public void ResolveAll_WithoutMatch_ShouldReturnEmptyEnumerable()
        {
            var container = new ContainerBuilder("").Build();
            container.All<IDisposable>().Should().BeEmpty();
        }
        
        [Test]
        public void All_OnParentShouldNotBeAffectedByScoped()
        {
            var container = new ContainerBuilder("").AddSingleton(1).Build();
            string.Join(",", container.All<int>()).Should().Be("1");
            var scoped = container.Scope("", descriptor => { descriptor.AddSingleton(2); });
            string.Join(",", container.All<int>()).Should().Be("1");
        }
        
        [Test]
        public void HasBindingReturnFalseWhenBindingIsNotDefined()
        {
            var container = new ContainerBuilder("").Build();
            container.HasBinding<int>().Should().BeFalse();
        }
        
        [Test]
        public void HasBindingReturnTrueWhenBindingIsDefined()
        {
            var container = new ContainerBuilder("").AddSingleton(42).Build();
            container.HasBinding<int>().Should().BeTrue();
        }
    }
}