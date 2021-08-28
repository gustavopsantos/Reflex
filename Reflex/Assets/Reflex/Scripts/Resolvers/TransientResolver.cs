using System;
using Reflex.Scripts.Utilities;

namespace Reflex
{
    internal static class TransientResolver
    {
        internal static object Resolve(Type contract, Container container)
        {
            return Construct(container.GetConcreteTypeFor(contract), container);
        }

        private static object Construct(Type concrete, Container container)
        {
            var info = TypeInfoCache.GetClassInfo(concrete);
            var objects = ArrayPool<object>.Shared.Rent(info.ConstructorParameters.Length);
            GetConstructionObjects(info.ConstructorParameters, container, ref objects);

            try
            {
                var instance = info.Activator(objects);
                ArrayPool<object>.Shared.Return(objects);
                return instance;
            }
            catch (Exception e)
            {
                throw new Exception($"Error occurred while instantiating object with type '{concrete.GetFormattedName()}'\n\n{e.Message}");
            }
        }

        private static void GetConstructionObjects(Type[] parameters, Container container, ref object[] array)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                array[i] = container.Resolve(parameters[i]);
            }
        }
    }
}