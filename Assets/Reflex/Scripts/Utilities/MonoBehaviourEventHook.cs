using Reflex.Scripts.Events;
using UnityEngine;

namespace Reflex.Scripts.Utilities
{
    public class MonoBehaviourEventHook : MonoBehaviour
    {
        public DisposableEvent OnDestroyEvent = new DisposableEvent();

        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }
    }
}