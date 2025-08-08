using Reflex.Caching;
using Reflex.Core;

namespace Reflex.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(object obj, Container container)
        {
            if (obj is IAttributeInjectionContract contract)
            {
                Inject(contract, container);
                return;
            }

            var info = TypeInfoCache.Get(obj.GetType());

            var fields = info.InjectableFields;
            var fieldCount = fields.Length;
            for (var i = 0; i < fieldCount; i++)
            {
                FieldInjector.Inject(fields[i], obj, container);
            }

            var properties = info.InjectableProperties;
            var propertyCount = properties.Length;
            for (var i = 0; i < propertyCount; i++)
            {
                PropertyInjector.Inject(properties[i], obj, container);
            }

            var methods = info.InjectableMethods;
            var methodCount = methods.Length;
            for (var i = 0; i < methodCount; i++)
            {
                MethodInjector.Inject(methods[i], obj, container);
            }
        }
        public static void Inject(IAttributeInjectionContract contract, Container container)
        {
            FieldInjector.Inject(contract, container);
            PropertyInjector.Inject(contract, container);
            MethodInjector.Inject(contract, container);
        }
    }
}