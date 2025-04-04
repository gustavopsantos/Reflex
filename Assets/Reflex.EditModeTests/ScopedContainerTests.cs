using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;

namespace Reflex.EditModeTests
{
    internal class ScopedContainerTests
    {
        private class Disposable : IDisposable
        {
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }
        
        private class DisposeHook : IDisposable
        {
            private event Action OnDispose;

            public DisposeHook(Action onDispose)
            {
                OnDispose += onDispose;
            }

            public void Dispose()
            {
                OnDispose.Invoke();
            }
        }

        [Test]
        public void ScopedContainer_CanResolveDependency_FromParentContainer()
        {
            using (var outer = new ContainerBuilder().RegisterValue(42, Lifetime.Singleton).Build())
            {
                using (var inner = outer.Scope())
                {
                    inner.Single<int>().Should().Be(42);
                }
            }
        }

        [Test]
        public void ParentWithScopedContainer_ParentShouldNotBeAbleToResolveDependencyFromChild()
        {
            var outer = new ContainerBuilder().Build();
            var inner = outer.Scope(builder => builder.RegisterValue(42, Lifetime.Singleton));
            Action resolve = () => outer.Single<int>();
            resolve.Should().ThrowExactly<UnknownContractException>();
        }

        [Test]
        public void ScopedContainer_WhenDisposed_ShouldNotDisposeDependencyFromParentContainer()
        {
            using (var outer = new ContainerBuilder().RegisterType(typeof(Disposable), Lifetime.Singleton, Resolution.Lazy).Build())
            {
                using (var inner = outer.Scope())
                {
                    var disposable = inner.Single<Disposable>();
                    disposable.Should().NotBeNull();
                }

                outer.Single<Disposable>().Disposed.Should().Be(0);
            }
        }

        [Test]
        public void ScopedContainer_Scoped_ShouldParentAsParent()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    inner.Parent.Should().Be(outer);
                }
            }
        }

        [Test]
        public void ScopedContainer_Parent_ShouldHaveScopedAsChild()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    outer.Children.Contains(inner).Should().BeTrue();
                }
            }
        }

        [Test]
        public void ScopedContainer_AfterDisposal_ShouldBeRemoveAsParentChild()
        {
            var parent = new ContainerBuilder().Build();
            var child = parent.Scope();

            child.Parent.Should().Be(parent);
            parent.Children.Contains(child).Should().BeTrue();
            child.Dispose();
            parent.Children.Should().BeEmpty();
        }

        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder1()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .RegisterValue(new DisposeHook(() => disposalOrder.Add("a")), Lifetime.Singleton)
                .Build();

            var b = a.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("b")), Lifetime.Singleton));
            var c = b.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("c")), Lifetime.Singleton));
            var d = c.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("d")), Lifetime.Singleton));
            var e = d.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("e")), Lifetime.Singleton));
            
            a.Dispose();
            
            string.Join(",", disposalOrder).Should().BeEquivalentTo("e,d,c,b,a");
        }
        
        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder2()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .RegisterValue(new DisposeHook(() => disposalOrder.Add("a")), Lifetime.Singleton)
                .Build();

            var b = a.Scope(builder => builder.RegisterValue(new DisposeHook(() => disposalOrder.Add("b")), Lifetime.Singleton));
            
            a.Dispose();
            
            string.Join(",", disposalOrder).Should().BeEquivalentTo("b,a");
        }
        
        [Test]
        public void ScopedContainer_ResolvingContainerFromInnerScope_ShouldResolveInner()
        {
            using (var outer = new ContainerBuilder().Build())
            {
                using (var inner = outer.Scope())
                {
                    inner.Single<Container>().Should().Be(inner);
                }
            }
        }

        [Test]
        public void ScopedContainer_ResolvingContainerFromOuterScope_ShouldResolveOuter()
        {
            using var outer = new ContainerBuilder().Build();
            using var inner = outer.Scope(); 
            var temp = inner.Single<Container>();
            outer.Single<Container>().Should().Be(outer);
        }
    }
}