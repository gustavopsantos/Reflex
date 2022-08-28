using System;
using System.Collections.Generic;
using System.Reflection;
using Reflex.Scripts;
using UnityEngine;

namespace Reflex.Injectors
{
	internal static class PropertyInjector
	{
		internal static void Inject(PropertyInfo property, object instance, IContainer container)
		{
			try
			{
				property.SetValue(instance, container.Resolve(property.PropertyType));
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		internal static void InjectMany(IEnumerable<PropertyInfo> properties, object instance, IContainer container)
		{
			foreach (var property in properties)
			{
				Inject(property, instance, container);
			}
		}
	}
}