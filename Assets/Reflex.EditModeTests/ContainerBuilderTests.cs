using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Exceptions;
using UnityEngine;
using Resolution = Reflex.Core.Resolution;

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
            Action addSingleton = () => builder.Add(Singleton.FromType(typeof(Valuable), new[] { typeof(IDisposable) }, Resolution.Lazy));
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddSingletonFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.Add(Singleton.FromType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Resolution.Lazy));
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddSingletonFromValue_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.Add(Singleton.FromValue(new Valuable(), new[] { typeof(IDisposable) }));
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }
        
        [Test]
        public void AddSingletonFromValue_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.Add(Singleton.FromValue(new Valuable(), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }));
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
            Action addSingleton = () => builder.Add(Singleton.FromFactory(Factory, new[] { typeof(IDisposable) }, Resolution.Lazy));
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
            Action addSingleton = () => builder.Add(Singleton.FromFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Resolution.Lazy));
            addSingleton.Should().NotThrow();
        }
        
        [Test]
        public void AddTransientFromType_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.Add(Transient.FromType(typeof(Valuable), new[] { typeof(IDisposable) }));
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddTransientFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.Add(Transient.FromType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }));
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
            Action addSingleton = () => builder.Add(Transient.FromFactory(Factory, new[] { typeof(IDisposable) }));
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
            Action addSingleton = () => builder.Add(Transient.FromFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }));
            addSingleton.Should().NotThrow();
        }

        [Test]
        public void HasBinding_ShouldTrue()
        {
            var value = Debug.unityLogger;
            var builder = new ContainerBuilder().Add(Singleton.FromValue(value));
            builder.HasBinding(value.GetType()).Should().BeTrue();
        }
        
        [Test]
        public void Build_CallBack_ShouldBeCalled()
        {
            Container container = null;
            var builder = new ContainerBuilder();
            builder.OnContainerBuilt += ContainerCallback;
            Action addSingleton = () => builder.Add(Singleton.FromValue(new Valuable(), new[] { typeof(IDisposable) })).Build();
            void ContainerCallback(Container ctx)
            {
                container = ctx;
            }
            builder.Build();
            container.Should().NotBeNull();
        }
    }
}