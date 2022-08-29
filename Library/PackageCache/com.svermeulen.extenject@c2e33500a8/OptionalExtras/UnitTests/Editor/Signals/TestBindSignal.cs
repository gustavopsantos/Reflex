using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Signals
{
    [TestFixture]
    public class TestBindSignal : ZenjectUnitTestFixture
    {
        [SetUp]
        public void CommonInstall()
        {
            SignalBusInstaller.Install(Container);
            Container.Inject(this);
        }

        [Test]
        public void TestIncompleteBinding()
        {
            Container.DeclareSignal<FooSignal>();
            Container.BindSignal<FooSignal>();

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestBindWithoutDeclaration()
        {
            Container.BindSignal<FooSignal>().ToMethod(() => {});

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestStaticMethodHandler()
        {
            Container.DeclareSignal<FooSignal>();

            bool received = false;

            Container.BindSignal<FooSignal>().ToMethod(() => received = true);
            Container.ResolveRoots();

            var signalBus = Container.Resolve<SignalBus>();

            Assert.That(!received);
            signalBus.Fire<FooSignal>();
            Assert.That(received);
        }

        [Test]
        public void TestStaticMethodHandlerWithArgs()
        {
            Container.DeclareSignal<FooSignal>();

            FooSignal received = null;

            Container.BindSignal<FooSignal>().ToMethod(x => received = x);
            Container.ResolveRoots();

            var signalBus = Container.Resolve<SignalBus>();
            var sent = new FooSignal();

            Assert.IsNull(received);
            signalBus.Fire(sent);
            Assert.IsEqual(received, sent);
        }

        [Test]
        public void TestInstanceMethodHandler()
        {
            Container.DeclareSignal<FooSignal>();

            var qux = new Qux();
            Container.BindSignal<FooSignal>()
                .ToMethod<Qux>(x => x.OnFoo).From(b => b.FromInstance(qux));
            Container.ResolveRoots();

            var signalBus = Container.Resolve<SignalBus>();

            Assert.That(!qux.HasRecievedSignal);
            signalBus.Fire<FooSignal>();
            Assert.That(qux.HasRecievedSignal);
        }

        [Test]
        public void TestInstanceMethodHandler2()
        {
            Container.DeclareSignal<FooSignal>();

            var gorp = new Gorp();
            Container.BindSignal<FooSignal>()
                .ToMethod<Gorp>(x => x.OnFoo).From(b => b.FromInstance(gorp));
            Container.ResolveRoots();

            var signalBus = Container.Resolve<SignalBus>();
            var sent = new FooSignal();

            Assert.IsNull(gorp.ReceivedValue);
            signalBus.Fire(sent);
            Assert.IsEqual(gorp.ReceivedValue, sent);
        }

        [Test]
        public void TestMoveIntoDirectSubContainers()
        {
            Container.DeclareSignal<FooSignal>();

            var gorp = new Gorp();

            Container.BindSignal<FooSignal>()
                .ToMethod<Gorp>(x => x.OnFoo).From(b => b.FromInstance(gorp)).MoveIntoDirectSubContainers();
            Container.ResolveRoots();

            var signalBus1 = Container.Resolve<SignalBus>();
            var sent = new FooSignal();

            Assert.IsNull(gorp.ReceivedValue);
            signalBus1.Fire(sent);
            Assert.IsNull(gorp.ReceivedValue);

            var subContainer = Container.CreateSubContainer();
            subContainer.ResolveRoots();

            var signalBus2 = Container.Resolve<SignalBus>();

            Assert.IsNull(gorp.ReceivedValue);
            signalBus2.Fire(sent);
            Assert.IsEqual(gorp.ReceivedValue, sent);
        }

        public class Qux
        {
            public void OnFoo()
            {
                HasRecievedSignal = true;
            }

            public bool HasRecievedSignal
            {
                get; private set;
            }
        }

        public class Gorp
        {
            public void OnFoo(FooSignal foo)
            {
                ReceivedValue = foo;
            }

            public FooSignal ReceivedValue
            {
                get; private set;
            }
        }

        public class FooSignal
        {
        }
    }
}


