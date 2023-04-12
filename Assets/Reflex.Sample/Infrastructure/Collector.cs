using Reflex.Attributes;
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    internal class Collector : MonoBehaviour
    {
        [Inject] private readonly ICollectorInput _input;
        [Inject] private readonly CollectorConfigurationModel _model;

        private void Update()
        {
            var input = _input.Get();
            var motion = Vector3.ClampMagnitude(new Vector3(input.x, 0, input.y), 1);
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