---
name: create-service
description: Create a domain service (interface + class + Zenject binding), or a stateful service with a separate read-only Query interface for the UI. Use for logic that is not a factory and not a config.
---

Create a service for the feature described below.

Service description: $ARGUMENTS

## Steps

### 1. Decide what it is

- **Logic without dependencies** (pure math over a struct or a config) → not a service. Write an extension method in a static class in the feature's `Extensions/` folder.
- **Creates objects** → not a service. Use the `create-factory` skill.
- **Stateless logic with dependencies** → a domain service (the default).
- **Holds state that the UI reads** → a stateful service with a writing interface and a read-only Query interface. Stateful services are the exception — check whether the state already lives somewhere before adding one.

### 2. Create the interface(s)

In the feature's `Services/` folder.

**Domain service:**
```csharp
namespace Code.Gameplay.Features.{Feature}.Services
{
	public interface I{Name}Service
	{
		{ReturnType} {MethodName}({params});
	}
}
```

**Stateful service + Query** (two files):
```csharp
namespace Code.Gameplay.Features.{Feature}.Services
{
	public interface I{Name}Service
	{
		void {WriteMethod}({params});
	}
}
```

```csharp
using System;

namespace Code.Gameplay.Features.{Feature}.Services
{
	public interface I{Name}Query
	{
		event Action On{What}Changed;

		{ReturnType} Get{Data}();
	}
}
```

The Query interface never exposes write methods.

### 3. Create the class

```csharp
namespace Code.Gameplay.Features.{Feature}.Services
{
	public class {Name}Service : I{Name}Service
	{
		private readonly I{Dependency} _{dependency};

		public {Name}Service(I{Dependency} {dependency})
		{
			_{dependency} = {dependency};
		}

		public {ReturnType} {MethodName}({params})
		{
			...
		}
	}
}
```

A stateful service implements both interfaces (`public class {Name}Service : I{Name}Service, I{Name}Query`) and raises `On{What}Changed` after every write. If it needs a Zenject lifecycle hook, add `IInitializable`, `ITickable`, `IFixedTickable` or `IDisposable` to the class, not to its public interfaces.

### 4. Bind it in an installer

Gameplay features go into `Assets/Code/Infrastructure/Installers/GameplayInstaller.cs` (scene `SceneContext`), project-wide infrastructure into `BootstrapInstaller.cs` (`ProjectContext`). Find (or add) the `Bind{Area}Services()` method and add:

```csharp
Container.BindInterfacesTo<{Name}Service>().AsSingle();
```

`BindInterfacesTo` binds both the service and the Query interface to the same instance, and wires Zenject lifecycle interfaces.

### 5. Test and verify

Pure logic gets EditMode tests (`create-tests` skill). Then refresh Unity and read the console (`verify-in-unity` skill).

## Rules

- Domain services are stateless and expose pure functions; randomness and time come in through `IRandomService` / `ITimeService` or as parameters.
- A service never creates objects (that is a factory's job) and never references views or UI.
- Events are named `On{X}`; whoever subscribes uses a `Handle{X}` method and unsubscribes in `Dispose` / `OnDisable`.
- More than 3 parameters on a method → a `*Request` / `*Dto` struct in the feature's `Data/` folder.
- Namespace: `Code.Gameplay.Features.{Feature}.Services`.
- Follow `.claude/rules/CodingGuidelines.md`.
