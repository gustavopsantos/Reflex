using Level1.Level2;

using Reflex.Core;
using Reflex.Injectors;

namespace Reflex.Generator.Injector.Tests
{
    [TestClass]
    public sealed class MockUsageTests
    {
        [TestMethod]
        public void ImplementsInterface() //Somewhat un-needed since the injection test bellow will cause a compile time exception first, but whatever ¯\_(ツ)_/¯
        {
            var sample = new MockContainer.MockUsage();
            Assert.IsTrue(sample is Reflex.Injectors.IAttributeInjectionContract);
        }

        [TestMethod]
        public void Injection()
        {
            var TestDependency = "Hello World";

            var container = new Container();

            container.AddSingleton(TestDependency);

            var sample = new MockContainer.MockUsage();

            AttributeInjector.Inject(sample, container);

            Assert.AreEqual(sample.SampleField, TestDependency);
            Assert.AreEqual(sample.SampleProperty, TestDependency);
            Assert.AreEqual(sample.SampleMethodParameter, TestDependency);
        }
    }
}