using System;
using Reflex.Enums;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Extensions
{
    public static class ContainerExtensions
    {
        public static T Instantiate<T>(
            this IServiceProvider serviceProvider,
            T original,
            MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) 
                where T : Component
        {
            T instance = UnityEngine.Object.Instantiate(original);

            foreach (MonoBehaviour injectable in instance.GetInjectables(injectionMode))
            {
                AttributeInjector.Inject(injectable, serviceProvider);
            }

            return instance;
        }
    }
}