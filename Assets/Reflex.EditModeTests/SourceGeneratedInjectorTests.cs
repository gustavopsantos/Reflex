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
            var sample = new MockParent.MockUsage();
            Assert.IsTrue(sample is Reflex.Injectors.IAttributeInjectionContract);
        }

        [Test]
        public void MockUsage_Injection()
        {
            var TestDependency = "Hello World";

            var container = new ContainerBuilder()
                .AddSingleton(TestDependency)
                .Build();

            var sample = new MockParent.MockUsage();

            AttributeInjector.Inject(sample, container);

            Assert.AreEqual(sample.SampleField, TestDependency);
            Assert.AreEqual(sample.SampleProperty, TestDependency);
            Assert.AreEqual(sample.SampleMethodParameter, TestDependency);
        }
    }

    public partial class MockParent
    {
        [SourceGeneratorInjectable]
        public partial class MockUsage
        {
            [Inject]
            public string SampleField;

            [Inject]
            public string SampleProperty { get; private set; }

            public string SampleMethodParameter;
            [Inject]
            public void SampleMethod(string SampleMethodParameter)
            {
                this.SampleMethodParameter = SampleMethodParameter;
            }
        }
    }
}