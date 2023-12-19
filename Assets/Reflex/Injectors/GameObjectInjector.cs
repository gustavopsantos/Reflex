using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Injectors
{
    public static class GameObjectInjector
    {
        public static void InjectSingle(GameObject gameObject, Container container)
        {
            if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                AttributeInjector.Inject(monoBehaviour, container);
            }
        }

        public static void InjectObject(GameObject gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponents<MonoBehaviour>(monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    AttributeInjector.Inject(monoBehaviour, container);
                }
            }
        }

        public static void InjectRecursive(GameObject gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponentsInChildren<MonoBehaviour>(true, monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    AttributeInjector.Inject(monoBehaviour, container);
                }
            }
        }

        public static void InjectRecursiveMany(List<GameObject> gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);

            for (int i = 0; i < gameObject.Count; i++)
            {
                gameObject[i].GetComponentsInChildren<MonoBehaviour>(true, monoBehaviours);

                for (int j = 0; j < monoBehaviours.Count; j++)
                {
                    var monoBehaviour = monoBehaviours[j];

                    if (monoBehaviour != null)
                    {
                        AttributeInjector.Inject(monoBehaviour, container);
                    }
                }
            }
        }
    }
}