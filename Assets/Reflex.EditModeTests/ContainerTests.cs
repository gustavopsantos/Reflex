using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;
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

        private class ClassWithDefaultUnresolvableValues : IClassWithDependency
        {
            private readonly int _constructorValue;

            private int _methodValue;

            public ClassWithDefaultUnresolvableValues(int value = 42)
            {
                _constructorValue = value;
            }

            [Inject]
            public void InjectionMethod(int value = 42)
            {
                _methodValue = value;
            }
        }

        [Test]
        public void Resolve_ValueTypeSingleton_ShouldReturn42()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
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
                .RegisterType(typeof(Valuable), new[] { typeof(IValuable) }, Lifetime.Transient, Resolution.Lazy)
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
                .RegisterFactory(Factory, Lifetime.Transient, Resolution.Lazy)
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
                .RegisterType(typeof(Valuable), new[] { typeof(IValuable) }, Lifetime.Singleton, Resolution.Lazy)
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
                .RegisterFactory(Factory, Lifetime.Singleton, Resolution.Lazy)
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
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) }, Lifetime.Transient, Resolution.Lazy)
                .Build();

            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) }, Lifetime.Singleton, Resolution.Lazy)
                .Build();

            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(int), Lifetime.Transient, Resolution.Lazy)
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
                .RegisterValue(42)
                .RegisterType(typeof(MyStruct), Lifetime.Transient, Resolution.Lazy)
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

        private class NumberAndStringCache
        {
            public int Int;
            public string String;

            public NumberAndStringCache(ISetup<int> intSetup, ISetup<string> stringSetup)
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
                .RegisterType(typeof(NumberAndStringCache), Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(IntSetup), new[] { typeof(ISetup<int>) }, Lifetime.Transient, Resolution.Lazy)
                .RegisterType(typeof(StringSetup), new[] { typeof(ISetup<string>) }, Lifetime.Transient, Resolution.Lazy)
                .Build();

            var instance = container.Construct<NumberAndStringCache>();
            instance.Int.Should().Be(42);
            instance.String.Should().Be("abc");
        }

        [Test]
        public void AddSingleton_WithoutContract_ShouldBindToItsType()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
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
            var container = new ContainerBuilder().RegisterValue(1).Build();
            string.Join(",", container.All<int>()).Should().Be("1");
            var scoped = container.Scope(containerBuilder => { containerBuilder.RegisterValue(2); });
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
            var container = new ContainerBuilder().RegisterValue(42).Build();
            container.HasBinding<int>().Should().BeTrue();
        }

        [Test]
        public void TryGetResolverDoesNotThrowsWhenContractIsMissing()
        {
            var container = new ContainerBuilder().Build();
            Func<bool> call = () => container.TryGetResolver(typeof(int), out _);
            call.Should().NotThrow();
            call().Should().BeFalse();
        }

        [Test]
        public void TryGetResolverGenericDoesNotThrowsWhenContractIsMissing()
        {
            var container = new ContainerBuilder().Build();
            Func<bool> call = () => container.TryGetResolver<int>(out _);
            call.Should().NotThrow();
            call().Should().BeFalse();
        }

        [Test]
        public void FailedResolutionSubstitutesWithDefaultMethodValues()
        {
            var container = new ContainerBuilder().RegisterType(typeof(ClassWithDefaultUnresolvableValues), Lifetime.Singleton, Resolution.Lazy).Build();
            Action call = () =>
            {
                container.Resolve<ClassWithDefaultUnresolvableValues>();
            };

            call.Should().NotThrow();
        }
    }
}