using System;
using Reflex.Microsoft.Buffers;
using Reflex.Microsoft.Caching;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Exceptions;

namespace Reflex.Microsoft.Injectors
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