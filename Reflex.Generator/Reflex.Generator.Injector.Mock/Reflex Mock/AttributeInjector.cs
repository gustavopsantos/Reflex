using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;

namespace Reflex.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(IAttributeInjectionContract obj, Container container)
        {
            obj.InjectFields(container);
            obj.InjectProperties(container);
            obj.InjectMethods(container);
        }
    }
}