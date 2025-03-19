using System;
using Reflex.Extensions;

namespace Reflex.Pooling
{
    internal sealed class SizeSpecificArrayPool<T> where T : new()
    {
        public const int InitialBucketSize = 4;
        private readonly T[][][] _buckets; // Similar to a Dictionary<int, Stack<T[]>> but all inlined for performance
        private int[] _rentals;

        public SizeSpecificArrayPool(int maxLength)
        {
            maxLength += 1;

            _buckets = new T[maxLength][][];
            _rentals = new int[maxLength];

            for (var length = 0; length < maxLength; length++)
            {
                var bucket = new T[InitialBucketSize][];

                for (var j = 0; j < bucket.Length; j++)
                {
                    bucket[j] = new T[length];
                }

                _buckets[length] = bucket;
            }
        }

        public T[] Rent(int length)
        {
            var maxLengthSupported = _buckets.Length - 1;
            if (length > maxLengthSupported)
            {
                return new T[length];
            }

            var bucket = _buckets[length];
            var rentalIndex = _rentals[length];

            if (rentalIndex >= bucket.Length)
            {
                var newSize = bucket.Length * 2;
                Array.Resize(ref bucket, newSize);

                for (var i = rentalIndex; i < newSize; i++)
                {
                    bucket[i] = new T[length];
                }
            }

            var array = bucket[rentalIndex];
            _rentals[length]++;
            return array;
        }

        public void Return(T[] array)
        {
            var length = array.Length;
            var maxLengthSupported = _buckets.Length - 1;

            if (length > maxLengthSupported)
            {
                return;
            }

            Array.Clear(array, 0, length);
            _rentals[length]--;
        }
    }
}