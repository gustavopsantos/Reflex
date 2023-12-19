using System;
using System.Reflection;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class PropertyInjector
    {
        internal static void Inject(PropertyInfo property, object instance, Container container)
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