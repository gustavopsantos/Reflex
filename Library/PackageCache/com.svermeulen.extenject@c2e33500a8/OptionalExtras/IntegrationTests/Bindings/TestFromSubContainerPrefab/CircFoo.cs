using UnityEngine;

namespace Zenject.Tests.Bindings.FromSubContainerPrefab
{
    public class CircFoo : MonoBehaviour
    {
        [Inject]
        public CircBar Bar;
    }
}
