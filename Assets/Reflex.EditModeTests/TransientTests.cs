using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class TransientTests
    {
        private class Service : IDisposable
        {
            public bool IsDisposed { get; private set; }
            
            public void Dispose()
            {
                IsDisposed = true;
            }
        }
        
        [Test]
        public void TransientFromType_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Transient, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();
            
            childContainer.Dispose();

            instanceConstructedByChild.IsDisposed.Should().BeTrue();
            instanceConstructedByParent.IsDisposed.Should().BeFalse();
        }
        
        [Test]
        public void TransientFromFactory_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Transient, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();
            
            childContainer.Dispose();

            instanceConstructedByChild.IsDisposed.Should().BeTrue();
            instanceConstructedByParent.IsDisposed.Should().BeFalse();
        }
    }
}