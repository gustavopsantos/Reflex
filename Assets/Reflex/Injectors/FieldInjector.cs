using System;
using System.Reflection;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class FieldInjector
    {
        public static void InjectMany(FieldInfo[] fields, object instance, IServiceProvider serviceProvider)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Inject(fields[i], instance, serviceProvider);
            }
        }
        
        private static void Inject(FieldInfo field, object instance, IServiceProvider serviceProvider)
        {
            try
            {
                field.SetValue(instance, serviceProvider.GetService(field.FieldType));
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(e);
            }
        }
    }
}