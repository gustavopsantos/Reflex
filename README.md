<p align="center">
  <img src="logo-crimson.png" width="250">
</p>
<p align="center">
  <img src="text-crimson.png" width="300">
</p>

Reflex is an [Dependency Injection](https://stackify.com/dependency-injection/) framework for [Unity](https://unity.com/). Making your classes independent of its dependencies, granting better separation of concerns. It achieves that by decoupling the usage of an object from its creation. This helps you to follow SOLID’s dependency inversion and single responsibility principles. Making your project more **readable, testable and scalable.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)   
[![Unity](https://img.shields.io/badge/Unity-2019+-black.svg)](https://unity3d.com/pt/get-unity/download/archive)  
[![PullRequests](https://img.shields.io/badge/PRs-welcome-brightgreen)](http://makeapullrequest.com)  
[![Releases](https://img.shields.io/github/release/gustavopsantos/reflex.svg)](https://github.com/gustavopsantos/reflex/releases)  
![](https://github.com/gustavopsantos/reflex/workflows/Tests/badge.svg)  


## Features 
- Blazing fast
- IL2CPP Friendly
- Minimal code base
- Contructor injection
- `[MonoInject]` Property, field and method injection attribute

## Performance
> Resolving ten thousand times a transient dependency with four levels of chained dependencies. See [NestedBenchmarkReflex.cs](Reflex.Benchmark/Assets/Benchmark/NestedBenchmarkReflex.cs).

### Android

<table>
<tr><th>Mono</th><th>IL2CPP</th></tr>
<tr><td>

|           | GC    | Time |
|-----------|------:|-----:|
| Reflex    |  54KB | 10ms
| Zenject   | 464KB | 73ms
| VContainer| 128KB | 51ms

</td><td>

|           | GC    | Time |
|-----------|------:|-----:|
| Reflex    |  70KB | 15ms
| Zenject   | 480KB | 77ms
| VContainer| 128KB | 18ms

</td></tr> </table>

### Windows

<table>
<tr><th>Mono</th><th>IL2CPP</th></tr>
<tr><td>

|           | GC    | Time |
|-----------|------:|-----:|
| Reflex    | 109KB | 1ms
| Zenject   | 900KB | 7ms
| VContainer| 257KB | 3ms

</td><td>

|           | GC    | Time |
|-----------|------:|-----:|
| Reflex    | 140KB | 1ms
| Zenject   | 900KB | 7ms
| VContainer| 257KB | 2ms

</td></tr> </table>

> The performance on `IL2CPP (AOT)` backend is not so good because the expressions are actually interpreted, unlike `Mono (JIT)`, where they are actually compiled.

> I'm investigating whether dealing with IL Reweaving is worth the complexity it brings.

## Installation

*Requires Unity 2019+*

### Install via UPM (using Git URL)
```json
"com.gustavopsantos.reflex": "https://github.com/gustavopsantos/reflex.git?path=/Reflex/Assets/Reflex/#1.0.3"
```

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/gustavopsantos/reflex/releases) page.
2. Import Reflex.X.X.X.unitypackage

## Getting Started

### Installing Bindings

Create a MonoInstaller to install your bindings in the project context, and remember to add this component in the ProjectContext prefab, and reference it in the Mono Installers list of the ProjectContext. See [ProjectContext.prefab](Reflex.GettingStarted/Assets/GettingStarted/Resources/ProjectContext.prefab).

```csharp
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings(Container container)
    {
        container.BindSingleton<int>(42);
        container.Bind<IDependencyOne>().To<DependencyOne>().AsTransient();
        container.Bind<IDependencyTwo>().To<DependencyTwo>().AsSingletonLazy();
        container.BindGenericContract(typeof(SetupAsset<>)).To(
            typeof(SetupEnemy),
            typeof(SetupLevel)
        ).AsSingletonNonLazy();
    }
}
```

### MonoBehaviour Injection

> Be aware that fields and properties with [MonoInject] are only injected into pre-existing MonoBehaviours within the scene after the SceneManager.sceneLoaded event, which happens after Awake and before Start. See [MonoInjector.cs](Reflex/Assets/Reflex/Scripts/Injectors/MonoInjector.cs).

```csharp
public class MonoBehaviourInjection : MonoBehaviour
{
    [MonoInject] private readonly Container _container;
    [MonoInject] public IDependencyOne DependencyOne { get; private set; }

    [MonoInject]
    private void Inject(Container container, IDependencyOne dependencyOne)
    {
        var dependencyTwo = container
            .Resolve(typeof(IDependencyTwo));
    }

    private void Start()
    {
        var dependencyTwo = _container
            .Resolve(typeof(IDependencyTwo));

        var answerForLifeTheUniverseAndEverything = _container
            .Resolve<int>();

        var setupForDynamicType = _container
            .ResolveGenericContract<SetupScriptableObject>(
                typeof(SetupAsset<>), typeof(Enemy));
    }
}
```

### Non MonoBehaviour Injection

```csharp
public class NonMonoBehaviourInjection
{
    private readonly Container _container;
    private readonly IDependencyOne _dependencyOne;
    private readonly int _answerForLifeTheUniverseAndEverything;

    public NonMonoBehaviourInjection(Container container, IDependencyOne dependencyOne, int answerForLifeTheUniverseAndEverything)
    {
        _container = container;
        _dependencyOne = dependencyOne;
        _answerForLifeTheUniverseAndEverything = answerForLifeTheUniverseAndEverything;
    }
}
```

## Author
[![Twitter](https://img.shields.io/twitter/follow/codinggustavo.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=codinggustavo)  

[![LinkedIn](https://img.shields.io/badge/Linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/codinggustavo)  


## License

Reflex is licensed under the MIT license, so you can comfortably use it in commercial applications (We still love contributions though).
