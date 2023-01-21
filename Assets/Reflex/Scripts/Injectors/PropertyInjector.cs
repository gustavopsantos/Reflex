using System;
using System.Reflection;

namespace Reflex.Injectors
{
	internal static class PropertyInjector
	{
		public static void InjectMany(PropertyInfo[] properties, object instance, Container container)
		{
			for (var i = 0; i < properties.Length; i++)
			{
				Inject(properties[i], instance, container);
			}
		}
        
		private static void Inject(PropertyInfo property, object instance, Container container)
		{
			try
			{
				property.SetValue(instance, container.Resolve(property.PropertyType));
			}
			catch (Exception e)
			{
				throw new PropertyInjectorException(e);
			}
		}
	}
}