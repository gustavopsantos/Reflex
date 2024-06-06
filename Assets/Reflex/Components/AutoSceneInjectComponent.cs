using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Components
{
    [DefaultExecutionOrder(int.MinValue + 1)]
    public sealed class AutoSceneInjectComponent : MonoBehaviour
    {
        [SerializeField] private InjectType _injectType = InjectType.Object;
        
        private void Awake()
        {
            var sceneContainer = gameObject.scene.GetSceneContainer();

            switch (_injectType)
            {
                case InjectType.Single:
                    GameObjectInjector.InjectSingle(gameObject, sceneContainer);
                    break;
                case InjectType.Object:
                    GameObjectInjector.InjectObject(gameObject, sceneContainer);
                    break;
                case InjectType.Recursive:
                    GameObjectInjector.InjectRecursive(gameObject, sceneContainer);
                    break;
            }
       }

        private enum InjectType
        {
            Single,
            Object,
            Recursive
        }
    }

    
}