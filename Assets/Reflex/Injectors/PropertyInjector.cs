using System;
using System.Reflection;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class PropertyInjector
    {
        public static void InjectMany(PropertyInfo[] properties, object instance, IServiceProvider serviceProvider)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                Inject(properties[i], instance, serviceProvider);
            }
        }
        
        private static void Inject(PropertyInfo property, object instance, IServiceProvider serviceProvider)
        {
            try
            {
                property.SetValue(instance, serviceProvider.GetService(property.PropertyType));
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(e);
            }
        }
    }
}