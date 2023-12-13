using System;
using FluentAssertions;
using Reflex.Core;
using NUnit.Framework;

namespace Reflex.Tests
{
    internal class DisposeTests
    {
        private class Service : IDisposable
        {
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        [Test]
        public void SingletonFromType_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder("")
                .AddSingleton(typeof(Service), typeof(Service))
                .Build();
            
            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }
        
        [Test]
        public void SingletonFromValue_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var service = new Service();
            var container = new ContainerBuilder("")
                .AddSingleton(service, typeof(Service))
                .Build();
            
            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void SingletonFromFactory_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            Service Factory(Container container)
            {
                return new Service();
            }
            
            var container = new ContainerBuilder("")
                .AddSingleton(Factory, typeof(Service))
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void TransientFromType_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder("")
                .AddTransient(typeof(Service), typeof(Service))
                .Build();
            
            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }
        
        [Test]
        public void TransientFromFactory_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            Service Factory(Container container)
            {
                return new Service();
            }
            
            var container = new ContainerBuilder("")
                .AddTransient(Factory, typeof(Service))
                .Build();
            
            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }
    }
}