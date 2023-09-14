using System;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
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