using Reflex.Attributes;

namespace Level1
{
    namespace Level2
    {
        public partial class MockContainer
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
}