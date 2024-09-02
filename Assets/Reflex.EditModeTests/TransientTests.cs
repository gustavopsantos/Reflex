using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

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
            var parentContainer = new ContainerBuilder().AddTransient(typeof(Service)).Build();
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
            var parentContainer = new ContainerBuilder().AddTransient(_ => new Service()).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();
            
            childContainer.Dispose();

            instanceConstructedByChild.IsDisposed.Should().BeTrue();
            instanceConstructedByParent.IsDisposed.Should().BeFalse();
        }
        
        [Test, Retry(3)]
        public void TransientFromType_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().AddTransient(typeof(Service)).Build();

            void Act()
            {
                using (var childContainer = parentContainer.Scope())
                {
                    instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                    instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
                }
            }
            
            Act();
            GarbageCollectionTests.ForceGarbageCollection();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeTrue();
        }
        
        [Test, Retry(3)]
        public void TransientFromFactory_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().AddTransient(c => new Service()).Build();

            void Act()
            {
                using (var childContainer = parentContainer.Scope())
                {
                    instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                    instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
                }
            }
            
            Act();
            GarbageCollectionTests.ForceGarbageCollection();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeTrue();
        }
    }
}