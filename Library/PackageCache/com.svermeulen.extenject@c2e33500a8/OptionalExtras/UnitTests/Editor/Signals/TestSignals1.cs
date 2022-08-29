using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Signals
{
    [TestFixture]
    public class TestSignals1 : ZenjectUnitTestFixture
    {
        [Test]
        public void TestMissingDeclaration()
        {
            SignalBusInstaller.Install(Container);

            var signalBus = Container.Resolve<SignalBus>();

            Assert.Throws(() => signalBus.Fire<FooSignal>());
        }

        [Test]
        public void TestSubscribeAndUnsubscribe()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<FooSignal>();

            var signalBus = Container.Resolve<SignalBus>();

            bool received = false;

            Action callback = () => received = true;
            signalBus.Subscribe<FooSignal>(callback);

            Assert.That(!received);
            signalBus.Fire<FooSignal>();
            Assert.That(received);

            received = false;
            signalBus.Fire<FooSignal>();
            Assert.That(received);

            signalBus.Unsubscribe<FooSignal>(callback);

            received = false;
            signalBus.Fire<FooSignal>();
            Assert.That(!received);
        }

        [Test]
        public void TestWithArgs()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<FooSignal>();

            var signalBus = Container.Resolve<SignalBus>();

            FooSignal received = null;

            signalBus.Subscribe<FooSignal>(x => received = x);

            var sent = new FooSignal();

            signalBus.Fire(sent);

            Assert.IsEqual(received, sent);
        }

        [Test]
        public void TestUnsubscribeWithoutSubscribe()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<FooSignal>();

            var signalBus = Container.Resolve<SignalBus>();

            Action callback = () => {};

            Assert.Throws(() => signalBus.Unsubscribe<FooSignal>(callback));

            signalBus.TryUnsubscribe<FooSignal>(callback);

            signalBus.Subscribe<FooSignal>(callback);
            signalBus.Unsubscribe<FooSignal>(callback);
        }

        [Test]
        public void TestUntypedSubscribe()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<FooSignal>();
            Container.DeclareSignal<BarSignal>();

            var signalBus = Container.Resolve<SignalBus>();

            object received = null;

            signalBus.Subscribe(typeof(FooSignal), x =>
                {
                    Assert.That(x is FooSignal);
                    received = x;
                });

            var data = new FooSignal();

            signalBus.Fire(data);

            Assert.IsEqual(received, data);

            signalBus.Fire(new BarSignal());
        }

        public class FooSignal
        {
        }

        public class BarSignal
        {
            public string Value;
        }
    }
}
