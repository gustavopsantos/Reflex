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
            var sample = new MockContainer.MockUsageChild();
            Assert.IsTrue(sample is Reflex.Injectors.IAttributeInjectionContract);
        }

        [TestMethod]
        public void Injection()
        {
            var TestDependency = "Hello World";

            var container = new Container();

            container.AddSingleton(TestDependency);

            var sample = new MockContainer.MockUsageChild();

            AttributeInjector.Inject(sample, container);

            Assert.AreEqual(sample.BaseSampleField, TestDependency);
            Assert.AreEqual(sample.BaseSampleProperty, TestDependency);
            Assert.AreEqual(sample.BaseSampleMethodParameter, TestDependency);

            Assert.AreEqual(sample.ChildSampleField, TestDependency);
            Assert.AreEqual(sample.ChildSampleProperty, TestDependency);
            Assert.AreEqual(sample.ChildSampleMethodParameter, TestDependency);
        }
    }
}