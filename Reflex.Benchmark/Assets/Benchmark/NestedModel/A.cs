namespace Benchmark.NestedModel
{
    public class A : IA
    {
        private readonly IB _b;

        public A(IB b)
        {
            _b = b;
        }
    }
}