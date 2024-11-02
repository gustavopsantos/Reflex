using System;
using System.Collections.Generic;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Components
{
    [DefaultExecutionOrder(int.MinValue + 1000)]
    internal sealed class GameObjectSelfInjector : MonoBehaviour
    {
        [SerializeField] private InjectionStrategy _injectionStrategy = InjectionStrategy.Recursive;

        private void Awake()
        {
            var sceneContainer = gameObject.scene.GetSceneContainer();
            var injectionActions = new Dictionary<InjectionStrategy, Action>
            {
                { InjectionStrategy.Single, () => GameObjectInjector.InjectSingle(gameObject, sceneContainer) },
                { InjectionStrategy.Object, () => GameObjectInjector.InjectObject(gameObject, sceneContainer) },
                { InjectionStrategy.Recursive, () => GameObjectInjector.InjectRecursive(gameObject, sceneContainer) }
            };
            
            injectionActions[_injectionStrategy]();
        }

        private enum InjectionStrategy
        {
            Single,
            Object,
            Recursive
        }
    }
}