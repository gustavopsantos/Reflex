using System;

namespace Reflex.Injectors
{
	internal static class ConstructorInjector
	{
		internal static T ConstructAndInject<T>(Container container)
		{
			return (T) ConstructAndInject(typeof(T), container);
		}
		
		internal static object ConstructAndInject(Type concrete, Container container)
		{
			var info = TypeInfoCache.Cache[concrete];
			var objects = ArrayPool<object>.Shared.Rent(info.ConstructorParameters.Length);
			GetConstructionObjects(info.ConstructorParameters, container, ref objects);

			try
			{
				return info.Activator(objects);
			}
			catch (Exception e)
			{
				throw new Exception(
					$"Error occurred while instantiating object with type '{concrete.GetFormattedName()}'\n\n{e.Message}");
			}
			finally
			{
				ArrayPool<object>.Shared.Return(objects);
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