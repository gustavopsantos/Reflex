using System;

namespace Domain.Generics
{
    public class RingBuffer<T>
    {
        private int _offset;
        private readonly T[] _array;
        public int Capacity => _array.Length;
        public int Length { get; private set; }
        public T this[int i] => _array[Circle(_offset - i, Length)];

        public RingBuffer(int capacity)
        {
            ValidateCapacity(capacity);
            _array = new T[capacity];
            _offset = -1;
        }

        public void Push(T element)
        {
            _offset = (_offset + 1) % Capacity;
            _array[_offset] = element;
            Length = Math.Min(Length + 1, Capacity);
        }

        private static int Circle(int number, int rangeExclusive)
        {
            var result = number % rangeExclusive;

            if ((result < 0 && rangeExclusive > 0) || (result > 0 && rangeExclusive < 0))
            {
                result += rangeExclusive;
            }

            return result;
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity),
                    capacity,
                    "Capacity should not be less or equal to zero.");
            }
        }
    }
}