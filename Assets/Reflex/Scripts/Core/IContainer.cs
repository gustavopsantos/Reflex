using System;
using UnityEngine;

namespace Reflex.Scripts
{
    public interface IContainer : IDisposable
    {
        public void AddDisposable(IDisposable disposable);
        public MonoBehaviour InjectMono(MonoBehaviour instance, bool recursive = false);
        public T Instantiate<T>(T original, Transform container = null) where T : Component;
        public T Instantiate<T>(T original, Transform container, bool worldPositionStays) where T : Component;
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform container = null) where T : Component;
        public GameObject Instantiate(GameObject original);
        public T Construct<T>();
        public object Construct(Type concrete);
        public BindingContractDefinition<TContract> Bind<TContract>();
        public void BindSingleton<TContract>(TContract instance);
        public BindingGenericContractDefinition BindGenericContract(Type genericContract);
        public TContract Resolve<TContract>();
        public object Resolve(Type contract);
        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete);
    }
}