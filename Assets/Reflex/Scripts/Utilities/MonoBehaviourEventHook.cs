using System;
using UnityEngine;

namespace Reflex.Scripts.Utilities
{
    public class MonoBehaviourEventHook : MonoBehaviour
    {
        public event Action OnDestroyEvent;

        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }
    }
}