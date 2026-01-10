using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    internal class ContainerBuilderTests
    {
        private interface IValuable
        {
            int Value { get; set; }
        }

        private class Valuable : IValuable
        {
            public int Value { get; set; }
        }
        
        [Test]
        public void AddSingletonFromType_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(IDisposable) }, Lifetime.Singleton, Resolution.Lazy);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddSingletonFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Singleton, Resolution.Lazy);
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddSingletonFromValue_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterValue(new Valuable(), new[] { typeof(IDisposable) });
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }
        
        [Test]
        public void AddSingletonFromValue_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterValue(new Valuable(), new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddSingletonFromFactory_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            Valuable Factory(Container container)
            {
                return new Valuable();
            }
            
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(IDisposable) }, Lifetime.Singleton, Resolution.Lazy);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }
        
        [Test]
        public void AddSingletonFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            Valuable Factory(Container container)
            {
                return new Valuable();
            }
            
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Singleton, Resolution.Lazy);
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddTransientFromType_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(IDisposable) }, Lifetime.Transient, Resolution.Lazy);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddTransientFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient, Resolution.Lazy);
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddTransientFromFactory_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            Valuable Factory(Container container)
            {
                return new Valuable();
            }
            
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(IDisposable) }, Lifetime.Transient, Resolution.Lazy);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddTransientFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            Valuable Factory(Container container)
            {
                return new Valuable();
            }
            
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient, Resolution.Lazy);
            addSingleton.Should().NotThrow();
        }

        [Test]
        public void HasBinding_ShouldTrue()
        {
            var value = Debug.unityLogger;
            var builder = new ContainerBuilder().RegisterValue(value);
            builder.HasBinding(value.GetType()).Should().BeTrue();
        }
        
        [Test]
        public void Build_CallBack_ShouldBeCalled()
        {
            Container container = null;
            var builder = new ContainerBuilder();
            builder.OnContainerBuilt += ContainerCallback;
            void ContainerCallback(Container ctx)
            {
                container = ctx;
            }
            builder.Build();
            container.Should().NotBeNull();
        }
    }
}