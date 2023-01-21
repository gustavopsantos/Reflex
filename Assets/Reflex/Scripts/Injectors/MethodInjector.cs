using System;
using System.Reflection;

namespace Reflex.Injectors
{
	internal static class MethodInjector
	{
		public static void InjectMany(MethodInfo[] methods, object instance, Container container)
		{
			for (var i = 0; i < methods.Length; i++)
			{
				Inject(methods[i], instance, container);
			}
		}
        
		private static void Inject(MethodInfo method, object instance, Container container)
		{
			var parameters = method.GetParameters();
			var arguments = ExactArrayPool<object>.Shared.Rent(parameters.Length);

			for (int i = 0; i < parameters.Length; i++)
			{
				arguments[i] = container.Resolve(parameters[i].ParameterType);
			}

			try
			{
				method.Invoke(instance, arguments);
			}
			catch (Exception e)
			{
				throw new MethodInjectorException(instance, method, e);
			}
			finally
			{
				ExactArrayPool<object>.Shared.Return(arguments);
			}
		}
	}
}