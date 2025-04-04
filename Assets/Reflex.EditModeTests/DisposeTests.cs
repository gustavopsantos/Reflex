using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
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
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Lazy)
                .Build();
            
            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }
        
        [Test]
        public void SingletonFromValue_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var service = new Service();
            var container = new ContainerBuilder()
                .RegisterValue(service, Lifetime.Singleton)
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
            
            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Singleton, Resolution.Lazy)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void TransientFromType_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Transient, Resolution.Lazy)
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
            
            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Transient, Resolution.Lazy)
                .Build();
            
            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }
    }
}