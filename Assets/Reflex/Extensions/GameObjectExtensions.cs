using System;
using System.Collections.Generic;
using Reflex.Enums;
using UnityEngine;

namespace Reflex.Extensions
{
    internal static class GameObjectExtensions
    {
        internal static void GetInjectables(this GameObject gameObject, MonoInjectionMode injectionMode, List<MonoBehaviour> result)
        {
            switch (injectionMode)
            {
                case MonoInjectionMode.Single:
                    result.Add(gameObject.GetComponent<MonoBehaviour>());
                    break;
                case MonoInjectionMode.Object:
                    gameObject.GetComponents<MonoBehaviour>(result);
                    break;
                case MonoInjectionMode.Recursive:
                    gameObject.GetComponentsInChildren<MonoBehaviour>(true, result);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(injectionMode), injectionMode, null);
            }
        }
    }
}