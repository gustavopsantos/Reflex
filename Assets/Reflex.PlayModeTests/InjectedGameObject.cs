using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class InjectedGameObject : MonoBehaviour
    {
        public readonly List<string> ExecutionOrder = new List<string>();

        [Inject]
        private void Inject()
        {
            ExecutionOrder.Add(nameof(Inject));
        }

        private void Awake()
        {
            ExecutionOrder.Add(nameof(Awake));
        }

        private void Start()
        {
            ExecutionOrder.Add(nameof(Start));
        }
    }
}