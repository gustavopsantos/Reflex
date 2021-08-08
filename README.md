<p align="center">
  <img src="logo-crimson.png" width="250">
</p>
<p align="center">
  <img src="text-crimson.png" height="180">
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
10,000 iterations solving a non nested transient dependency.

|            | GC Alloc | More Alloc Than Best | Time | Slower Than Best Best |
|:----------:|:--------:|:--------------------:|:----:|:---------------------:|
| Reflex     | 235KB    | 1x                   | 5ms  | 1x
| VContainer | 512KB    | 2x                   | 13ms | 2.6x
| Zenject    | 2355KB   | 10x                  | 48ms | 9.6x

## Installation

*Requires Unity 2019+*

## Get Started

### Installing Bindings

Create a MonoInstaller to install your bindings in the project context, and remember to add this component in the ProjectContext prefab, and reference it in the Mono Installers list of the ProjectContext. See [ProjectContext.prefab](Assets/Reflex/Resources/ProjectContext.prefab).

```csharp
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings(IContainer container)
    {
        container.BindSingleton<int>(42);
        container.Bind<IDependencyOne>().To<DependencyOne>().AsTransient();
        container.Bind<IDependencyTwo>().To<DependencyTwo>().AsSingleton();
        container.BindGenericContract(typeof(SetupAsset<>)).To(
            typeof(SetupEnemy),
            typeof(SetupLevel)
        ).AsSingleton();
    }
}
```

### MonoBehaviour Injection

> Be aware that fields and properties with [MonoInject] are only injected into pre-existing MonoBehaviours within the scene after the SceneManager.sceneLoaded event, which happens after Awake and before Start. See [MonoInjector.cs](Assets/Reflex/Scripts/MonoInjector.cs).

```csharp
public class MonoBehaviourInjection : MonoBehaviour
{
    [MonoInject] private readonly IContainer _container;
    [MonoInject] public IDependencyOne DependencyOne { get; private set; }

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
    private readonly IContainer _container;
    private readonly int _answerForLifeTheUniverseAndEverything;

    public NonMonoBehaviourInjection(IContainer container, int answerForLifeTheUniverseAndEverything)
    {
        _container = container;
        _answerForLifeTheUniverseAndEverything = answerForLifeTheUniverseAndEverything;
    }
}
```

## Credits
- Inspired by [Zenject](https://github.com/modesttree/Zenject)

## Contributing
  
Here’s how we suggest you go about proposing a change to this project:
  
1. [Fork this project][fork] to your account.
2. [Create a branch][branch] for the change you intend to make.
3. Make your changes to your fork.
4. [Send a pull request][pr] from your fork’s branch to our `master` branch.
  
Using the web-based interface to make changes is fine too, and will help you
by automatically forking the project and prompting to send a pull request too.
  
[fork]: https://help.github.com/articles/fork-a-repo/
[branch]: https://help.github.com/articles/creating-and-deleting-branches-within-your-repository
[pr]: https://help.github.com/articles/using-pull-requests/

## Author
[![Twitter](https://img.shields.io/twitter/follow/codinggustavo.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=codinggustavo)  

[![LinkedIn](https://img.shields.io/badge/Linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/codinggustavo)  


## License

Reflex is licensed under the MIT license, so you can comfortably use it in commercial applications (We still love contributions though).
