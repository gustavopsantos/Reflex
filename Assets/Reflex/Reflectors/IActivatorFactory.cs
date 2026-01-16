using System;
using System.Reflection;
using Reflex.Caching;
using Reflex.Delegates;

namespace Reflex.Reflectors
{
    internal interface IActivatorFactory
    {
        ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, MemberParamInfo[] parameters);
        ObjectActivator GenerateDefaultActivator(Type type);
    }
}