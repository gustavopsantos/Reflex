using System;
using FluentAssertions;
using NUnit.Framework;

namespace Reflex.Tests
{
    public class ScopedContainerTests
    {
        private class Foo : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public Foo()
            {
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        private class DependsOnFoo : IDisposable
        {
            public Foo Foo { get; }

            public DependsOnFoo(Foo foo)
            {
                Foo = foo;
            }

            public void Dispose()
            {
            }
        }

        [Test]
        public void InnerContainerCanResolveBindingFromOuterScope()
        {
            using (var outer = new Container(string.Empty))
            {
                outer.BindInstance(42);

                using (var inner = outer.Scope(string.Empty))
                {
                    inner.Resolve<int>().Should().Be(42);
                }
            }
        }

        [Test]
        public void InnerContainerCanOverrideBindingFromOuterScope()
        {
            using (var outer = new Container(string.Empty))
            {
                outer.BindInstance("outer");

                using (var inner = outer.Scope(string.Empty))
                {
                    inner.BindInstance("inner");
                    inner.Resolve<string>().Should().Be("inner");
                }
            }
        }

        [Test]
        public void OuterContainerShouldNotBeAffectedByInnerContainerOverride()
        {
            using (var outer = new Container(string.Empty))
            {
                outer.BindInstance("outer");

                using (var inner = outer.Scope(string.Empty))
                {
                    inner.BindInstance("inner");
                }

                outer.Resolve<string>().Should().Be("outer");
            }
        }

        [Test]
        public void InnerScopeBindingCanResolveOuterDependency()
        {
            using (var outer = new Container(string.Empty))
            {
                var foo = new Foo();
                outer.BindInstance(foo);

                using (var inner = outer.Scope(string.Empty))
                {
                    inner.BindSingleton<DependsOnFoo, DependsOnFoo>();
                    inner.Resolve<DependsOnFoo>().Foo.Should().Be(foo);
                }
            }
        }

        [Test]
        public void DisposingInnerScopeShouldNotDisposeInstancesFromOuterScope()
        {
            using (var outer = new Container(string.Empty))
            {
                outer.BindSingleton<Foo, Foo>();

                using (var inner = outer.Scope(string.Empty))
                {
                    inner.BindSingleton<DependsOnFoo, DependsOnFoo>();
                    inner.Resolve<Foo>();
                    inner.Resolve<DependsOnFoo>().Foo.IsDisposed.Should().BeFalse();
                }

                outer.Resolve<Foo>().IsDisposed.Should().BeFalse();
            }
        }

        [Test]
        public void ResolvingContainerFromInnerScopeShouldResolveInner()
        {
            using (var outer = new Container(string.Empty))
            {
                using (var inner = outer.Scope(string.Empty))
                {
                    inner.Resolve<Container>().Should().Be(inner);
                }
            }
        }

        [Test]
        public void ResolvingContainerFromOuterScopeShouldResolveOuter()
        {
            using (var outer = new Container(string.Empty))
            {
                using (var inner = outer.Scope(string.Empty))
                {
                    inner.Resolve<Container>();
                }

                outer.Resolve<Container>().Should().Be(outer);
            }
        }

        private interface IManager : IDisposable
        {
            bool Disposed { get; }
        }

        public class Manager : IManager
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        [Test]
        public void InnerScopedContainerShouldNotDisposeOuterBindings()
        {
            var outer = new Container("Outer");
            outer.BindSingleton<Foo, Foo>();
            var inner = outer.Scope("Inner");
            inner.Resolve<Foo>();
            inner.Dispose();
            outer.Resolve<Foo>().IsDisposed.Should().BeFalse();
        }
    }
}