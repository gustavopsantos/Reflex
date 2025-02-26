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
        private static ThreadStaticArrayPool<object> _arrayPool;
        internal static ThreadStaticArrayPool<object> ArrayPool => _arrayPool ??= new ThreadStaticArrayPool<object>(initialSize: 16);
        
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