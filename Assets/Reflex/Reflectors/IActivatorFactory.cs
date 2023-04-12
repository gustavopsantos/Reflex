using System;
using System.Reflection;
using Reflex.Delegates;

namespace Reflex.Reflectors
{
    internal interface IActivatorFactory
    {
        ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);
        ObjectActivator GenerateDefaultActivator(Type type);
    }
}