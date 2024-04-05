using System.Collections;
using FluentAssertions;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Reflex.PlayModeTests
{
    public class ScenePreInstallerTests
    {
        [UnityTest]
        public IEnumerator PreInstall_ShouldWorkWith_SceneManagerLoadSceneAsync()
        {
            var service = new object();
            var loadingOperation = SceneManager.LoadSceneAsync("ExecutionOrderTestsScene", LoadSceneMode.Single);
            var sceneBeingLoaded = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            ReflexSceneManager.PreInstallScene(sceneBeingLoaded, builder => builder.AddSingleton(service));
            yield return loadingOperation;
            var sceneContainer = sceneBeingLoaded.GetSceneContainer();
            sceneContainer.Single<object>().Should().Be(service);
        }

        [UnityTest]
        public IEnumerator PreInstall_ShouldWorkWith_SceneManagerLoadScene()
        {
            var service = new object();
            var loadSceneParams = new LoadSceneParameters(LoadSceneMode.Single);
            var sceneBeingLoaded = SceneManager.LoadScene("ExecutionOrderTestsScene", loadSceneParams);
            ReflexSceneManager.PreInstallScene(sceneBeingLoaded, builder => builder.AddSingleton(service));
            yield return new WaitUntil(() => sceneBeingLoaded.isLoaded);
            var sceneContainer = sceneBeingLoaded.GetSceneContainer();
            sceneContainer.Single<object>().Should().Be(service);
        }
    }
}