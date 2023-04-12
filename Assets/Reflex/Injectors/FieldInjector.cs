using System;
using System.Reflection;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class FieldInjector
    {
        public static void InjectMany(FieldInfo[] fields, object instance, Container container)
        {
            for (var i = 0; i < fields.Length; i++)
            {
                Inject(fields[i], instance, container);
            }
        }
        
        private static void Inject(FieldInfo field, object instance, Container container)
        {
            try
            {
                field.SetValue(instance, container.Resolve(field.FieldType));
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(e);
            }
        }
    }
}