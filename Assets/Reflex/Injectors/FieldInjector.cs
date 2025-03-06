using System;
using System.Reflection;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class FieldInjector
    {
        internal static void Inject(FieldInfo field, object instance, Container container)
        {
            try
            {
                field.SetValue(instance, container.Resolve(field.FieldType, field.GetCustomAttribute<InjectAttribute>()));
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(e);
            }
        }
    }
}