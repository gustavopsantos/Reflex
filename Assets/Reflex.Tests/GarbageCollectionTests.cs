using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Reflex.Tests
{
    internal class GarbageCollectionTests
    {
        private class Service
        {
        }

        private static void AssertIncrementalGarbageCollectionIsDisabled()
        {
            if (GarbageCollector.isIncremental) // Incremental GC can mess a bit with finalizer queue
            {
                Assert.Inconclusive();
            }
        }

        private static void ForceGarbageCollection()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [Test]
        public void Singleton_ShouldBeFinalized_WhenOwnerIsDisposed()
        {
            AssertIncrementalGarbageCollectionIsDisabled();
            var references = new List<WeakReference>();
            
            void Act()
            {
                var container = new ContainerBuilder().AddSingleton(typeof(Service), typeof(Service)).Build();
                var service = container.Single<Service>();
                references.Add(new WeakReference(service));
                container.Dispose();
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test]
        public void DisposedScopedContainer_ShouldHaveNoReferencesToItself_AndShouldBeCollectedAndFinalized()
        {
            AssertIncrementalGarbageCollectionIsDisabled();
            var references = new List<WeakReference>();
            var parent = new ContainerBuilder().Build();

            void Act()
            {
                var scoped = parent.Scope();
                references.Add(new WeakReference(scoped));
                scoped.Dispose();
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }

        [Test]
        public void Construct_ContainerShouldNotControlConstructedObjectLifeCycle_ByNotKeepingReferenceToIt()
        {
            AssertIncrementalGarbageCollectionIsDisabled();
            var references = new List<WeakReference>();
            var container = new ContainerBuilder().Build();

            void Act()
            {
                var service = container.Construct<Service>();
                references.Add(new WeakReference(service));
            }

            Act();
            ForceGarbageCollection();
            references.Any(r => r.IsAlive).Should().BeFalse();
        }
    }
}