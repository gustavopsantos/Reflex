using System.Collections.Concurrent;

namespace Reflex.Buffers
{
    internal sealed class ExactArrayPool<T>
    {
        public static readonly ExactArrayPool<T> Shared = new();

        private readonly ConcurrentDictionary<int, ConcurrentQueue<T[]>> _buckets = new();

        public T[] Rent(int size)
        {
            if (!_buckets.TryGetValue(size, out var bucket))
            {
                bucket = new ConcurrentQueue<T[]>();
                _buckets.TryAdd(size, bucket);
            }
            
            return bucket.TryDequeue(out var array) ? array : new T[size];
        }

        public void Return(T[] array)
        {
            if (_buckets.TryGetValue(array.Length, out var bucket))
            {
                bucket.Enqueue(array);
            }
        }
    }
}