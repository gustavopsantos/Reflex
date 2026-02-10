using System;
using System.Linq;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Exceptions;
using Reflex.Pooling;

namespace Reflex.Injectors
{
    internal static class MethodInjector
    {
        [ThreadStatic]
        private static SizeSpecificArrayPool<object> _arrayPool;
        private static SizeSpecificArrayPool<object> ArrayPool => _arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);
        
        internal static void Inject(InjectableMethodInfo method, object instance, Container container)
        {
            var methodParameters = method.Parameters;
            var methodParametersLength = methodParameters.Length;
            var arguments = ArrayPool.Rent(methodParametersLength);

            try
            {
                for (var i = 0; i < methodParametersLength; i++)
                {
                    try
                    {
                        arguments[i] = container.Resolve(methodParameters[i].ParameterType);                    
                    }
                    catch (UnknownContractException exception)
                    {
                        if (methodParameters[i].HasDefaultValue)
                        {
                            arguments[i] = methodParameters[i].DefaultValue;
                        }
                        else
                        {
                            throw exception;
                        }
                    }
                }

                method.MethodInfo.Invoke(instance, arguments);
            }
            catch (Exception e)
            {
                throw new MethodInjectorException(instance, method.MethodInfo, e);
            }
            finally
            {
                ArrayPool.Return(arguments);
            }
        }
    }
}