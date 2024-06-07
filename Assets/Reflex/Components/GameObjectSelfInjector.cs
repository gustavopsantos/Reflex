using System;
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

            switch (_injectionStrategy)
            {
                case InjectionStrategy.Single:
                    GameObjectInjector.InjectSingle(gameObject, sceneContainer);
                    break;
                case InjectionStrategy.Object:
                    GameObjectInjector.InjectObject(gameObject, sceneContainer);
                    break;
                case InjectionStrategy.Recursive:
                    GameObjectInjector.InjectRecursive(gameObject, sceneContainer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_injectionStrategy.ToString());
            }
        }

        private enum InjectionStrategy
        {
            Single,
            Object,
            Recursive
        }
    }
}