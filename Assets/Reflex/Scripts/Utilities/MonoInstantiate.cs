using System;
using Reflex.Injectors;
using Reflex.Scripts.Enums;
using Reflex.Scripts.Extensions;
using UnityEngine;

namespace Reflex.Scripts.Utilities
{
    internal static class MonoInstantiate
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

        internal static T Instantiate<T>(T original, Transform parent, Container container, Func<Transform, T> instantiate, MonoInjectionMode injectionMode) where T : Component
        {
            var root = parent;
            var prefabWasActive = original.gameObject.activeSelf;

            if (prefabWasActive)
                root = HiddenParent;

            var instance = instantiate.Invoke(root);

            if (prefabWasActive)
                instance.gameObject.SetActive(false);

            if (instance.transform.parent != parent)
                instance.transform.SetParent(parent, false);

            instance.GetInjectables(injectionMode).ForEach(mb => MonoInjector.Inject(mb, container));

            instance.gameObject.RestoreRectTransform(original);
            instance.gameObject.SetActive(prefabWasActive);

            return instance;
        }

        internal static void RestoreRectTransform(this GameObject gameObject, Component original)
        {
            if (gameObject.TryGetComponent<RectTransform>(out var rectTransform) && original.TryGetComponent<RectTransform>(out var originalRectTransform))
            {
                rectTransform.offsetMax = originalRectTransform.offsetMax;
                rectTransform.offsetMin = originalRectTransform.offsetMin;
                rectTransform.anchorMax = originalRectTransform.anchorMax;
                rectTransform.anchorMin = originalRectTransform.anchorMin;
                rectTransform.localScale = originalRectTransform.localScale;
            }
        }
    }
}