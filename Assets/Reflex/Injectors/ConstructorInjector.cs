using System;
using Reflex.Buffers;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    public static class ConstructorInjector
    {
        public static object Construct(Type concrete, IServiceProvider serviceProvider)
        {
			TypeConstructionInfo info = TypeConstructionInfoCache.Get(concrete);
			object[] arguments = ExactArrayPool<object>.Shared.Rent(info.ConstructorParameters.Length);

            for (int i = 0; i < info.ConstructorParameters.Length; i++)
            {
                arguments[i] = serviceProvider.GetService(info.ConstructorParameters[i]);
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
    }
}