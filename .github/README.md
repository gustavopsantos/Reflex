<div align=center>   

<p align="center">
  <img src="graphics\logo.png" width="250">
</p>

### Blazing fast, minimal but complete dependency injection library for <a href="https://unity.com/">Unity</a>

Reflex is an [Dependency Injection](https://stackify.com/dependency-injection/) framework for [Unity](https://unity.com/). Making your classes independent of its dependencies, granting better separation of concerns. It achieves that by decoupling the usage of an object from its creation. This helps you to follow SOLID’s dependency inversion and single responsibility principles. Making your project more **readable, testable and scalable.**

[![Discord](https://img.shields.io/static/v1?label=&labelColor=5865F2&message=Support&color=grey&logo=Discord&logoColor=white&url=https://discord.gg/XM47TsGScH)](https://discord.gg/XM47TsGScH)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Tests](https://github.com/gustavopsantos/reflex/actions/workflows/run-tests.yml/badge.svg?branch=main)
[![PullRequests](https://img.shields.io/badge/PRs-welcome-blueviolet)](http://makeapullrequest.com)
[![Releases](https://img.shields.io/github/release/gustavopsantos/reflex.svg)](https://github.com/gustavopsantos/reflex/releases)
[![OpenUPM](https://img.shields.io/npm/v/com.gustavopsantos.reflex?label=OpenUPM&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gustavopsantos.reflex/)
[![Unity](https://img.shields.io/badge/Unity-2021+-black.svg)](https://unity3d.com/pt/get-unity/download/archive)

</div>

---

<details>
<summary>📌 Table Of Contents</summary>

- [Overview](#-overview)
- [Installation](#-installation)
  - [Unity Package Manager](#unity-package-manager)
  - [Open Unity Package Manager](#open-unity-package-manager)
  - [Unity Package](#unity-package)
- [Getting Started](#-getting-started)
- [Execution Order](#-execution-order)
- [Injection Strategy](#-injection-strategy)
- [Container Hierarrchy](#-container-hierarchy)
- [Scopes](#-scopes)
- [Bindings](#-bindings)
- [Resolving](#-resolving)
- [Selective Resolution Alternative](#-selective-resolution-alternative)
- [Callbacks](#-callbacks)
- [Attributes](#-attributes)
- [Manual Injection](#-manual-injection)
- [Extensions](#-extensions)
- [Debugger](#-debugger)
- [Settings](#-settings)
- [Performance](#-performance)
- [Scripting Restrictions](#-scripting-restrictions)
- [Support](#-support)
- [License](#-license)

</details>

---

## 👀 Overview
- **Fast:** up to 414% faster than VContainer, up to 800% faster than Zenject.
- **GC Friendly:** up to 28% less allocations than VContainer, up to 921% less allocations than Zenject.
- **AOT Support:** Basically there's no runtime Emit, so it works fine on IL2CPP builds. [<sup>[*]</sup>](#-scripting-restrictions)
- **Contract Table:** Allows usages of APIs like `container.All<IDisposable>`
- **Immutable Container**: Performant thread safety free of lock plus predictable behavior.
- **Source Generated**: Implements modern Roslyn source generator to minimize reflection usage and speed up injection.

Compatible with the following platforms:

- iOS
- Android
- Windows/Mac/Linux
- PS4/PS5
- Xbox One/S/X and Xbox Series X/S
- WebGL

---

## 💾 Installation
You can install Reflex using any of the following methods:

### Unity Package Manager
```
https://github.com/gustavopsantos/reflex.git?path=/Assets/Reflex/#14.1.0
```

1. In Unity, open **Window** → **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter url above and press **Add**.

### Open Unity Package Manager

```bash
openupm install com.gustavopsantos.reflex
```

### Unity Package
1. Download the .unitypackage from [releases](https://github.com/gustavopsantos/reflex/releases) page.
2. Import Reflex.X.X.X.unitypackage

---

## 🚀 Getting Started
1. [Install Reflex](#-installation)
2. Create `RootInstaller.cs` with 
```csharp
using Reflex.Core;
using UnityEngine;

public class RootInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue("Hello"); // Note that values are always registered as singletons
    }
}
```
3. In unity project window
4. Right click over any folder, Create → Reflex → RootScope. Since RootScope is strongly referenced by ReflexSettings, you can create it anywhere, it does not need to be inside `Resources` folder.
5. Select `RootScope` you just created
6. Add `RootInstaller.cs` as a component
7. Create directory `Assets/Resources`
8. Right click over `Resources` folder, Create → Reflex → Settings. ReflexSettings should always be created directly inside `Resources` folder, without any subfolder.
9. Select `ReflexSettings` ScriptableObject and add the `RootScope` prefab to the RootScopes list
10. Create new scene `Greet`
11. Add `Greet` to `Build Settings` → `Scenes In Build`
12. Create `Greeter.cs` with
```csharp
using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;

public class Greeter : MonoBehaviour
{
    [Inject] private readonly IEnumerable<string> _strings;

    private void Start()
    {
        Debug.Log(string.Join(" ", _strings));
    }
}
```
13. Add `Greeter.cs` to any gameobject in `Greet` scene
14. Inside Greet scene, create a scene scope, Right Click on Hierarchy > Reflex > SceneScope.
15. Create `GreetInstaller.cs` with
```csharp
using Reflex.Core;
using UnityEngine;

public class GreetInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterValue("World"); // Note that values are always registered as singletons
    }
}
```
16. Add `GreetInstaller.cs` to the `SceneScope` you created on step 14
17. Create new scene `Boot`
18. Add `Boot` to `Build Settings` → `Scenes In Build`
19. Create `Loader.cs` with
```csharp
using Reflex.Core;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        void InstallExtra(UnityEngine.SceneManagement.Scene scene, ContainerBuilder builder)
        {
            builder.RegisterValue("of Developers");
        }
        
        // This way you can access ContainerBuilder of the scene that is currently building
        ContainerScope.OnSceneContainerBuilding += InstallExtra;

        // If you are loading scenes without addressables
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Greet").completed += operation =>
        {
            ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        };

        // If you are loading scenes with addressables
        UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("Greet").Completed += operation =>
        {
            ContainerScope.OnSceneContainerBuilding -= InstallExtra;
        };
    }
}
```
20. Add `Loader.cs` to any gameobject at `Boot` scene
21. Thats it, hit play from `Boot` scene
22. When Greet scene is loaded, there should be 3 instances implementing string contract
23. So when Greeter.Start is called, you should see the following log in the unity console: `Hello World of Developers`

---

## 🎬 Execution Order
<p align="center">
  <img src="graphics/execution-order.png" />
</p>

---

## 🎯 Injection Strategy
As of version 8.0.0 Reflex has stopped automatically managing dependency injection for any scene.

If you plan on using dependency injection in one of your scenes, add a game object somewhere in the hierarchy with a `ContainerScope` component attached. You can still manage shared dependencies using Root Container or utilize this Scene Containers for limited access. This component must be present at scene load time.

This allows users to consume injected dependencies on callbacks such as `Awake` and `OnEnable` while giving more granular control over which scene should be injected or not.

---

## 🌱 Container Hierarchy
### Default Behaviour
Reflex's default strategy for creating containers involves initially generating a root container. For each newly loaded scene, an additional container is created, which always inherits from the root container. This container hierarchy mirrors the flat hierarchy of Unity scenes. You can see how the structure looks like below:

```mermaid
graph
RootContainer --> BootScene
RootContainer --> LobbyScene
RootContainer --> GameScene
RootContainer --> GameModeTwoScene
```

### Override scene container parent
To do this or whatever else you want with scene `ContainerBuilder` you can access it with `SceneScope.OnSceneContainerBuilding` like we show in `Loader.cs` in "Getting Started" section.
```csharp
// here we take boot scene container just for an example, you can use any container you need
var bootSceneContainer = gameObject.scene.GetSceneContainer();
void OverrideParent(Scene scene, ContainerBuilder builder) => builder.SetParent(bootSceneContainer);
ContainerScope.OnSceneContainerBuilding += OverrideParent;

// If you are loading scenes without addressables
SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive).completed += operation =>
{
    ContainerScope.OnSceneContainerBuilding -= OverrideParent;
};

// If you are loading scenes with addressables
Addressables.LoadSceneAsync("Lobby", LoadSceneMode.Additive).Completed += operation =>
{
    ContainerScope.OnSceneContainerBuilding -= OverrideParent;
};
```

By utilizing this API, you can create hierarchical structures such as the one shown below:

```mermaid
graph
RootContainer-->BootScene
BootScene-->LobbyScene
```


>1. Please note that it is not possible to override the parent container for the initial scene loaded by Unity.
>2. Exercise caution when managing the scene lifecycle with this type of hierarchy. For example, unloading a parent scene before its child scenes can lead to unexpected behavior, as the parent container will be disposed while the child scenes are still active. As a general rule, always unload the scene hierarchy from the bottom up, starting with the child scenes and progressing to the parent scenes.



---

## 📦 Scopes
Container scoping refers to the ability of being able to create a container inheriting the registrations of its parent container while also being able to extend it.

### Root Scope
The root scope by default share all its dependencies with all SceneScopes.
It is created lazily once the first scene containing a ContainerScope is loaded.
To register bindings to it, create a prefab, name it how you wish, the name is not used as a identifier, and attach a "ContainerScope" component to it.
Select ReflexSettings and add the prefab you created to the RootScopes list.
Then, create your installer as MonoBehaviour and implement IInstaller interface.
Remember to attach your installer to the RootScope prefab, as RootScope searches for every child implementing IInstaller when it's time to create the Root Container.
There's a menu item to ease the process: Assets > Create > Reflex > RootScope
You can create multiple RootScope prefabs, and when its time to create the root container, all active RootScope prefabs will be merged, this allow a better separation of concerns if required.
Note that RootScope prefab is not required, so if ReflexSettings.RootScopes list is empty, an empty root container will be created.
RootContainer instance will be disposed once app closes/app quits.
Invokes `ContainerScope.OnRootContainerBuilding` static event while being built in case you need to dynamically extend it, you can use `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]` method attribute to register to this event.
> Note that unity does not call OnDestroy deterministically, so rule of thumb is do not rely on injected dependencies on OnDestroy event functions.

### Scene Scope
It is scoped from RootScope, inheriting all its bindings.
It is created and injected on Awake, using a custom execution order to execute as the earliest Awake, see ContainerScope.SceneContainerScopeExecutionOrder. 
To register bindings to it, create a gameobject on desired scene, name it "SceneScope", put it as root game object, and attach a "ContainerScope" component to it.
Then, create your installer as MonoBehaviour and implement IInstaller interface.
Remember to attach your installer to your SceneScope gameobject, as SceneScope searches for every child implementing IInstaller when it's time to create the SceneScope container.
There's a menu item to ease the process: GameObject > Reflex > SceneScope
Remember to have a single SceneScope to avoid undesired behaviour.
Note that SceneScope gameobject is required only if you want its scene to be injected, in case Reflex do not find SceneScope, the scene injection will be skipped for that specific scene missing SceneScope.
SceneScope instance will be disposed once scene is unloaded.
Invokes `ContainerScope.OnSceneContainerBuilding` static event while being built for every scene containing a SceneScope in case you need to dynamically extend it,
> Note that unity does not call OnDestroy deterministically, so rule of thumb is do not rely on injected dependencies on OnDestroy event functions. 

### Manual Scoping
```csharp
using var childContainer = parentContainer.Scope(builder =>  
{  
  // Extend your scoped container by adding extra registrations here  
});
```

## 🔩 Bindings
### ContainerBuilder.RegisterValue
Used to register **user-created objects** into the container.

* The instance is created **outside** the container
* The container **does not construct this object
* Gets disposed by the container if it implements `IDisposable`
* The registered instance always behaves as a **singleton**
---
### ContainerBuilder.RegisterType
Registers a **type** that the container is responsible for creating.

* The type must be constructible by the container
* The container resolves constructor dependencies automatically
* The container resolves decorated fields, properties and methods automatically
* Instances are created according to the configured **resolution**
* Instances are provided according to the configured **lifetime**
---
### ContainerBuilder.RegisterFactory
Allows the user to provide a **factory function** that the container will invoke to create instances.

* Full control over object creation
* Can depend on runtime data
* Factory receives the resolution container as context for further resolutions if needed
* Calls to the registered function happens according to the configured **resolution**
* Calls to the registered function happens according to the configured **lifetime**
---
### Resolution
The `Resolution` enum defines **when** a dependency is instantiated.
#### Lazy
* Instance is created **only when first resolved**
* Default behavior in most DI frameworks
* Improves startup performance

#### Eager
* Instance is created **during container build**
* Useful for early validation
* Ideal for critical systems required at startup
---

### Lifetime
The `Lifetime` enum defines **how** instances will be provided.
#### Singleton
* One instance shared **globally**
* Same object returned for every resolve
#### Scoped
* One instance per **container**
* Each container has their own unique instance
#### Transient
* A **new instance per resolve**
* No instance sharing
#### Summary
| Lifetime  | Instance Behavior   |
| --------- | ------------------- |
| Singleton | One global instance |
| Scoped    | One per container   |
| Transient | One per resolve     |

## 🔍 Resolving
### Constructor
If your type is non-mono, and it's going to be created by the container, then the most recommended way to inject dependencies into it its by constructor injection.
It's simply as just requesting the contracts you need as following example:
```csharp
private class Foo
{  
	...
  
	public Foo(IInputManager inputManager, IEnumerable<IManager> managers)  
	{  
		...
	}  
}
```

If resolution fails on a constructor parameter, Reflex will attempt to use the default value provided in the parameter - if one is present.

> Note that constructor injection relies on `Resolve<TContract>` API, so in case there's two objects with `IInputManager` contract, the last one will be injected. 

### Attribute
Attribute injection is the way to go for **MonoBehaviours**.
You can use it to inject fields, writeable properties and methods like following:
```csharp
class Foo : MonoBehaviour  
{  
	[Inject] private readonly IInputManager _inputManager;  
	[Inject] public IEnumerable<IManager> Managers { get; private set; }  
  
	[Inject]  
	private void Inject(IEnumerable<int> numbers) // Method name here does not matter  
	{  
	  ...
	}  
}
```

If resolution fails on a function parameter, Reflex will attempt to use the default value provided in the parameter - if one is present.

> Note that attribute injection also works on non-mono classes.
### Single
`Container::Single<TContract>` actually validates that there's a single binding implementing given contract, and returns it.
If there's more than one the following exception will be thrown.
```
InvalidOperationException: Sequence contains more than one element
```
It's recommended for every binding that you know that there should be a single binding implementing the contract.
### Resolve
`Container::Resolve<TContract>` runs no validations, and return the last valid object implementing given contract.

### All
`Container::All<TContract>` returns all objects implementing given contract.
Example:
```csharp
private void Documentation_Bindings()  
{
	var container = new ContainerBuilder()
		.RegisterValue(1)
		.RegisterValue(2)
		.RegisterValue(3)
		.Build();

	Debug.Log(string.Join(", ", container.All<int>())); // Prints: 1, 2, 3
}
```

---
## 🍒 Selective Resolution Alternative
Selective Resolution is the technique of resolving a specific dependency or implementation by using a composite key (often a combination of a string identifier and a type). This approach allows developers to choose exactly which binding to use in scenarios where multiple bindings of the same type are registered.
Reflex does not support selective resolution natively, there are no `WithId` builder methods or `[Inject(Id = "FooId")]` attributes like in some other DI frameworks. However there is a simple and type-safe alternative: instead of registering multiple contracts for the same type (e.g., two string registrations), you can create unique wrapper types to distinguish them.
Below is an example demonstrating this approach:
```cs
using NUnit.Framework;
using Reflex.Core;
using UnityEngine;

namespace Reflex.EditModeTests 
{
    public class TypedInstance<T> 
    {
        private readonly T _value;
        protected TypedInstance(T value) => _value = value;
        public static implicit operator T(TypedInstance<T> typedInstance) => typedInstance._value;
    }

    public class AppName : TypedInstance<string> 
    {
        public AppName(string value): base(value) {}
    }

    public class AppVersion : TypedInstance<string> 
    {
        public AppVersion(string value): base(value) {}
    }

    public class AppWindow
    {
        private readonly string _appName;
        private readonly string _appVersion;

        public AppWindow(AppName appName, AppVersion appVersion) 
        {
            _appName = appName;
            _appVersion = appVersion;
        }

        public void Present() => Debug.Log($"Hello from {_appName} version: {_appVersion}");
    }

    public class SelectiveBindingTests 
    {
        [Test]
        public void TestSelectiveBinding() 
        {
            var container = new ContainerBuilder()
                .RegisterValue(typeof (AppWindow))
                .RegisterValue(new AppVersion("0.9"))
                .RegisterValue(new AppName("MyHelloWorldConsoleApp"))
                .Build();

            var appWindow = container.Resolve<AppWindow>();
            appWindow.Present();
        }
    }
}
```

---

## 🪝 Callbacks
### `ContainerBuilder::OnContainerBuilt`
OnContainerBuilt is a instance callback of ContainerBuilder, it is called once the container is fully built and initialized properly. 

---

## 🔖 Attributes
### InjectAttribute
Should be used to inject fields, writeable properties and methods like following:
```csharp
class Foo : MonoBehaviour  
{  
	[Inject] private readonly IInputManager _inputManager;  
	[Inject] public IEnumerable<IManager> Managers { get; private set; }  
  
	[Inject]  
	private void Inject(IEnumerable<int> numbers) // Method name here does not matter  
	{  
	  ...
	}  
}
```
> Note that `InjectAttribute` also works on non-mono classes.

### SourceGeneratorInjectable
Should be used to allow for source generation on the type being injected to, simply use it to decorate classes that implement `[Inject]` members:
```csharp
[SourceGeneratorInjectable]
public partial class Foo : MonoBehaviour  
{  
	[Inject] private readonly IInputManager _inputManager;  
	[Inject] public IEnumerable<IManager> Managers { get; private set; }  
  
	[Inject]  
	private void Inject(IEnumerable<int> numbers) // Method name here does not matter  
	{  
	  ...
	}  
}
```
> Note that `SourceGeneratorInjectable` requires that the decorated type be partial and public alongside all of its containing types:
```csharp
public partial class Foo
{
	[SourceGeneratorInjectable]
	public partial class Bar : MonoBehaviour  
	{  
		[Inject] private readonly IInputManager _inputManager;  
		[Inject] public IEnumerable<IManager> Managers { get; private set; }  
	
		[Inject]  
		private void Inject(IEnumerable<int> numbers) // Method name here does not matter  
		{  
		...
		}  
	}
}
```

### ReflexConstructorAttribute
Can be placed on constructors, telling reflex which constructor to use when instantiating an object.
By default its not required, as usually injected classes have a single constructor, so by default reflex tries to find the constructor with most arguments.
But sometimes this can be required if you need more granular control on which construtor reflex should use.

---

## 💉 Manual Injection

If objects (plain old c# objects or unity objects) are created during runtime, theres no way reflex can detect this creation to auto inject the object, this needs to be done manually using one of the following methods:

```csharp
AttributeInjector::void Inject(object obj, Container container)
// Injects given object fields, properties and methods that was annotated with Inject attribute
```

```csharp
ConstructorInjector::object Construct(Type concrete, Container container)
// construct object of given type, using the constructor with most parameters, using given container to pull the constructor arguments
```

```csharp
GameObjectInjector::void InjectSingle(GameObject gameObject, Container container)
// Optimized code meant to find injectables (MonoBehaviours) from a given GameObject, to then, inject using AttributeInjector
// This option injects only the first MonoBehaviour found on the given GameObject
```

```csharp
GameObjectInjector::void InjectObject(GameObject gameObject, Container container)
// Optimized code meant to find injectables (MonoBehaviours) from a given GameObject, to then, inject using AttributeInjector
// This option injects all MonoBehaviours found on the given GameObject (not recursively, so it does not account for children) 
```

```csharp
GameObjectInjector::void InjectRecursive(GameObject gameObject, Container container)
// Optimized code meant to find injectables (MonoBehaviours) from a given GameObject, to then, inject using AttributeInjector
// This option injects all MonoBehaviours found on the given GameObject and its childrens recursively 
```

```csharp
GameObjectInjector::void InjectRecursiveMany(List<GameObject> gameObject, Container container)
// Optimized code meant to find injectables (MonoBehaviours) from a given GameObject, to then, inject using AttributeInjector
// This option injects all MonoBehaviours found on the given list of GameObject and its childrens recursively 
```
### Components
An alternative approach is to utilize the `GameObjectSelfInjector`, which can be attached to a prefab to resolve its dependencies at runtime. Through the inspector, you can select the injection strategy: `Single`, `Object`, or `Recursive`. Each strategy invokes the corresponding method in the `GameObjectInjector` class.

---

## 🧩 Extensions

### GetSceneContainer
```csharp
// Allows you to get a scene container, allowing you to resolve/inject dependencies in a different way during runtime
SceneExtensions::GetSceneContainer(this Scene scene)
{
    return UnityInjector.ContainersPerScene[scene];
}

// Usage example:
var foo = gameObject.scene.GetSceneContainer().Resolve<IFoo>();
```

---

## 🐛 Debugger

It can be accessed from menu item  Window → Analysis → Reflex Debugger, or from shortcut CTRL + E.  
To enable reflex debug mode you must go to Edit → Project Settings → Player, then in the Other Settings panel, scroll down to Script Compilation → Scripting Define Symbols and add `REFLEX_DEBUG`. This can be easily achieved by clicking on the bug button at bottom right corner inside Reflex Debugger Window.
> Note that debug mode reduces performance and increases memory pressure, so use it wisely.  

![Preview](graphics/reflex-debugger.png)  

### Legend

| Icon                                                                                                    | Name                                                                                                                                                                                            | Description                                                                                                                                                                          |
|---------------------------------------------------------------------------------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <img style='vertical-align:middle;' src='graphics\icon-container.png' alt="Container Icon" width="24">  | Name taken from `Name` property of a `Container` instance. Scene containers uses `scene.name` + `scene.GetHashCode()`, so you can differentiate between two instances of the same opened scene. | Represents a container, containers has a collection of bindings                                                                                                                      |
| <img style='vertical-align:middle;' src='graphics\icon-resolver.png' alt="Container Icon" width="24">   | Name created from the array of contracts you described your binding.                                                                                                                            | Represents a binding, bindings has a collection of instances, singleton will have only one instance, transients can have many instances and factories depends on your implementation |
| <img style='vertical-align:middle;' src='graphics\icon-instance.png' alt="Container Icon" width="24">   | Name taken from `Name` property of the `Type` of the concrete object.                                                                                                                           | Represents a instance, it's the concrete object that were created by the parent binding and it's being injected to consumers                                                           |

Debugger window allows you to inspect the following:
- Hierarchy of Containers, Bindings and Instances
- Binding Contracts, Kind and Lifetime
- Binding Resolution Count
- Container construction call stack (who created the container)
- Binding construction call stack (who created the binding)
- Instance construction call stack (who resolved the binding making selected instance to be instantiated)

---

## 🪛 Settings
It's a  `ReflexSettings` scriptable object instance, named `ReflexSettings` that should live inside a `Resources` folder.
It can be created by asset menu item Assets → Create → Reflex → Settings.

- logging verbosity is configured in this asset, and default value is set to `Info`
- the list of RootScopes is also configured in this asset, and default value is null

> [!IMPORTANT]
> ReflexSettings asset is obligatory to have

---

## 📊 Performance
> Resolving ten thousand times a transient dependency with four levels of chained dependencies. See [NestedBenchmarkReflex.cs](Assets/Reflex.Benchmark/NestedBenchmarkReflex.cs).

### Android + Mono
|            |       GC |     Time | GC Ratio | Time Ratio |
|------------|---------:|---------:|---------:|-----------:|
| Reflex     |  54.7 KB |    4.9ms |     100% |       100% |
| Zenject    | 503.9 KB |   34.4ms |     921% |       702% |
| VContainer |  70.3 KB |   20.3ms |     128% |       414% |

### Android + IL2CPP
|           |        GC |   Time | GC Ratio | Time Ratio |
|-----------|----------:|-------:|---------:|-----------:|
| Reflex    |  140.6 KB |  4.0ms |     100% |       100% |
| Zenject   |   1000 KB | 15.8ms |     711% |       395% |
| VContainer|  140.6 KB |  4.2ms |     100% |       105% |

### Windows + Mono
|           |         GC |   Time | GC Ratio | Time Ratio |
|-----------|-----------:|-------:|---------:|-----------:|
| Reflex    |   140.6 KB |  0.7ms |     100% |       100% |
| Zenject   |    1000 KB |  5.6ms |     711% |       800% |
| VContainer|   140.6 KB |  1.9ms |     100% |       271% |

### Windows + IL2CPP
|           |       GC |   Time | GC Ratio | Time Ratio |
|-----------|---------:|-------:|---------:|-----------:|
| Reflex    | 140.6 KB |  1.4ms |     100% |       100% |
| Zenject   |  1000 KB |  6.2ms |     711% |       442% |
| VContainer| 140.6 KB |  3.0ms |     100% |       214% |

## 🚫 Scripting Restrictions
If you are taking advantage of reflex to inject `IEnumerable<T>` in your constructors **AND** your are building for **IL2CPP**, you will probably get some exceptions like following:

```
System.ExecutionEngineException: Attempting to call method 'System.Linq.Enumerable::Cast<ANY-TYPE>' for which no ahead of time (AOT) code was generated.
```

This happens because compiler does not know at compile time that a specific `System.Linq.Enumerable::Cast<T>` should be included. And currently Reflex does not implement any type of assembly weaving.
> Reflex 4.0.0 had and assembly weaver that was relying on unity UnityEditor.Compilation.CompilationPipeline events and Mono.Cecil. But it was causing conflicts with projects using Burst. So it's being removed temporarly until a definitive solution is found.
> Most probably we are going to weave assemblies the same way unity is doing for Burst as well.

Temporary workaround example:

```csharp
class NumberManager
{
    public IEnumerable<int> Numbers { get; }

    public NumberManager(IEnumerable<int> numbers)
    {
        Numbers = numbers;
    }
    
    // https://docs.unity3d.com/Manual/ScriptingRestrictions.html
    [Preserve] private static void UsedOnlyForAOTCodeGeneration()
    {
        Array.Empty<object>().Cast<int>(); // This compiler hint will get rid of: System.ExecutionEngineException: Attempting to call method 'System.Linq.Enumerable::Cast<System.Int32>' for which no ahead of time (AOT) code was generated. 
        throw new Exception("This method is used for AOT code generation only. Do not call it at runtime.");
    }
}
```

## 🤝 Support

Ask your questions and participate in discussions regarding Reflex related and dependency injection topics at the Reflex Discord server. 

<a href="https://discord.gg/XM47TsGScH"><img src="https://amplication.com/images/discord_banner_purple.svg" /></a>

---

## 📜 License
Reflex is distributed under the terms of the MIT License.
A complete version of the license is available in the [LICENSE](LICENSE) file in
this repository. Any contribution made to this project will be licensed under
the MIT License.
