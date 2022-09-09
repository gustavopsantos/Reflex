using System;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Scripts.Utilities
{
    internal class MonoInstantiate
    {
        private static Transform _hiddenParent;

        private static Transform HiddenParent
        {
            get
            {
                if (_hiddenParent == null)
                {
                    var gameObject = new GameObject("[Reflex] Hidden Parent");
                    gameObject.SetActive(false);
                    _hiddenParent = gameObject.transform;
                }

                return _hiddenParent;
            }
        }

        internal static void InjectMonoBehaviour<T>(T instance, IContainer container) where T : Component
        {
            instance.GetComponentsInChildren<MonoBehaviour>().ForEach(mb => MonoInjector.Inject(mb, container));
        }

        internal static T Instantiate<T>(T original, Transform root, IContainer container, Func<Transform, T> instantiate) where T : Component
        {
            var parent = root;
            var prefabWasActive = original.gameObject.activeSelf;

            if (prefabWasActive)
                parent = HiddenParent;

            var instance = instantiate.Invoke(parent);

            if (prefabWasActive)
                instance.gameObject.SetActive(false);

            if (instance.transform.parent != root)
                instance.transform.SetParent(root, false);

            InjectMonoBehaviour(instance, container);

            instance.gameObject.SetActive(prefabWasActive);

            return instance;
        }
    }
}