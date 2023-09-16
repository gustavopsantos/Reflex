using Reflex.Microsoft.Attributes;
using Reflex.Microsoft.Sample.Application;
using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
{
    internal class Collector : MonoBehaviour
    {
        [Inject] private readonly ICollectorInput _input;
        [Inject] private readonly CollectorConfigurationModel _model;

        private void Update()
        {
			Vector2 input = _input.Get();
			Vector3 motion = Vector3.ClampMagnitude(new Vector3(input.x, 0, input.y), 1);
            transform.Translate(motion * (Time.deltaTime * _model.MovementSpeed));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Collectable collectable))
            {
                collectable.Collect();
            }
        }
    }
}