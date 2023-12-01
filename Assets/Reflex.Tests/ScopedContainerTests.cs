using System;
using System.Collections.Generic;
using FluentAssertions;
using Reflex.Core;
using Reflex.Exceptions;
using NUnit.Framework;

namespace Reflex.Tests
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
            using (var outer = new ContainerDescriptor("").AddSingleton(42, typeof(int)).Build())
            {
                using (var inner = outer.Scope(""))
                {
                    inner.Single<int>().Should().Be(42);
                }
            }
        }

        [Test]
        public void ParentWithScopedContainer_ParentShouldNotBeAbleToResolveDependencyFromChild()
        {
            var outer = new ContainerDescriptor("").Build();
            var inner = outer.Scope("", builder => builder.AddSingleton(42, typeof(int)));
            Action resolve = () => outer.Single<int>();
            resolve.Should().ThrowExactly<UnknownContractException>();
        }

        [Test]
        public void ScopedContainer_WhenDisposed_ShouldNotDisposeDependencyFromParentContainer()
        {
            using (var outer = new ContainerDescriptor("").AddSingleton(typeof(Disposable), typeof(Disposable)).Build())
            {
                using (var inner = outer.Scope(""))
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
            using (var outer = new ContainerDescriptor("").Build())
            {
                using (var inner = outer.Scope(""))
                {
                    inner.Parent.Should().Be(outer);
                }
            }
        }

        [Test]
        public void ScopedContainer_Parent_ShouldHaveScopedAsChild()
        {
            using (var outer = new ContainerDescriptor("").Build())
            {
                using (var inner = outer.Scope(""))
                {
                    outer.Children.Contains(inner).Should().BeTrue();
                }
            }
        }

        [Test]
        public void ScopedContainer_AfterParentDisposal_ShouldBeUnParented()
        {
            var outer = new ContainerDescriptor("").Build();
            var inner = outer.Scope("");

            inner.Parent.Should().Be(outer);
            outer.Children.Contains(inner).Should().BeTrue();
            outer.Dispose();
            inner.Parent.Should().BeNull();
            outer.Children.Contains(inner).Should().BeFalse();
        }

        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder1()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerDescriptor("")
                .AddSingleton(new DisposeHook(() => { disposalOrder.Add("a"); }))
                .Build();

            var b = a.Scope("", builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("b"); })));
            var c = b.Scope("", builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("c"); })));
            var d = c.Scope("", builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("d"); })));
            var e = d.Scope("", builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("e"); })));
            
            a.Dispose();
            
            string.Join(",", disposalOrder).Should().BeEquivalentTo("e,d,c,b,a");
        }
        
        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder2()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerDescriptor("")
                .AddSingleton(new DisposeHook(() => { disposalOrder.Add("a"); }))
                .Build();

            var b = a.Scope("", builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("b"); })));
            
            a.Dispose();
            
            string.Join(",", disposalOrder).Should().BeEquivalentTo("b,a");
        }
        
        [Test]
        public void ScopedContainer_ResolvingContainerFromInnerScope_ShouldResolveInner()
        {
            using (var outer = new ContainerDescriptor("").Build())
            {
                using (var inner = outer.Scope(""))
                {
                    inner.Single<Container>().Should().Be(inner);
                }
            }
        }

        [Test]
        public void ScopedContainer_ResolvingContainerFromOuterScope_ShouldResolveOuter()
        {
            using var outer = new ContainerDescriptor("").Build();
            using var inner = outer.Scope(""); 
            var temp = inner.Single<Container>();
            outer.Single<Container>().Should().Be(outer);
        }
    }
}