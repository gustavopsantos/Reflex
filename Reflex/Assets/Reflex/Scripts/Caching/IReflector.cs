using System;
using System.Reflection;

namespace Reflex
{
	internal interface IReflector
	{
		DynamicObjectActivator GenerateDefaultActivator(Type type);
		DynamicObjectActivator GenerateGenericActivator(Type type, ConstructorInfo constructor, Type[] parameters);
	}
}