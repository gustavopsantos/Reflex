using System.Reflection;

public struct MonoInjectableTypeInfo
{
	internal FieldInfo[] InjectableFields;
	internal PropertyInfo[] InjectableProperties;
	internal MethodInfo[] InjectableMethods;
}