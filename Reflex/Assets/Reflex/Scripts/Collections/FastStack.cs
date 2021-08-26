using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex 
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal sealed class FastStack<T> 
    {
        internal int length;
        internal int capacity;

        internal T[] data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal FastStack() 
        {
            capacity = 4;
            data = new T[capacity];
            length = 0;
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class IntStackExtensions 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Push<T>(this FastStack<T> stack, in T value) 
        {
            if (stack.length == stack.capacity) 
            {
                ArrayHelpers.Grow(ref stack.data, stack.capacity <<= 1);
            }

            stack.data[stack.length++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Pop<T>(this FastStack<T> stack)
        {
            return stack.data[--stack.length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Clear<T>(this FastStack<T> stack) 
        {
            stack.data   = null;
            stack.length = stack.capacity = 0;
        }
    }
}