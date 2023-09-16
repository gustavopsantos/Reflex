using System;
using System.Reflection;
using Reflex.Microsoft.Delegates;

namespace Reflex.Microsoft.Reflectors
{
	public interface IActivatorFactory
	{
		ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);

		ObjectActivator GenerateDefaultActivator(Type type);
	}
}