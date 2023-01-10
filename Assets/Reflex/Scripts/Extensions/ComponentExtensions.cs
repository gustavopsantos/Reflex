using System;
using System.Collections.Generic;
using Reflex.Scripts.Enums;
using Reflex.Scripts.Utilities;
using UnityEngine;

namespace Reflex.Scripts.Extensions
{
    internal static class ComponentExtensions
    {
        internal static IEnumerable<MonoBehaviour> GetInjectables<T>(this T component, MonoInjectionMode injectionMode) where T : Component
        {
            switch (injectionMode)
            {
                case MonoInjectionMode.Single: return component.GetComponent<MonoBehaviour>().Yield();
                case MonoInjectionMode.Object: return component.GetComponents<MonoBehaviour>();
                case MonoInjectionMode.Recursive: return component.GetComponentsInChildren<MonoBehaviour>(true);
                default: throw new ArgumentOutOfRangeException(nameof(injectionMode), injectionMode, null);
            }
        }
    }
}