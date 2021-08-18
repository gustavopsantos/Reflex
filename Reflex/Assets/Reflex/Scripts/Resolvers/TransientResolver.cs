using System;
using Reflex.Scripts.Utilities;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class TransientResolver
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Resolve(int hashContract, Container container)
        {
            //inline internal method for performance
            var concrete = container.Bindings.GetValueByKey(hashContract).ConcreteHashCode;
            var info     = TypeInfoCache.Registry.GetValueByKey(concrete);
            var objects  = ArrayPool<object>.Rent(info.ConstructorParameters.Length);

            //inline internal method for performance
            var parameters = info.ConstructorParameters;
            
            for (int i = 0; i < parameters.Length; ++i)
            {
                objects[i] = container.Resolve(parameters[i]);
            }

            try
            {
                var instance = info.Activator(objects);
                ArrayPool<object>.Return(objects);
                return instance;
            }
            catch (Exception e)
            {
                throw new Exception($"Error occurred while instantiating object with type '{info.Type.GetFormattedName()}'\n\n{e.Message}");
            }
        }
    }
}