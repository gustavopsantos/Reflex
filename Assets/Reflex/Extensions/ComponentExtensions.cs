using System;
using System.Collections.Generic;
using Reflex.Enums;
using UnityEngine;

namespace Reflex.Extensions
{
    internal static class ComponentExtensions
    {
        internal static void GetInjectables(this Component component, MonoInjectionMode injectionMode, List<MonoBehaviour> result)
        {
            switch (injectionMode)
            {
                case MonoInjectionMode.Single:
                    result.Add(component.GetComponent<MonoBehaviour>());
                    break;
                case MonoInjectionMode.Object:
                    component.GetComponents<MonoBehaviour>(result);
                    break;
                case MonoInjectionMode.Recursive:
                    component.GetComponentsInChildren<MonoBehaviour>(true, result);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(injectionMode), injectionMode, null);
            }
        }
    }
}