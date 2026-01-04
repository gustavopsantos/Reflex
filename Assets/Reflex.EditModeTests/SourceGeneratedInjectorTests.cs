using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Reflex.Attributes;
using Reflex.Extensions;
using Reflex.Injectors;
using Reflex.Core;
using Reflex.Enums;

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
            const string testDependency = "Hello World";

            var container = new ContainerBuilder()
                .RegisterValue(testDependency, Lifetime.Singleton)
                .Build();

            var sample = new MockParent.MockUsageChild();

            IAttributeInjectionContract handle = sample;
            AttributeInjector.Inject(handle, container);

            Assert.AreEqual(sample.BaseSampleField, testDependency);
            Assert.AreEqual(sample.BaseSampleProperty, testDependency);
            Assert.AreEqual(sample.BaseSampleMethodParameter, testDependency);

            Assert.AreEqual(sample.ChildSampleField, testDependency);
            Assert.AreEqual(sample.ChildSampleProperty, testDependency);
            Assert.AreEqual(sample.ChildSampleMethodParameter, testDependency);
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