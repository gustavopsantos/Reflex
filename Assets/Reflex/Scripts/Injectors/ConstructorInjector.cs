using System;
using Reflex.Scripts;

namespace Reflex.Injectors
{
	internal static class ConstructorInjector
	{
		internal static T ConstructAndInject<T>(IContainer container)
		{
			return (T) ConstructAndInject(typeof(T), container);
		}
		
		internal static object ConstructAndInject(Type concrete, IContainer container)
		{
			var info = TypeInfoRepository.Repository.Fetch(concrete);
			var objects = ArrayPool<object>.Shared.Rent(info.ConstructorParameters.Length);
			GetConstructionObjects(info.ConstructorParameters, container, ref objects);

			try
			{
				var instance = info.Activator(objects);
				if (instance is IDisposable disposable)
				{
					container.AddDisposable(disposable);
				}
				ArrayPool<object>.Shared.Return(objects);
				return instance;
			}
			catch (Exception e)
			{
				throw new Exception($"Error occurred while instantiating object with type '{concrete.GetFormattedName()}'\n\n{e.Message}");
			}
		}

		private static void GetConstructionObjects(Type[] parameters, IContainer container, ref object[] array)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = container.Resolve(parameters[i]);
			}
		}
	}
}