using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class ScopedTests
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
        public void ScopedFromType_ShouldReturnAlwaysSameInstance_WhenCalledFromSameContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();
            parentContainer.Resolve<Service>().Should().Be(parentContainer.Resolve<Service>());
            childContainer.Resolve<Service>().Should().Be(childContainer.Resolve<Service>());
        }

        [Test]
        public void ScopedFromFactory_ShouldReturnAlwaysSameInstance_WhenCalledFromSameContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();
            parentContainer.Resolve<Service>().Should().Be(parentContainer.Resolve<Service>());
            childContainer.Resolve<Service>().Should().Be(childContainer.Resolve<Service>());
        }

        [Test]
        public void ScopedFromType_NewInstanceShouldBeConstructed_ForEveryNewContainer()
        {
            var instances = new HashSet<Service>();
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service),Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Count.Should().Be(2);
        }

        [Test]
        public void ScopedFromFactory_NewInstanceShouldBeConstructed_ForEveryNewContainer()
        {
            var instances = new HashSet<Service>();
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Count.Should().Be(2);
        }

        [Test]
        public void ScopedFromType_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();

            childContainer.Dispose();

            instanceConstructedByChild.IsDisposed.Should().BeTrue();
            instanceConstructedByParent.IsDisposed.Should().BeFalse();
        }

        [Test]
        public void ScopedFromFactory_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped, Resolution.Lazy).Build();
            var childContainer = parentContainer.Scope();
        
            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();
        
            childContainer.Dispose();
        
            instanceConstructedByChild.IsDisposed.Should().BeTrue();
            instanceConstructedByParent.IsDisposed.Should().BeFalse();
        }
        
        [Test, Retry(3)]
        public void ScopedFromType_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped, Resolution.Lazy).Build();

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
        public void ScopedFromFactory_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped, Resolution.Lazy).Build();

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