using System.Collections;
using System.Collections.Generic;
using Project.Code.DI;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

public class InjectExample : MonoBehaviour
{
    [Inject] private readonly IEnumerable<string> _strings;

    private void Start()
    {
        Debug.Log(string.Join(" ", _strings));
    }
    
    public class InjectExampleFactory : MonoBehaviour, IReflexFactory<InjectExample>
    {
        private Container _container;
        
        [Inject]
        private void Construct(Container container)
        {
            _container = container;
        }
        
        public InjectExample Create()
        {
            return _container.Resolve<InjectExample>();
        }
    }
    
    public class InjectExampleFactory2 : ReflexFactory<InjectExample>
    {
    }
}

