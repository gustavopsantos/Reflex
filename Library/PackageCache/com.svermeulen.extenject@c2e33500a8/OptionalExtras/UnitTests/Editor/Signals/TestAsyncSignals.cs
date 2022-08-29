using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Signals
{
    [TestFixture]
    public class TestAsyncSignals : ZenjectUnitTestFixture
    {
        static int CallCount;

        [Inject]
        SignalBus _signalBus = null;

        [Inject]
        Receiver1 _receiver1 = null;

        [Inject]
        Receiver2 _receiver2 = null;

        [Inject]
        TickableManager _tickManager = null;

        [InjectOptional]
        Foo _foo = null;

        public override void Setup()
        {
            base.Setup();

            SignalBusInstaller.Install(Container);
            ZenjectManagersInstaller.Install(Container);
        }

        [Test]
        public void TestBasicAsync()
        {
            Container.DeclareSignal<Signal1>().RunAsync();
            Container.DeclareSignal<Signal2>().RunAsync();

            Container.Bind<Receiver1>().AsSingle();
            Container.Bind<Receiver2>().AsSingle();

            Container.BindSignal<Signal1>().ToMethod<Receiver1>(x => x.OnSignal).FromResolve();
            Container.BindSignal<Signal2>().ToMethod<Receiver2>(x => x.OnSignal).FromResolve();

            Container.ResolveRoots();
            Container.Resolve<InitializableManager>().Initialize();

            Container.Inject(this);

            CallCount = 1;
            _receiver1.CallIndex = 0;
            _receiver2.CallIndex = 0;

            _signalBus.Fire<Signal1>();
            _signalBus.Fire<Signal2>();

            Assert.IsEqual(_receiver1.CallIndex, 0);
            Assert.IsEqual(_receiver2.CallIndex, 0);

            _tickManager.Update();

            Assert.IsEqual(_receiver1.CallIndex, 1);
            Assert.IsEqual(_receiver2.CallIndex, 2);
        }

        [Test]
        public void TestTickPriority()
        {
            Container.DeclareSignal<Signal1>().WithTickPriority(1);
            Container.DeclareSignal<Signal2>().WithTickPriority(-4);

            Container.BindInterfacesAndSelfTo<Foo>().AsSingle();

            Container.Bind<Receiver1>().AsSingle();
            Container.Bind<Receiver2>().AsSingle();

            Container.BindSignal<Signal1>().ToMethod<Receiver1>(x => x.OnSignal).FromResolve();
            Container.BindSignal<Signal2>().ToMethod<Receiver2>(x => x.OnSignal).FromResolve();

            Container.ResolveRoots();
            Container.Resolve<InitializableManager>().Initialize();

            Container.Inject(this);

            CallCount = 1;
            _receiver1.CallIndex = 0;
            _receiver2.CallIndex = 0;
            _foo.CallIndex = 0;

            _signalBus.Fire<Signal1>();
            _signalBus.Fire<Signal2>();

            Assert.IsEqual(_receiver1.CallIndex, 0);
            Assert.IsEqual(_receiver2.CallIndex, 0);
            Assert.IsEqual(_foo.CallIndex, 0);

            _tickManager.Update();

            Assert.IsEqual(_receiver2.CallIndex, 1);
            Assert.IsEqual(_foo.CallIndex, 2);
            Assert.IsEqual(_receiver1.CallIndex, 3);
        }

        public class Foo : ITickable
        {
            public int CallIndex
            {
                get; set;
            }

            public void Tick()
            {
                CallIndex = CallCount++;
            }
        }

        public class Signal1
        {
        }

        public class Signal2
        {
        }

        public class Receiver1
        {
            public int CallIndex
            {
                get; set;
            }

            public void OnSignal()
            {
                CallIndex = CallCount++;
            }
        }

        public class Receiver2
        {
            public int CallIndex
            {
                get; set;
            }

            public void OnSignal()
            {
                CallIndex = CallCount++;
            }
        }
    }
}

