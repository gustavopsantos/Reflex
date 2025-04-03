using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.EditModeTests
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
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(42))
                .Build();
            
            container.Single<int>().Should().Be(42);
        }

        [Test]
        public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder().Build();
            Action resolve = () => container.Single<int>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_AsTransientFromType_ShouldReturnAlwaysANewInstance()
        {
            var container = new ContainerBuilder()
                .Add(Transient.FromType(typeof(Valuable), new[] { typeof(IValuable) }))
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
            
            var container = new ContainerBuilder()
                .Add(Transient.FromFactory(Factory))
                .Build();
            
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalled(3);
        }

        [Test]
        public void Resolve_AsSingletonFromType_ShouldReturnAlwaysSameInstance()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromType(typeof(Valuable), new[] { typeof(IValuable) }, Resolution.Lazy))
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
            
            var container = new ContainerBuilder()
                .Add(Singleton.FromFactory(Factory, Resolution.Lazy))
                .Build();
            
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalledOnce();
        }

        [Test]
        public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder().Build();
            Action resolve = () => container.Single<IValuable>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .Add(Transient.FromType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) }))
                .Build();
            
            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromType(typeof(ClassWithDependency), new []{typeof(IClassWithDependency)}, Resolution.Lazy))
                .Build();
            
            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
        {
            var container = new ContainerBuilder()
                .Add(Transient.FromType(typeof(int)))
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
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(42))
                .Add(Transient.FromType(typeof(MyStruct)))
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
            var container = new ContainerBuilder()
                .Add(Transient.FromType(typeof(Xing), new[] { typeof(Xing) }))
                .Add(Transient.FromType(typeof(IntSetup), new[] { typeof(ISetup<int>) }))
                .Add(Transient.FromType(typeof(StringSetup), new[] { typeof(ISetup<string>) }))
                .Build();
            
        	var instance = container.Construct<Xing>();
        	instance.Int.Should().Be(42);
        	instance.String.Should().Be("abc");
        }

        [Test]
        public void AddSingleton_WithoutContract_ShouldBindToItsType()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(42))
                .Build();
            
            container.Single<int>().Should().Be(42);
        }
        
        [Test]
        public void ResolveAll_WithoutMatch_ShouldReturnEmptyEnumerable()
        {
            var container = new ContainerBuilder().Build();
            container.All<IDisposable>().Should().BeEmpty();
        }
        
        [Test]
        public void All_OnParentShouldNotBeAffectedByScoped()
        {
            var container = new ContainerBuilder().Add(Singleton.FromValue(1)).Build();
            string.Join(",", container.All<int>()).Should().Be("1");
            var scoped = container.Scope(containerBuilder => { containerBuilder.Add(Singleton.FromValue(2)); });
            string.Join(",", container.All<int>()).Should().Be("1");
        }
        
        [Test]
        public void HasBindingReturnFalseWhenBindingIsNotDefined()
        {
            var container = new ContainerBuilder().Build();
            container.HasBinding<int>().Should().BeFalse();
        }
        
        [Test]
        public void HasBindingReturnTrueWhenBindingIsDefined()
        {
            var container = new ContainerBuilder().Add(Singleton.FromValue(42)).Build();
            container.HasBinding<int>().Should().BeTrue();
        }
    }
}