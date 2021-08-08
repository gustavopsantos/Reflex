namespace Benchmark.NestedModel
{
    public class B : IB
    {
        private readonly IC _c;

        public B(IC c)
        {
            _c = c;
        }
    }
}