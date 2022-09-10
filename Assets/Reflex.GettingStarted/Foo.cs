using EasyButtons;
using Reflex.Scripts;
using Reflex.Scripts.Attributes;
using UnityEngine;

public class Foo : MonoBehaviour
{
    [Inject] private readonly IContainer _container;
    
    [Button]
    private void Solve()
    {
        Debug.Log(_container.Resolve<int>());
    }
}
