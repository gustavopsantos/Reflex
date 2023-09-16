using System;
using Reflex.Microsoft.Buffers;
using Reflex.Microsoft.Caching;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Exceptions;

namespace Reflex.Microsoft.Injectors
{
    internal static class MethodInjector
    {
        public static void InjectMany(InjectedMethodInfo[] methods, object instance, IServiceProvider serviceProvider)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                Inject(methods[i], instance, serviceProvider);
            }
        }
        
        private static void Inject(InjectedMethodInfo method, object instance, IServiceProvider serviceProvider)
        {
			object[] arguments = ExactArrayPool<object>.Shared.Rent(method.Parameters.Length);

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                arguments[i] = serviceProvider.GetService(method.Parameters[i].ParameterType);
            }

            try
            {
                method.MethodInfo.Invoke(instance, arguments);
            }
            catch (Exception e)
            {
                throw new MethodInjectorException(instance, method.MethodInfo, e);
            }
            finally
            {
               ExactArrayPool<object>.Shared.Return(arguments);
            }
        }
    }
}