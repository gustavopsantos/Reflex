using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.PlayModeTests;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reflex.EditModeTests
{
    public class CreateProjectContainerTests
    {
        [Test]
        public void Create_ShouldBeAbleToCreate_WithoutAnyProjectScope()
        {
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            projectScopes.Length.Should().Be(0);
            var projectContainer = CreateProjectContainer.Create();
            projectContainer.Should().NotBeNull();
        }
        
        [Test]
        public void Create_ShouldBeAbleToCreate_WithOnlyOneProjectScope()
        {
            using var disposable = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            projectScopes.Length.Should().Be(1);
            var projectContainer = CreateProjectContainer.Create();
            projectContainer.Should().NotBeNull();
            var strings = projectContainer.All<string>().ToList();
            strings.Count(s => s == "A").Should().Be(1);
        }
        
        [Test]
        public void Create_ShouldBeAbleToCreate_WithMultipleProjectScopes_1()
        {
            using var disposable1 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            using var disposable2 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerB>());
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            projectScopes.Length.Should().Be(2);
            var projectContainer = CreateProjectContainer.Create();
            projectContainer.Should().NotBeNull();
            var strings = projectContainer.All<string>().ToList();
            strings.Count(s => s == "A").Should().Be(1);
            strings.Count(s => s == "B").Should().Be(1);
        }
        
        [Test]
        public void Create_ShouldBeAbleToCreate_WithMultipleProjectScopes_2()
        {
            using var disposable1 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            using var disposable2 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerB>());
            using var disposable3 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerC>());
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            projectScopes.Length.Should().Be(3);
            var projectContainer = CreateProjectContainer.Create();
            projectContainer.Should().NotBeNull();
            var strings = projectContainer.All<string>().ToList();
            strings.Count(s => s == "A").Should().Be(1);
            strings.Count(s => s == "B").Should().Be(1);
            strings.Count(s => s == "C").Should().Be(1);
        }
        
        [Test]
        public void Create_ShouldBeAbleToCreate_WithMultipleProjectScopes_3()
        {
            using var disposable1 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            using var disposable2 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            using var disposable3 = CreateProjectScopePrefabWithInstaller(p => p.AddComponent<MockedInstallerA>());
            var projectScopes = Resources.LoadAll<ProjectScope>(string.Empty);
            projectScopes.Length.Should().Be(3);
            var projectContainer = CreateProjectContainer.Create();
            projectContainer.Should().NotBeNull();
            var strings = projectContainer.All<string>().ToList();
            strings.Count(s => s == "A").Should().Be(3);
        }
        
        private static IDisposable CreateProjectScopePrefabWithInstaller(Action<GameObject> extend)
        {
            var gameObject = new GameObject("ProjectScope");
            gameObject.AddComponent<ProjectScope>();
            extend.Invoke(gameObject);
            
            var prefabPath = $"Assets/Reflex/Resources/ProjectScope-{Guid.NewGuid():N}.prefab";
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath, out var success);
            Assert.IsTrue(success);
            Object.DestroyImmediate(gameObject);
            
            return Disposable.Create(() => AssetDatabase.DeleteAsset(prefabPath));
        }
    }
}