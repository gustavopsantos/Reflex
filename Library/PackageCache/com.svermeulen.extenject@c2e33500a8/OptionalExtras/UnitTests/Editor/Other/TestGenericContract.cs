using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Other
{
    [TestFixture]
    public class TestGenericContract : ZenjectUnitTestFixture
    {
        class Test1<T>
        {
            public T Data;
        }

        class Test2
        {
        }

        [Test]
        public void TestToSingle()
        {
            Container.Bind(typeof(Test1<>)).AsSingle().NonLazy();

            var test1 = Container.Resolve<Test1<int>>();
            Assert.That(test1.Data == 0);
            test1.Data = 5;

            var test2 = Container.Resolve<Test1<int>>();

            Assert.That(test2 == test1);
            Assert.That(test1.Data == 5);
        }

        [Test]
        public void TestToTransient()
        {
            Container.Bind(typeof(Test1<>)).AsTransient().NonLazy();

            var test1 = Container.Resolve<Test1<int>>();
            Assert.That(test1.Data == 0);

            var test2 = Container.Resolve<Test1<int>>();
            Assert.That(test2.Data == 0);
            Assert.That(test2 != test1);

            Container.Resolve<Test1<string>>();
            Container.Resolve<Test1<List<int>>>();
            Container.Resolve<Test1<Test2>>();
        }

        interface IFoo<T>
        {
        }

        interface IBar<T>
        {
        }

        class Test2<T> : IFoo<T>, IBar<T>
        {
        }

        [Test]
        public void TestToSingleMultipleContracts()
        {
            Container.Bind(typeof(IFoo<>), typeof(IBar<>)).To(typeof(Test2<>)).AsSingle();

            var foo = Container.Resolve<IFoo<int>>();
            Assert.That(foo is Test2<int>);

            var bar = Container.Resolve<IBar<int>>();
            Assert.That(bar is Test2<int>);

            Assert.IsEqual(foo, bar);
            Assert.IsEqual(foo, Container.Resolve<IFoo<int>>());
            Assert.IsEqual(bar, Container.Resolve<IBar<int>>());
        }

        public interface IQux {
        }

        public class Qux : IQux {
        }

        [Test]
        public void TestToSingleMultipleContractsMismatch()
        {
            Container.Bind(typeof(IQux), typeof(IFoo<>), typeof(IBar<>)).To(typeof(Test2<>), typeof(Qux)).AsSingle();

            var foo = Container.Resolve<IFoo<int>>();
            Assert.That(foo is Test2<int>);

            var bar = Container.Resolve<IBar<int>>();
            Assert.That(bar is Test2<int>);

            Assert.IsEqual(foo, bar);
            Assert.IsEqual(foo, Container.Resolve<IFoo<int>>());
            Assert.IsEqual(bar, Container.Resolve<IBar<int>>());

            var qux = Container.Resolve<IQux>();

            Assert.IsEqual(qux, Container.Resolve<IQux>());
        }
    }
}
