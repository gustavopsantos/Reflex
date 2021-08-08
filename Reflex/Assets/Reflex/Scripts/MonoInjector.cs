using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Reflex.Scripts.Attributes;
using UnityEngine.SceneManagement;

namespace Reflex.Scripts
{
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

        private static void InjectSceneDependencies(Scene scene, IContainer container)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var monoBehaviour in rootGameObject.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    var type = monoBehaviour.GetType();

                    const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                    foreach (var field in type.GetFields(flags).Where(f => f.IsDefined(typeof(MonoInjectAttribute))))
                    {
                        InjectField(field, monoBehaviour, container);
                    }

                    foreach (var property in type.GetProperties(flags).Where(p => p.CanWrite && p.IsDefined(typeof(MonoInjectAttribute))))
                    {
                        InjectProperty(property, monoBehaviour, container);
                    }

                    foreach (var method in type.GetMethods(flags).Where(m => m.IsDefined(typeof(MonoInjectAttribute))))
                    {
                        InjectMethod(method, monoBehaviour, container);
                    }
                }
            }
        }

        private static bool TryGetProjectContext(out ProjectContext projectContext)
        {
            projectContext = Resources.Load<ProjectContext>("ProjectContext");
            if (projectContext == null) Debug.LogWarning("Skipping MonoInjector. A project context prefab named 'ProjectContext' should exist inside a Resources folder.");
            return projectContext != null;
        }

        private static void InjectField(FieldInfo fieldInfo, object instance, IContainer container)
        {
            try
            {
                fieldInfo.SetValue(instance, container.Resolve(fieldInfo.FieldType));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void InjectProperty(PropertyInfo propertyInfo, object instance, IContainer container)
        {
            try
            {
                propertyInfo.SetValue(instance, container.Resolve(propertyInfo.PropertyType));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void InjectMethod(MethodInfo methodInfo, object instance, IContainer container)
        {
            try
            {
                var parameters = methodInfo.GetParameters();
                var arguments = parameters.Select(p => container.Resolve(p.ParameterType)).ToArray();
                methodInfo.Invoke(instance, arguments);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}