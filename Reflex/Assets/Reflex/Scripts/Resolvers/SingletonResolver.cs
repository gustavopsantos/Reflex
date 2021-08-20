using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class SingletonResolver
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Resolve(int hashContract, Container container)
        {
            //inline internal method for performance
            if (container.Singletons.TryGetValue(hashContract, out var instance))
            {
                return instance;
            }

            return CreateAndRegisterSingletonInstance(hashContract, container);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateAndRegisterSingletonInstance(int hashContract, Container container)
        {
            var instance = TransientResolver.Resolve(hashContract, container);
            //inline internal method for performance
            container.Singletons.Add(hashContract, instance, out _);
            return instance;
        }
    }
}