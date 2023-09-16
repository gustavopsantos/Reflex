using System;
using System.Reflection;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Exceptions;

namespace Reflex.Microsoft.Injectors
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