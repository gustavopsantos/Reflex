using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Reflex.Injectors
{
	internal static class MethodInjector
	{
		internal static void Inject(MethodInfo method, object instance, Container container)
		{
			try
			{
				var parameters = method.GetParameters();
				var arguments = parameters.Select(p => container.Resolve(p.ParameterType)).ToArray();
				method.Invoke(instance, arguments);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		internal static void InjectMany(IEnumerable<MethodInfo> methods, object instance, Container container)
		{
			foreach (var method in methods)
			{
				Inject(method, instance, container);
			}
		}
	}
}