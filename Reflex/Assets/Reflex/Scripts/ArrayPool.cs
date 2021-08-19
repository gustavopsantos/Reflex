using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class ArrayPool<T>
    {
        private static readonly IntHashMap<FastStack<T[]>> registry;
        
        static ArrayPool()
        {
            registry = new IntHashMap<FastStack<T[]>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T[] Rent(int size)
        {
            if (registry.TryGetValue(size, out var stack))
            {
                if (stack.length > 0) {
                    return stack.Pop();
                }
            }

            return new T[size];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Return(T[] array)
        {
            if (registry.TryGetValue(array.Length, out var stack))
            {
                stack.Push(array);
            }
            else {
                stack = new FastStack<T[]>();
                stack.Push(array);
                registry.Add(array.Length, stack, out _);
            }
        }
    }
}