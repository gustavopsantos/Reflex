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
        internal int Length;
        internal int Capacity;

        internal T[] Data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal FastStack() 
        {
            Capacity = 4;
            Data     = new T[Capacity];
            Length   = 0;
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
            if (stack.Length == stack.Capacity) 
            {
                ArrayHelpers.Grow(ref stack.Data, stack.Capacity <<= 1);
            }

            stack.Data[stack.Length++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Pop<T>(this FastStack<T> stack) => stack.Data[--stack.Length];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Clear<T>(this FastStack<T> stack) 
        {
            stack.Data   = null;
            stack.Length = stack.Capacity = 0;
        }
    }
}