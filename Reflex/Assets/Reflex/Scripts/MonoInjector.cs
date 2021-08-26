using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Reflex.Scripts.Attributes;
using UnityEngine.SceneManagement;
using Unity.IL2CPP.CompilerServices;
using System.Runtime.CompilerServices;

namespace Reflex.Scripts
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal class MonoInjector : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded()
        {
            if (TryGetProjectContext(out var projectContext))
            {
                projectContext.InstallBindings();
                SceneManager.sceneLoaded += (scene, mode) => InjectSceneDependencies(scene, projectContext);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InjectSceneDependencies(Scene scene, IContainer container)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var monoBehaviour in rootGameObject.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    var type       = monoBehaviour.GetType();
                    var reflection = ReflectionsCache.Get(type);

                    reflection.Inject(monoBehaviour, container);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetProjectContext(out ProjectContext projectContext)
        {
            projectContext = Resources.Load<ProjectContext>("ProjectContext");
            if (projectContext == null) 
            {
                Debug.LogWarning("Skipping MonoInjector. A project context prefab named 'ProjectContext' should exist inside a Resources folder.");
            }
            return projectContext != null;
        }
        
        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        private struct Reflection 
        {
            internal FieldInfo[]    Fields;
            internal PropertyInfo[] Properties;
            internal MethodInfo[]   Methods;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Inject(object instance, IContainer container) 
            {
                foreach (var field in Fields) 
                {
                    try
                    {
                        field.SetValue(instance, container.Resolve(field.FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                
                foreach (var property in Properties) 
                {
                    try
                    {
                        property.SetValue(instance, container.Resolve(property.PropertyType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                
                foreach (var method in Methods) 
                {
                    try
                    {
                        var parameters = method.GetParameters();
                        var arguments  = parameters.Select(p => container.Resolve(p.ParameterType)).ToArray();
                        method.Invoke(instance, arguments);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        private static class ReflectionsCache 
        {
            private static readonly IntHashMap<Reflection> reflections;
            private static readonly Type attributeType;

            static ReflectionsCache() 
            {
                reflections = new IntHashMap<Reflection>();
                attributeType = typeof(MonoInjectAttribute);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static Reflection Get(Type type) 
            {
                var hash = type.GetHashCode();
                if (reflections.TryGetValue(hash, out var reflection))
                {
                    return reflection;
                }
                
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                reflection.Fields = type.GetFields(flags).Where(f => f.IsDefined(attributeType)).ToArray();
                reflection.Properties = type.GetProperties(flags).Where(p => p.CanWrite && p.IsDefined(attributeType)).ToArray();
                reflection.Methods = type.GetMethods(flags).Where(m => m.IsDefined(attributeType)).ToArray();

                reflections.Add(hash, reflection, out _);
                return reflection;
            }
        }
    }
}