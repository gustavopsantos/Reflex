using Reflex.Caching;
using Reflex.Core;

namespace Reflex.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(object obj, Container container)
        {
            var info = TypeAttributeInfoCache.Get(obj.GetType());
            FieldInjector.InjectMany(info.InjectableFields, obj, container);
            PropertyInjector.InjectMany(info.InjectableProperties, obj, container);
            MethodInjector.InjectMany(info.InjectableMethods, obj, container);
        }
    }
}