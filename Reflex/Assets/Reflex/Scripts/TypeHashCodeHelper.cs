using Unity.IL2CPP.CompilerServices;

namespace Reflex 
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    //replace slow typeof().GetHashCode() with generic cache
    internal static class TypeHashCodeHelper<T> 
    {
        internal static readonly int Hash;

        static TypeHashCodeHelper() 
        {
            Hash = typeof(T).GetHashCode();
        }
    }
}