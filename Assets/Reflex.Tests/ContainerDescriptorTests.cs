using System;
using FluentAssertions;
using Reflex.Core;
using Reflex.Exceptions;
using NUnit.Framework;
using UnityEngine;

namespace Reflex.Tests
{
    internal class ContainerDescriptorTests
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
        public void AddInstance_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddInstance(new Valuable(), typeof(IDisposable));
            addInstance.Should().ThrowExactly<ContractDefinitionException>();
        }
        
        [Test]
        public void AddInstance_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddInstance(new Valuable(), typeof(object), typeof(Valuable), typeof(IValuable));
            addInstance.Should().NotThrow();
        }
        
        [Test]
        public void AddTransient_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddTransient(typeof(Valuable), typeof(IDisposable));
            addInstance.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddTransient_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddTransient(typeof(Valuable), typeof(object), typeof(Valuable), typeof(IValuable));
            addInstance.Should().NotThrow();
        }
        
        [Test]
        public void AddSingleton_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddSingleton(typeof(Valuable), typeof(IDisposable));
            addInstance.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddSingleton_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor("");
            Action addInstance = () => builder.AddSingleton(typeof(Valuable), typeof(object), typeof(Valuable), typeof(IValuable));
            addInstance.Should().NotThrow();
        }

        [Test]
        public void AddSingleton_ValuableWithIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor(string.Empty);
            Action addInstance = () => builder.AddSingleton<Valuable, IValuable>();
            addInstance.Should().NotThrow();
        }

        [Test]
        public void AddTransient_ValuableWithIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor(string.Empty);
            Action addInstance = () => builder.AddTransient<Valuable, IValuable>();
            addInstance.Should().NotThrow();
        }

        [Test]
        public void AddSingleton_SpecificValuableWithIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor(string.Empty);
            Action addInstance = () => builder.AddSingleton<Valuable, IValuable>(() => new Valuable());
            addInstance.Should().NotThrow();
        }

        [Test]
        public void AddTransient_SpecificValuableWithIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor(string.Empty);
            Action addInstance = () => builder.AddTransient<Valuable, IValuable>(() => new Valuable());
            addInstance.Should().NotThrow();
        }

        [Test]
        public void AddInstance_SpecificValuableWithIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerDescriptor(string.Empty);
            Action addInstance = () => builder.AddInstance<Valuable, IValuable>(() => new Valuable());
            addInstance.Should().NotThrow();
        }

        [Test]
        public void HasBinding_ShouldTrue()
        {
            var builder = new ContainerDescriptor("").AddInstance(Debug.unityLogger);
            builder.HasBinding(Debug.unityLogger.GetType()).Should().BeTrue();
        }
        
        [Test]
        public void Build_CallBack_ShouldBeCalled()
        {
            Container container = null;
            var builder = new ContainerDescriptor("");
            builder.OnContainerBuilt += ContainerCallback;
            Action addInstance = () => builder.AddInstance(new Valuable(), typeof(IDisposable)).Build();
            void ContainerCallback(Container ctx)
            {
                container = ctx;
            }
            builder.Build();
            container.Should().NotBeNull();
        }
    }
}