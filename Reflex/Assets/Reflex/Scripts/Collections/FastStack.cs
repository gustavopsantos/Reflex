using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex {
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class FastStack<T> {
        public int length;
        public int capacity;

        public T[] data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FastStack() {
            this.capacity = 4;
            this.data     = new T[this.capacity];
            this.length   = 0;
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public static class IntStackExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push<T>(this FastStack<T> stack, in T value) {
            if (stack.length == stack.capacity) {
                ArrayHelpers.Grow(ref stack.data, stack.capacity <<= 1);
            }

            stack.data[stack.length++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pop<T>(this FastStack<T> stack) {
            return stack.data[--stack.length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this FastStack<T> stack) {
            stack.data   = null;
            stack.length = stack.capacity = 0;
        }
    }
}