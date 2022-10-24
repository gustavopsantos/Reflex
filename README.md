<p align="center">
  <img src="Graphics\logo-crimson.png" width="250">
</p>
<p align="center">
  <img src="Graphics\text-crimson.png" width="300">
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
- `[Inject]` Property, field and method injection attribute

## Performance
> Resolving ten thousand times a transient dependency with four levels of chained dependencies. See [NestedBenchmarkReflex.cs](Assets/Reflex.Benchmark/NestedBenchmarkReflex.cs).

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
"com.gustavopsantos.reflex": "https://github.com/gustavopsantos/reflex.git?path=/Assets/Reflex/#3.5.1"
```

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/gustavopsantos/reflex/releases) page.
2. Import Reflex.X.X.X.unitypackage

## Getting Started

### Installing Bindings

Create a Installer to install your bindings in the project context, and remember to add this component in the ProjectContext prefab, and reference it in the Installers list of the ProjectContext. See [ProjectContext.prefab](Assets/Reflex.GettingStarted/Resources/ProjectContext.prefab).

```csharp
public class ProjectInstaller : Installer
{
    public override void InstallBindings(Container container)
    {
        container.BindInstance<int>(42);
        container.BindTransient<IDependencyOne, DependencyOne>();
        container.BindSingleton<IDependencyTwo, DependencyTwo>();
    }
}
```

### MonoBehaviour Injection

> Be aware that fields and properties with [Inject] are injected only into pre-existing MonoBehaviours within the scene after the SceneManager.sceneLoaded event, which happens after Awake and before Start. See [MonoInjector.cs](Assets/Reflex/Scripts/Injectors/MonoInjector.cs).  

> If you want to instantiate a MonoBehaviour/Component at runtime and wants injection to happen, use the `Instantiate` method from Container.

```csharp
public class MonoBehaviourInjection : MonoBehaviour
{
    [Inject] private readonly Container _container;
    [Inject] public IDependencyOne DependencyOne { get; private set; }

    [Inject]
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

## Order of Execution when a Scene is Loaded

| Events                                               |
|:----------------------------------------------------:|
| SceneContext.Awake(DefaultExecutionOrder(-10000))    |
| ↓                                                    |
| Reflex.Injectors.SceneInjector.Inject                |
| ↓                                                    |
| MonoBehaviour.Awake                                  |
| ↓                                                    |
| MonoBehaviour.Start                                  |

> `Reflex.Injectors.SceneInjector.Inject` injects fields, properties and methods decorated with [Inject] attribute.

## Contexts

### Project Context
A single prefab named `ProjectContext` that should live inside a `Resources` folder and should contain a `ProjectContext` component attached
> Non-Obligatory to have

### Scene Context
A single root gameobject per scene that should contain a `SceneContext` component attached
> Non-Obligatory to have, but scenes without it wont be injected

## Bindings

### Bind Function
Binds a function to a type. Every time resolve is called to this binding, the function binded will be invoked.

### Bind Instance
Binds a object instance to a type. Every time resolve is called, this instance will be returned.
> Instances provided by the user, since not created by Reflex, will not be disposed automatically, we strongly recomend using BindSingleton.

### Bind Transient
Binds a factory. Every time the resolve is called, a new instance will be provided.
> Instances will be disposed once the container that provided the instances are disposed.

### Bind Singleton
Binds a factory. Every time the resolve is called, the same instance will be provided.
> The instance will be disposed once the container that provided the instance are disposed.

## Author
[![Twitter](https://img.shields.io/twitter/follow/codinggustavo.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=codinggustavo)  

[![LinkedIn](https://img.shields.io/badge/Linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/codinggustavo)  


## License

Reflex is licensed under the MIT license, so you can comfortably use it in commercial applications (We still love contributions though).
