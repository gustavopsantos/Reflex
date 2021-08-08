namespace Benchmark.NestedModel
{
    public class C : IC
    {
        private readonly ID _d;

        public C(ID d)
        {
            _d = d;
        }
    }
}