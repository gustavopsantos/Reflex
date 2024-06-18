using System.Collections;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;

public class Test : MonoBehaviour
{
    private ExampleFactory3 _factory;

    [Inject]
    private void Construct(ExampleFactory3 factory)
    {
        _factory = factory;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var enemy = _factory.Create();
    }
}
