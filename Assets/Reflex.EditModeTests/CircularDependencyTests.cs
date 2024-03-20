using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    public class CircularDependencyTests
    {
        private class ServiceA
        {
            private readonly ServiceB _b;

            public ServiceA(ServiceB b)
            {
                _b = b;
            }
        }

        private class ServiceB
        {
            private readonly ServiceA _a;

            public ServiceB(ServiceA a)
            {
                _a = a;
            }
        }

        [Test]
        public void Resolve_ShouldThrowStackOverflowException_WithCircularDependencies_AsIsNotSupported()
        {
            var builder = new ContainerBuilder();
            builder.AddSingleton(typeof(ServiceA));
            builder.AddSingleton(typeof(ServiceB));
            var container = builder.Build();
            Action resolve = () => container.Resolve<ServiceB>();
            resolve.Should().ThrowExactly<StackOverflowException>();
        }

        [Test]
        public void AllGeneric_ShouldThrowStackOverflowException_WithCircularDependencies_AsIsNotSupported()
        {
            var builder = new ContainerBuilder();
            builder.AddSingleton(typeof(ServiceA));
            builder.AddSingleton(typeof(ServiceB));
            var container = builder.Build();
            Action resolve = () => container.All<ServiceB>();
            resolve.Should().ThrowExactly<StackOverflowException>();
        }

        [Test]
        public void AllNonGeneric_ShouldThrowStackOverflowException_WithCircularDependencies_AsIsNotSupported()
        {
            var builder = new ContainerBuilder();
            builder.AddSingleton(typeof(ServiceA));
            builder.AddSingleton(typeof(ServiceB));
            var container = builder.Build();
            Action resolve = () => container.All(typeof(ServiceB));
            resolve.Should().ThrowExactly<StackOverflowException>();
        }
    }
}