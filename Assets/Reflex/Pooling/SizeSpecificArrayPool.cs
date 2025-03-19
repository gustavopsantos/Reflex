using Reflex.Extensions;

namespace Reflex.Pooling
{
    internal sealed class SizeSpecificArrayPool<T> where T : new()
    {
        private T[][] _pool;

        public SizeSpecificArrayPool(int initialSize)
        {
            _pool = BuildPool(initialSize);
        }

        public T[] Rent(int size)
        {
            if (_pool.Length - 1 < size)
            {
                _pool = BuildPool(size.CeilPowerOf2() + 1);
            }

            return _pool[size];
        }

        public int GetPoolSize()
        {
            return _pool.Length;
        }

        private static T[][] BuildPool(int size)
        {
            var pool = new T[size][];

            for (var i = 0; i < pool.Length; i++)
            {
                pool[i] = new T[i];
            }

            return pool;
        }
    }
}