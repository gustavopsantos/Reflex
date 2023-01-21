using System;
using System.Reflection;
using Reflex.Scripts.Delegates;

namespace Reflex.Scripts.Reflectors
{
    internal interface IActivatorFactory
    {
        ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);
        ObjectActivator GenerateDefaultActivator(Type type);
    }
}