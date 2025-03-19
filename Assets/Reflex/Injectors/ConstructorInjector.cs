using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Exceptions;
using Reflex.Pooling;

namespace Reflex.Injectors
{
    public static class ConstructorInjector
    {
        [ThreadStatic]
        private static SizeSpecificArrayPool<object> _arrayPool;
        internal static SizeSpecificArrayPool<object> ArrayPool => _arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);
        
        public static object Construct(Type concrete, Container container)
        {
            var info = TypeConstructionInfoCache.Get(concrete);
            var constructorParameters = info.ConstructorParameters;
            var constructorParametersLength = info.ConstructorParameters.Length;
            var arguments = ArrayPool.Rent(constructorParametersLength);

            try
            {
                for (var i = 0; i < constructorParametersLength; i++)
                {
                    arguments[i] = container.Resolve(constructorParameters[i]);
                }

                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception exception)
            {
                throw new ConstructorInjectorException(concrete, exception, constructorParameters);
            }
            finally
            {
                ArrayPool.Return(arguments);
            }
        }
        
        public static object Construct(Type concrete, object[] arguments)
        {
            var info = TypeConstructionInfoCache.Get(concrete);

            try
            {
                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception exception)
            {
                throw new ConstructorInjectorException(concrete, exception, info.ConstructorParameters);
            }
        }
    }
}