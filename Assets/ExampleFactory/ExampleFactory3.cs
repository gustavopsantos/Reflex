using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

namespace Project.Code.DI
{
    public static class ReflexExtensions
    {
        public static void AddFactory<T, TFactory>(this ContainerBuilder builder)
            where TFactory : MonoBehaviour, IReflexFactory<T> 
            where T : MonoBehaviour
        {
            builder.AddSingleton(container =>
            {
                Debug.Log("generating factory");
                // TODO: add game object name option
                var factory = new GameObject("Inject Example Factory");
                var component = factory.AddComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory, container);
        
                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                // TODO: add game object name option
                var obj = new GameObject("Inject Example");
                var component = obj.AddComponent<T>();
                GameObjectInjector.InjectRecursive(obj, container);
            
                return component;
            });
        }
        
        public static void AddFactory<T, TFactory, TFacInterface>(this ContainerBuilder builder)
            where TFactory : MonoBehaviour, TFacInterface
            // TFacInterface let's users inject factory as interface
            // thereby easily swapping out factory implementations
            // by using interface in injected class
            where TFacInterface : IReflexFactory<T>
            where T : MonoBehaviour
        {
            builder.AddSingleton<TFacInterface>(container =>
            {
                Debug.Log("generating factory");
                // TODO: add game object name option
                var factory = new GameObject("Inject Example Factory");
                var component = factory.AddComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory, container);

                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                // TODO: add game object name option
                var obj = new GameObject("Inject Example");
                var component = obj.AddComponent<T>();
                GameObjectInjector.InjectRecursive(obj, container);
            
                return component;
            });
        }
        
        public static void AddFactory<T, TFactory>(this ContainerBuilder builder, T createdPrefab)
            where TFactory : MonoBehaviour, IReflexFactory<T> 
            where T : MonoBehaviour
        {
            builder.AddSingleton(container =>
            {
                Debug.Log("generating factory");
                // TODO: add game object name option
                var factory = new GameObject("Inject Example Factory");
                var component = factory.AddComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory, container);

                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                var obj = Object.Instantiate(createdPrefab);
                var component = obj.GetComponent<T>();
                GameObjectInjector.InjectRecursive(obj.gameObject, container);
            
                return component;
            });
        }
        
        public static void AddFactory<T, TFactory, TFacInterface>(this ContainerBuilder builder, T createdPrefab)
            where TFactory : MonoBehaviour, TFacInterface
            where TFacInterface : IReflexFactory<T>
            where T : MonoBehaviour
        {
            builder.AddSingleton<TFacInterface>(container =>
            {
                Debug.Log("generating factory");
                // TODO: add game object name option
                var factory = new GameObject("Inject Example Factory");
                var component = factory.AddComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory, container);

                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                var obj = Object.Instantiate(createdPrefab);
                var component = obj.GetComponent<T>();
                GameObjectInjector.InjectRecursive(obj.gameObject, container);
            
                return component;
            });
        }

        public static void AddFactoryFromPrefab<T, TFactory>(this ContainerBuilder builder, TFactory factoryPrefab, T createdPrefab)
            where TFactory : MonoBehaviour, IReflexFactory<T> 
            where T : MonoBehaviour
        {
            builder.AddSingleton(container =>
            {
                Debug.Log("generating factory");
                var factory = Object.Instantiate(factoryPrefab);
                var component = factory.GetComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory.gameObject, container);

                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                var obj = Object.Instantiate(createdPrefab);
                var component = obj.GetComponent<T>();
                GameObjectInjector.InjectRecursive(obj.gameObject, container);

                return component;
            });
        }
        
        public static void AddFactoryFromPrefab<T, TFacInterface, TFactory>(this ContainerBuilder builder, TFactory factoryPrefab, T createdPrefab)
            where TFactory : MonoBehaviour, TFacInterface 
            where TFacInterface : IReflexFactory<T>
            where T : MonoBehaviour
        {
            builder.AddSingleton<TFacInterface>(container =>
            {
                Debug.Log("generating factory");
                var factory = Object.Instantiate(factoryPrefab);
                var component = factory.GetComponent<TFactory>();
                GameObjectInjector.InjectRecursive(factory.gameObject, container);

                return component;
            });
            
            builder.AddTransient(container =>
            {
                Debug.Log("generating inject example");
                var obj = Object.Instantiate(createdPrefab);
                var component = obj.GetComponent<T>();
                GameObjectInjector.InjectRecursive(obj.gameObject, container);

                return component;
            });
        }
    }
}