using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    internal class SourceGeneratorAttributeInjectorTests
    {
        [Test]
        public void MockUsage_ImplementsInterface()
        {
            var sample = new MockParent.MockUsageChild();
            Assert.IsTrue(sample is Reflex.Injectors.IAttributeInjectionContract);
        }

        [Test]
        public void MockUsage_Injection()
        {
            var TestDependency = "Hello World";

            var container = new ContainerBuilder()
                .AddSingleton(TestDependency)
                .Build();

            var sample = new MockParent.MockUsageChild();

            IAttributeInjectionContract handle = sample;
            AttributeInjector.Inject(handle, container);

            Assert.AreEqual(sample.BaseSampleField, TestDependency);
            Assert.AreEqual(sample.BaseSampleProperty, TestDependency);
            Assert.AreEqual(sample.BaseSampleMethodParameter, TestDependency);

            Assert.AreEqual(sample.ChildSampleField, TestDependency);
            Assert.AreEqual(sample.ChildSampleProperty, TestDependency);
            Assert.AreEqual(sample.ChildSampleMethodParameter, TestDependency);
        }
    }

    public partial class MockParent
    {
        [SourceGeneratorInjectable]
        public partial class MockUsageBase
        {
            [Inject]
            public string BaseSampleField;

            [Inject]
            public string BaseSampleProperty { get; private set; }

            public string BaseSampleMethodParameter;
            [Inject]
            public void BaseSampleMethod(string SampleMethodParameter)
            {
                this.BaseSampleMethodParameter = SampleMethodParameter;
            }
        }

        [SourceGeneratorInjectable]
        public partial class MockUsageChild : MockUsageBase
        {
            [Inject]
            public string ChildSampleField;

            [Inject]
            public string ChildSampleProperty { get; private set; }

            public string ChildSampleMethodParameter;
            [Inject]
            public void ChildSampleMethod(string SampleMethodParameter)
            {
                this.ChildSampleMethodParameter = SampleMethodParameter;
            }
        }
    }
}