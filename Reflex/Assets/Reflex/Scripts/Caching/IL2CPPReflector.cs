using System;
using System.Reflection;

namespace Reflex
{
	internal class IL2CPPReflector : IReflector
	{
		DynamicObjectActivator IReflector.GenerateGenericActivator(Type type, ConstructorInfo constructor, Type[] parameters)
		{
			return args =>
			{
				var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
				constructor.Invoke(instance, args);
				return instance;
			};
		}

		DynamicObjectActivator IReflector.GenerateDefaultActivator(Type type)
		{
			return args => System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
		}
	}
}