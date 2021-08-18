using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class MethodResolver
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Resolve(int hashContract, Container container)
        {
            if (container.Bindings.TryGetValue(hashContract, out var binding))
            {
                if (binding.Scope == BindingScope.Method) 
                {
                    return binding.Method.Invoke();
                }
            }
            
            throw new UnknownMethodException($"Cannot resolve method of type hash '{hashContract}'.");
        }
    }
}