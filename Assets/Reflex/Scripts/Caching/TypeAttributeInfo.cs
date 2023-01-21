using System.Reflection;

namespace Reflex.Scripts.Caching
{
    internal readonly struct TypeAttributeInfo
    {
        public readonly FieldInfo[] InjectableFields;
        public readonly PropertyInfo[] InjectableProperties;
        public readonly MethodInfo[] InjectableMethods;

        public TypeAttributeInfo(FieldInfo[] injectableFields, PropertyInfo[] injectableProperties, MethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods;
        }
    }
}