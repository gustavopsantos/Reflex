using System.Collections.Concurrent;

namespace Reflex.Microsoft.Buffers
{
    internal sealed class ExactArrayPool<T>
    {
        public static readonly ExactArrayPool<T> Shared = new();

        private readonly ConcurrentDictionary<int, ConcurrentQueue<T[]>> _buckets = new();

        public T[] Rent(int size)
        {
            if (!_buckets.TryGetValue(size, out ConcurrentQueue<T[]> bucket))
            {
                bucket = new ConcurrentQueue<T[]>();
                _buckets.TryAdd(size, bucket);
            }
            
            return bucket.TryDequeue(out T[] array) ? array : new T[size];
        }

        public void Return(T[] array)
        {
            if (_buckets.TryGetValue(array.Length, out ConcurrentQueue<T[]> bucket))
            {
                bucket.Enqueue(array);
            }
        }
    }
}