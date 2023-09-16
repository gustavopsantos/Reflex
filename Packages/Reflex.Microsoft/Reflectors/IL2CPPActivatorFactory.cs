using System;
using System.Reflection;
using Reflex.Microsoft.Delegates;

namespace Reflex.Microsoft.Reflectors
{
	public sealed class IL2CPPActivatorFactory : IActivatorFactory
	{
		public ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters)
		{
			return args =>
			{
				object instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
				constructor.Invoke(instance, args);
				return instance;
			};
		}

		public ObjectActivator GenerateDefaultActivator(Type type)
		{
			return args => System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
		}
	}
}