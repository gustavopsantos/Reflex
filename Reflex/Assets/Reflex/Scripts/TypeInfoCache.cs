using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Reflex.Scripts.Utilities;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
#if ENABLE_IL2CPP
using System.Runtime.Serialization;
#endif

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
        internal static void Register<T>()
        {
            var hashCode = TypeHashCodeHelper<T>.Hash;

            if (Registry.Has(hashCode))
            {
                return;
            }

            var type = typeof(T);
            RegisterInternal(type, hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Register(Type type)
        {
            var hashCode = type.GetHashCode();

            if (Registry.Has(hashCode))
            {
                return;
            }

            RegisterInternal(type, hashCode);
        }
        
        private static void RegisterInternal(Type type, int hashCode)
        {
            Type[] parameters;
            DynamicObjectActivator activator;
            TypeInfo info;
            
            var constructors = type.GetConstructors();
            
            //string workaround
            if (Type.GetTypeCode(type) == TypeCode.String)
            {
                parameters = Type.EmptyTypes;
                activator = objs => null;
            }
            else if (constructors.Length > 0)
            {
                var constructor = type.GetConstructors().MaxBy(c => c.GetParameters().Length);
                parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
                //Expressions for Mono Runtime, and FormatterServices for IL2CPP
#if ENABLE_IL2CPP
                activator = objs => 
                {
                    var instance = FormatterServices.GetUninitializedObject(type);
                    constructor.Invoke(instance, objs);
                    return instance;
                };
#else
                activator = CompileGenericActivator(constructor, parameters);
#endif
            }
            else
            {
                parameters = Type.EmptyTypes;
#if ENABLE_IL2CPP
                activator = objs => FormatterServices.GetUninitializedObject(type); //for value types return default(T)
#else
                activator = CompileDefaultActivator(type); //for value types return default(T)
#endif
            }

            //faster than call .cctor struct/class on IL2CPP
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
            return (DynamicObjectActivator)lambda.Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DynamicObjectActivator CompileDefaultActivator(Type type)
        {
            var e = Expression.Lambda(typeof(DynamicObjectActivator), Expression.Convert(Expression.Default(type), typeof(object)), Expression.Parameter(typeof(object[])));
            return (DynamicObjectActivator) e.Compile();
        }
    }
}
