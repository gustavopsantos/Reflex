using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Reflex.Scripts.Utilities;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal static class TypeInfoCache
    {
        internal static readonly IntHashMap<TypeInfo> Registry = new IntHashMap<TypeInfo>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //Register type before resolving for smooth FPS, cause resolve can be called at any moment of the gameplay.
        internal static void Register<T>() {
            var hashCode = TypeHashCodeHelper<T>.Hash;
            
            if (Registry.Has(hashCode))
            {
                return;
            }

            var type = typeof(T);
            var constructor = FindMostValuableConstructor(type);
            var parameters  = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var activator   = CompileGenericActivator(constructor, parameters);

            //faster than call .cctor struct/class on IL2CPP
            TypeInfo info;
            info.Type = type;
            info.ConstructorParameters = parameters;
            info.Activator = activator;
            
            Registry.Add(hashCode, info, out _);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Register(Type type) 
        {
            var hashCode = type.GetHashCode();
            
            if (Registry.Has(hashCode))
            {
                return;
            }

            var constructor = FindMostValuableConstructor(type);
            var parameters  = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var activator   = CompileGenericActivator(constructor, parameters);

            //faster than call .cctor struct/class on IL2CPP
            TypeInfo info;
            info.Type = type;
            info.ConstructorParameters = parameters;
            info.Activator = activator;
            
            Registry.Add(hashCode, info, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DynamicObjectActivator CompileGenericActivator(ConstructorInfo constructor, Type[] parametersTypes)
        {
            var param = Expression.Parameter(typeof(object[]));
            var argumentsExpressions = new Expression[parametersTypes.Length];

            for (int i = 0; i < parametersTypes.Length; i++)
            {
                var index = Expression.Constant(i);
                var parameterType = parametersTypes[i];
                var parameterAccessor = Expression.ArrayIndex(param, index);
                var parameterCast = Expression.Convert(parameterAccessor, parameterType);
                argumentsExpressions[i] = parameterCast;
            }

            var newExpression = Expression.New(constructor, argumentsExpressions);
            var lambda = Expression.Lambda(typeof(DynamicObjectActivator), Expression.Convert(newExpression, typeof(object)), param);
            return (DynamicObjectActivator) lambda.Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ConstructorInfo FindMostValuableConstructor(Type concrete)
        {
            return concrete.GetConstructors().MaxBy(constructor => constructor.GetParameters().Length);
        }
    }
}