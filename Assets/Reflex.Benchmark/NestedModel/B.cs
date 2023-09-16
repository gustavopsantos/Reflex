namespace Reflex.Microsoft.Benchmark.NestedModel
{
    internal class B : IB
    {
        private readonly IC _c;

        public B(IC c)
        {
            _c = c;
        }
    }
}