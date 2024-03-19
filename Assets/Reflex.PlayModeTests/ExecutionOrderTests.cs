using System.Collections;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Reflex.PlayModeTests
{
    public class ExecutionOrderTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return SceneManager.LoadSceneAsync("ExecutionOrderTestsScene", LoadSceneMode.Single);
            yield return WaitFrame(); // Wait until Start is called, takes one frame
        }
        
        [Test]
        public void ExecutionOrderOf_PreInstantiated_InjectedObject_ShouldBe_AwakeInjectStart()
        {
            var injectedObject = Object.FindObjectsOfType<InjectedGameObject>().Single();
            string.Join(",", injectedObject.ExecutionOrder).Should().Be("Awake,Inject,Start");
        }
        
        [UnityTest]
        public IEnumerator ExecutionOrderOf_RuntimeInstantiated_InjectedObject_ShouldBe_AwakeInjectStart()
        {
            var prefab = new GameObject("Prefab").AddComponent<InjectedGameObject>();
            var injectedObject = Object.Instantiate(prefab);
            GameObjectInjector.InjectRecursive(injectedObject.gameObject, injectedObject.gameObject.scene.GetSceneContainer());
            yield return WaitFrame(); // Wait until Start is called, takes one frame
            string.Join(",", injectedObject.ExecutionOrder).Should().Be("Awake,Inject,Start");
        }
        
        /// <summary>
        /// yield return new WaitForEndOfFrame() does not work when running tests on cli, it hangs
        /// See https://docs.unity3d.com/2022.3/Documentation/Manual/CLIBatchmodeCoroutines.html
        /// </summary>
        /// <returns></returns>
        private static IEnumerator WaitFrame()
        {
            var current = Time.frameCount;
 
            while (current == Time.frameCount)
            {
                yield return null;
            }
        }
    }
}