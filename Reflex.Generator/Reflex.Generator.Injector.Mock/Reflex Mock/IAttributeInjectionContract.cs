using Reflex.Core;

namespace Reflex.Injectors
{
    public interface IAttributeInjectionContract
    {
        void InjectFields(Container container);
        void InjectProperties(Container container);
        void InjectMethods(Container container);
    }
}