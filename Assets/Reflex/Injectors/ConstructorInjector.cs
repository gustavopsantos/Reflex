using System;
using Reflex.Buffers;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    public static class ConstructorInjector
    {
        public static object Construct(Type concrete, Container container)
        {
            var info = TypeConstructionInfoCache.Get(concrete);
            var arguments = ExactArrayPool<object>.Shared.Rent(info.ConstructorParameters.Length);

            for (var i = 0; i < info.ConstructorParameters.Length; i++)
            {
                arguments[i] = container.Resolve(info.ConstructorParameters[i]);
            }

            try
            {
                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception e)
            {
                throw new ConstructorInjectorException(concrete, e);
            }
            finally
            {
                ExactArrayPool<object>.Shared.Return(arguments);
            }
        }
        
        public static object Construct(Type concrete, object[] arguments)
        {
            var info = TypeConstructionInfoCache.Get(concrete);

            try
            {
                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception e)
            {
                throw new ConstructorInjectorException(concrete, e);
            }
        }
    }
}