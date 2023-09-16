using System;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Microsoft.Injectors
{
    internal static class SceneInjector
    {
        public static void Inject(Scene scene, IServiceProvider serviceProvider)
        {
            foreach (MonoBehaviour monoBehavior in scene.All<MonoBehaviour>())
            {
                AttributeInjector.Inject(monoBehavior, serviceProvider);
            }
        }
    }
}