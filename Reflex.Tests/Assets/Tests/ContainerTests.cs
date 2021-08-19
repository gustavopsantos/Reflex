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
        
        private struct MyStruct
        {
            public readonly int Value;

            public MyStruct(int value)
            {
                this.Value = value;
            }
        }
        
        [Test]
        public void Resolve_ValueTypeSingleton_ShouldReturn42()
        {
            Container container = new Container();
            container.BindSingleton(42);
            container.Resolve<int>().Should().Be(42);
        }
        
        [Test]
        public void Resolve_ValueTypeAsTransient_CustomConstructor_ValueShouldReturn42()
        {
            Container container = new Container();
            container.BindSingleton(42);
            container.Bind<MyStruct>().To<MyStruct>().AsTransient();
            container.Resolve<MyStruct>().Value.Should().Be(42);
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
            Action resolve = () =>
                container.ResolveGenericContract<object>(typeof(Pair<,>), typeof(int), typeof(string));
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
            container.Bind<IValuable>().To<Valuable>().AsSingleton();
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
            container.Bind<IClassWithDependency>().To<ClassWithDependency>().AsSingleton();
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
            container.BindGenericContract(typeof(IFoo<>)).To(typeof(StringFoo)).AsSingleton();
            var foo = container.ResolveGenericContract<object>(typeof(IFoo<>), typeof(string));
            foo.GetType().GetInterfaces().First().GenericTypeArguments.First().Should().Be(typeof(string));
        }

        [Test]
        public void Resolve_GenericTypeOfObject_ShouldReturnImplementationWithObjectAsGenericTypeArgument()
        {
            Container container = new Container();
            container.BindGenericContract(typeof(IFoo<>)).To(typeof(ObjectFoo)).AsSingleton();
            var foo = container.ResolveGenericContract<object>(typeof(IFoo<>), typeof(object));
            foo.GetType().GetInterfaces().First().GenericTypeArguments.First().Should().Be(typeof(object));
        }
    }
}