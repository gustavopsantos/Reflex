using System;
using Reflex.Caching;

namespace Reflex.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(object obj, IServiceProvider serviceProvider)
        {
			TypeAttributeInfo info = TypeAttributeInfoCache.Get(obj.GetType());
            FieldInjector.InjectMany(info.InjectableFields, obj, serviceProvider);
            PropertyInjector.InjectMany(info.InjectableProperties, obj, serviceProvider);
            MethodInjector.InjectMany(info.InjectableMethods, obj, serviceProvider);
        }
    }
}