using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace Project.Code.DI
{
    public class ReflexFactory<T> : MonoBehaviour, IReflexFactory<T>
    {
        private Container _container;
        
        [Inject]
        private void Construct(Container container)
        {
            _container = container;
        }
        
        public T Create()
        {
            return _container.Resolve<T>();
        }
    }
}