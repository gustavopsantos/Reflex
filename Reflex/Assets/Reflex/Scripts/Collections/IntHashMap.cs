using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex {
    
    //based on .Net5 BCL
    //https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/Dictionary.cs
    //but hash function changed to bit AND, cause we work with n^2 - 1 capacity
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class IntHashMap<T> : IEnumerable<int> {
        public int length;
        public int capacity;
        public int capacityMinusOne;
        public int lastIndex;
        public int freeIndex;

        public int[] buckets;

        public T[]    data;
        public Slot[] slots;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntHashMap(in int capacity = 0) {
            this.lastIndex = 0;
            this.length    = 0;
            this.freeIndex = -1;

            this.capacityMinusOne = HashHelpers.GetCapacity(capacity);
            this.capacity         = this.capacityMinusOne + 1;

            this.buckets = new int[this.capacity];
            this.slots   = new Slot[this.capacity];
            this.data    = new T[this.capacity];
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() {
            Enumerator e;
            e.hashMap = this;
            e.index   = 0;
            e.current = default;
            return e;
        }

        [Serializable]
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        public struct Slot {
            public int key;
            public int next;
        }

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        public struct Enumerator : IEnumerator<int> {
            public IntHashMap<T> hashMap;

            public int index;
            public int current;

            public bool MoveNext() {
                for (; this.index < this.hashMap.lastIndex; ++this.index) {
                    ref var slot = ref this.hashMap.slots[this.index];
                    if (slot.key - 1 < 0) {
                        continue;
                    }

                    this.current = this.index;
                    ++this.index;

                    return true;
                }

                this.index   = this.hashMap.lastIndex + 1;
                this.current = default;
                return false;
            }

            public int Current => this.current;

            object IEnumerator.Current => this.current;

            void IEnumerator.Reset() {
                this.index   = 0;
                this.current = default;
            }

            public void Dispose() {
            }
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public static class IntHashMapExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Add<T>(this IntHashMap<T> hashMap, in int key, in T value, out int slotIndex) {
            var rem = key & hashMap.capacityMinusOne;

            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = hashMap.slots[i].next) {
                if (hashMap.slots[i].key - 1 == key) {
                    slotIndex = -1;
                    return false;
                }
            }

            if (hashMap.freeIndex >= 0) {
                slotIndex         = hashMap.freeIndex;
                hashMap.freeIndex = hashMap.slots[slotIndex].next;
            }
            else {
                if (hashMap.lastIndex == hashMap.capacity) {
                    var newCapacityMinusOne = HashHelpers.ExpandCapacity(hashMap.length);
                    var newCapacity         = newCapacityMinusOne + 1;

                    ArrayHelpers.Grow(ref hashMap.slots, newCapacity);
                    ArrayHelpers.Grow(ref hashMap.data, newCapacity);

                    var newBuckets = new int[newCapacity];

                    for (int i = 0, len = hashMap.lastIndex; i < len; ++i) {
                        ref var slot = ref hashMap.slots[i];

                        var newResizeIndex = (slot.key - 1) & newCapacityMinusOne;
                        slot.next = newBuckets[newResizeIndex] - 1;

                        newBuckets[newResizeIndex] = i + 1;
                    }

                    hashMap.buckets          = newBuckets;
                    hashMap.capacity         = newCapacity;
                    hashMap.capacityMinusOne = newCapacityMinusOne;

                    rem = key & hashMap.capacityMinusOne;
                }

                slotIndex = hashMap.lastIndex;
                ++hashMap.lastIndex;
            }

            ref var newSlot = ref hashMap.slots[slotIndex];

            newSlot.key  = key + 1;
            newSlot.next = hashMap.buckets[rem] - 1;

            hashMap.data[slotIndex] = value;

            hashMap.buckets[rem] = slotIndex + 1;

            ++hashMap.length;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this IntHashMap<T> hashMap, in int key, in T value, out int slotIndex) {
            var rem = key & hashMap.capacityMinusOne;

            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = hashMap.slots[i].next) {
                if (hashMap.slots[i].key - 1 == key) {
                    hashMap.data[i] = value;
                    slotIndex       = i;
                    return;
                }
            }

            if (hashMap.freeIndex >= 0) {
                slotIndex         = hashMap.freeIndex;
                hashMap.freeIndex = hashMap.slots[slotIndex].next;
            }
            else {
                if (hashMap.lastIndex == hashMap.capacity) {
                    var newCapacityMinusOne = HashHelpers.ExpandCapacity(hashMap.length);
                    var newCapacity         = newCapacityMinusOne + 1;

                    ArrayHelpers.Grow(ref hashMap.slots, newCapacity);
                    ArrayHelpers.Grow(ref hashMap.data, newCapacity);

                    var newBuckets = new int[newCapacity];

                    for (int i = 0, len = hashMap.lastIndex; i < len; ++i) {
                        ref var slot           = ref hashMap.slots[i];
                        var     newResizeIndex = (slot.key - 1) & newCapacityMinusOne;
                        slot.next = newBuckets[newResizeIndex] - 1;

                        newBuckets[newResizeIndex] = i + 1;
                    }

                    hashMap.buckets          = newBuckets;
                    hashMap.capacity         = newCapacity;
                    hashMap.capacityMinusOne = newCapacityMinusOne;

                    rem = key & hashMap.capacityMinusOne;
                }

                slotIndex = hashMap.lastIndex;
                ++hashMap.lastIndex;
            }

            ref var newSlot = ref hashMap.slots[slotIndex];

            newSlot.key  = key + 1;
            newSlot.next = hashMap.buckets[rem] - 1;

            hashMap.data[slotIndex] = value;

            hashMap.buckets[rem] = slotIndex + 1;

            ++hashMap.length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Remove<T>(this IntHashMap<T> hashMap, in int key, out T lastValue) {
            var rem = key & hashMap.capacityMinusOne;

            int next;
            var num = -1;
            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next) {
                ref var slot = ref hashMap.slots[i];
                if (slot.key - 1 == key) {
                    if (num < 0) {
                        hashMap.buckets[rem] = slot.next + 1;
                    }
                    else {
                        hashMap.slots[num].next = slot.next;
                    }

                    lastValue = hashMap.data[i];

                    slot.key  = -1;
                    slot.next = hashMap.freeIndex;

                    --hashMap.length;
                    if (hashMap.length == 0) {
                        hashMap.lastIndex = 0;
                        hashMap.freeIndex = -1;
                    }
                    else {
                        hashMap.freeIndex = i;
                    }

                    return true;
                }

                next = slot.next;
                num  = i;
            }

            lastValue = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(this IntHashMap<T> hashMap, in int key) {
            var rem = key & hashMap.capacityMinusOne;

            int next;
            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next) {
                ref var slot = ref hashMap.slots[i];
                if (slot.key - 1 == key) {
                    return true;
                }

                next = slot.next;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue<T>(this IntHashMap<T> hashMap, in int key, out T value) {
            var rem = key & hashMap.capacityMinusOne;

            int next;
            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next) {
                ref var slot = ref hashMap.slots[i];
                if (slot.key - 1 == key) {
                    value = hashMap.data[i];
                    return true;
                }

                next = slot.next;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValueByKey<T>(this IntHashMap<T> hashMap, in int key) {
            var rem = key & hashMap.capacityMinusOne;

            int next;
            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next) {
                ref var slot = ref hashMap.slots[i];
                if (slot.key - 1 == key) {
                    return hashMap.data[i];
                }

                next = slot.next;
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValueByIndex<T>(this IntHashMap<T> hashMap, in int index) => hashMap.data[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetKeyByIndex<T>(this IntHashMap<T> hashMap, in int index) => hashMap.slots[index].key - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TryGetIndex<T>(this IntHashMap<T> hashMap, in int key) {
            var rem = key & hashMap.capacityMinusOne;

            int next;
            for (var i = hashMap.buckets[rem] - 1; i >= 0; i = next) {
                ref var slot = ref hashMap.slots[i];
                if (slot.key - 1 == key) {
                    return i;
                }

                next = slot.next;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this IntHashMap<T> hashMap, T[] array) {
            var num = 0;
            for (int i = 0, li = hashMap.lastIndex; i < li && num < hashMap.length; ++i) {
                if (hashMap.slots[i].key - 1 < 0) {
                    continue;
                }

                array[num] = hashMap.data[i];
                ++num;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this IntHashMap<T> hashMap) {
            if (hashMap.lastIndex <= 0) 
            {
                return;
            }

            Array.Clear(hashMap.slots, 0, hashMap.lastIndex);
            Array.Clear(hashMap.buckets, 0, hashMap.capacity);
            Array.Clear(hashMap.data, 0, hashMap.capacity);

            hashMap.lastIndex = 0;
            hashMap.length    = 0;
            hashMap.freeIndex = -1;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class ArrayHelpers 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Grow<T>(ref T[] array, int newSize) 
        {
            var newArray = new T[newSize];
            Array.Copy(array, 0, newArray, 0, array.Length);
            array = newArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(T[] array, T value, EqualityComparer<T> comparer) 
        {
            for (int i = 0, length = array.Length; i < length; ++i) 
            {
                if (comparer.Equals(array[i], value)) 
                {
                    return i;
                }
            }

            return -1;
        }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class HashHelpers 
    {
        //https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Collections/HashHelpers.cs#L32
        //different primes to fit n^2 - 1
        //todo expand to maxInt
        internal static readonly int[] capacities = 
        {
            3,
            15,
            63,
            255,
            1023,
            4095,
            16383,
            65535,
            262143,
            1048575,
            4194303,
        };

        public static int ExpandCapacity(int oldSize) 
        {
            var min = oldSize << 1;
            return min > 2146435069U && 2146435069 > oldSize ? 2146435069 : GetCapacity(min);
        }

        public static int GetCapacity(int min) 
        {
            for (int index = 0, length = capacities.Length; index < length; ++index) 
            {
                var prime = capacities[index];
                if (prime >= min) 
                {
                    return prime;
                }
            }

            throw new Exception("Prime is too big");
        }
    }
}