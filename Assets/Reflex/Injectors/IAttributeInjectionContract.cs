using Reflex.Core;

namespace Reflex.Injectors
{
    public interface IAttributeInjectionContract
    {
        /// <summary>
        /// Automatically invoked by Reflex for dependency injection
        /// </summary>
        void ReflexInject(Container container);
    }
}