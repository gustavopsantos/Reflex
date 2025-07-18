using System.Reflection;

namespace Reflex.Caching
{
    internal sealed class TypeAttributeInfo
    {
        public readonly FieldInfo[] InjectableFields;
        public readonly PropertyInfo[] InjectableProperties;
        public readonly InjectableMethodInfo[] InjectableMethods;

        public TypeAttributeInfo(FieldInfo[] injectableFields, PropertyInfo[] injectableProperties, InjectableMethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods;
        }
    }
}