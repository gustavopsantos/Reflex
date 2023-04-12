namespace Reflex.Benchmark.NestedModel
{
    internal class A : IA
    {
        private readonly IB _b;

        public A(IB b)
        {
            _b = b;
        }
    }
}