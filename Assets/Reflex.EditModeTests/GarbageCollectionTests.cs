using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UnityEditor.Compilation;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    internal class GarbageCollectionTests
    {
        private class Service
        {
        }
        
        private static async Task ForceGarbageCollectionAsync()
        {
            await Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        [OneTimeSetUp]
        public void Setup()
        {
            if (CompilationPipeline.codeOptimization == CodeOptimization.Debug)
            {
                Assert.Inconclusive("GC works differently in Debug mode. Please run tests in Release mode.");
            }
        }

        [Test]
        public async Task Singleton_ShouldBeFinalized_WhenOwnerIsDisposed()
        {
            var references = new List<WeakReference>();

            void Act()
            {
                var container = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Lazy).Build();
                var service = container.Single<Service>();
                references.Add(new WeakReference(service));
                container.Dispose();
            }

            Act();
            await ForceGarbageCollectionAsync();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test]
        public async Task DisposedScopedContainer_ShouldHaveNoReferencesToItself_AndShouldBeCollectedAndFinalized()
        {
            var references = new List<WeakReference>();

            void Act()
            {
                var parent = new ContainerBuilder().Build();
                var scoped = parent.Scope();
                references.Add(new WeakReference(scoped));
                scoped.Dispose();
            }

            Act();
            await ForceGarbageCollectionAsync();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test]
        public async Task Construct_ContainerShouldNotControlConstructedObjectLifeCycle_ByNotKeepingReferenceToIt()
        {
            var references = new List<WeakReference>();
            var container = new ContainerBuilder().Build();

            void Act()
            {
                var service = container.Construct<Service>();
                references.Add(new WeakReference(service));
            }

            Act();
            await ForceGarbageCollectionAsync();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }
        
        [Test]
        public async Task ScopedFromType_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
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
            await ForceGarbageCollectionAsync();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeTrue();
        }
        
        [Test]
        public async Task ScopedFromFactory_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
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
            await ForceGarbageCollectionAsync();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeTrue();
        }
        
        /// <summary>
        /// Because Transient instances are not tracked by the container,
        /// so they should be collected as soon as there are no references to them,
        /// even if the container that constructed them is still alive.
        /// </summary>
        [Test]
        public async Task TransientFromType_ConstructedInstances_ShouldBeCollected_BeforeConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Transient, Resolution.Lazy).Build();

            void Act()
            {
                using (var childContainer = parentContainer.Scope())
                {
                    instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                    instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
                }
            }
            
            Act();
            await ForceGarbageCollectionAsync();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeFalse();
        }
        
        /// <summary>
        /// Because Transient instances are not tracked by the container,
        /// so they should be collected as soon as there are no references to them,
        /// even if the container that constructed them is still alive.
        /// </summary>
        [Test]
        public async Task TransientFromFactory_ConstructedInstances_ShouldBeCollected_BeforeConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Transient, Resolution.Lazy).Build();

            void Act()
            {
                using (var childContainer = parentContainer.Scope())
                {
                    instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                    instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
                }
            }
            
            Act();
            await ForceGarbageCollectionAsync();
            instanceConstructedByChild.IsAlive.Should().BeFalse();
            instanceConstructedByParent.IsAlive.Should().BeFalse();
        }
    }
}