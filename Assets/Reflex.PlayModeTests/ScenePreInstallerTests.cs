using System.Collections;
using FluentAssertions;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Reflex.PlayModeTests
{
    public class ScenePreInstallerTests
    {
        private static void ExtraInstaller(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(42, Lifetime.Singleton);
        }
        
        [UnityTest]
        public IEnumerator PreInstall_ShouldWorkWith_SceneManagerLoadSceneAsync()
        {
            using (new ExtraInstallerScope(ExtraInstaller))
            {
                yield return SceneManager.LoadSceneAsync("ExecutionOrderTestsScene");
            }
            
            var activeScene = SceneManager.GetActiveScene();
            var sceneContainer = activeScene.GetSceneContainer();
            sceneContainer.Single<int>().Should().Be(42);
        }

        [UnityTest]
        public IEnumerator PreInstall_ShouldWorkWith_SceneManagerLoadScene()
        {
            using (new ExtraInstallerScope(ExtraInstaller))
            {
                var loadSceneParams = new LoadSceneParameters(LoadSceneMode.Single);
                var loadScene = SceneManager.LoadScene("ExecutionOrderTestsScene", loadSceneParams);
                yield return new WaitUntil(() => loadScene.isLoaded);
            }
            
            var activeScene = SceneManager.GetActiveScene();
            var sceneContainer = activeScene.GetSceneContainer();
            sceneContainer.Single<int>().Should().Be(42);
        }
    }
}