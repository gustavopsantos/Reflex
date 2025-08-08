using Reflex.Attributes;

namespace Level1
{
    namespace Level2
    {
        public partial class MockContainer
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
}