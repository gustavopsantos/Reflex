using System;
using FluentAssertions;
using Reflex.Core;
using Reflex.Exceptions;
using NUnit.Framework;

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
    }
}