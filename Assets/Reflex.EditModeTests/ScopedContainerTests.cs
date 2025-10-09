using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
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
            using (var outer = new ContainerBuilder().AddSingleton(42, typeof(int)).Build())
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
            var inner = outer.Scope(builder => builder.AddSingleton(42, typeof(int)));
            Action resolve = () => outer.Single<int>();
            resolve.Should().ThrowExactly<UnknownContractException>();
        }

        [Test]
        public void ScopedContainer_WhenDisposed_ShouldNotDisposeDependencyFromParentContainer()
        {
            using (var outer = new ContainerBuilder().AddSingleton(typeof(Disposable), typeof(Disposable)).Build())
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
                    inner.Parents.Count.Should().Be(1);
                    inner.Parents.Contains(outer).Should().BeTrue();
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

            child.Parents.Contains(parent).Should().BeTrue();
            parent.Children.Contains(child).Should().BeTrue();
            child.Dispose();
            parent.Children.Should().BeEmpty();
        }

        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder1()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .AddSingleton(new DisposeHook(() => { disposalOrder.Add("a"); }))
                .Build();

            var b = a.Scope(builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("b"); })));
            var c = b.Scope(builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("c"); })));
            var d = c.Scope(builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("d"); })));
            var e = d.Scope(builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("e"); })));
            
            a.Dispose();
            
            string.Join(",", disposalOrder).Should().BeEquivalentTo("e,d,c,b,a");
        }
        
        [Test]
        public void ContainerWithDisposables_ShouldDisposeInStackOrder2()
        {
            var disposalOrder = new List<string>();

            var a = new ContainerBuilder()
                .AddSingleton(new DisposeHook(() => { disposalOrder.Add("a"); }))
                .Build();

            var b = a.Scope(builder => builder.AddSingleton(new DisposeHook(() => { disposalOrder.Add("b"); })));
            
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

        [Test]
        public void ScopedContainer_ShouldInheritFromBothParents()
        {
            var parent0 = new ContainerBuilder().AddSingleton("A").Build();
            var parent1 = new ContainerBuilder().AddSingleton("B").Build();
            var parent2 = new ContainerBuilder().AddSingleton("C").Build();
            var child = new ContainerBuilder()
                .AddParent(parent0)
                .AddParent(parent1)
                .AddParent(parent2)
                .AddSingleton("D")
                .Build();

            string.Join("", child.All<string>()).Should().BeEquivalentTo("ABCD");
        }

        [Test] public void ScopedContainer_MultipleInheritance_SingleResolve()
        {
            var list = new List<int>();
            var parent = new ContainerBuilder().AddSingleton(list).Build();
            var subParent0 = new ContainerBuilder().AddParent(parent).Build();
            var subParent1 = new ContainerBuilder().AddParent(parent).Build();
            var child = new ContainerBuilder()
                .AddParent(subParent0)
                .AddParent(subParent1)
                .Build();

            child.Single<List<int>>();
        }
    }
}