---
name: create-factory
description: Create a factory (interface + class + Zenject binding) that returns a fully valid object in one call — instantiating prefabs through IInstantiator and reusing pooled instances when a pool exists.
---

Create a factory for the object described below.

Object / feature: $ARGUMENTS

## Steps

### 1. Find the feature folder

Factories go in the feature's `Services/` folder, next to the pool if there is one.

### 2. Create the interface

```csharp
namespace Code.Gameplay.Features.{Feature}.Services
{
	public interface I{Name}Factory
	{
		{Product} Create{Product}({params});
	}
}
```

At most 3 parameters; more go into a `{Product}SpawnRequest` struct in the feature's `Data/` folder.

### 3. Create the class

**Prefab-based product with a pool:**

```csharp
using Code.Infrastructure.Identifiers.Services;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.{Feature}.Services
{
	public class {Name}Factory : I{Name}Factory
	{
		private readonly IInstantiator _instantiator;
		private readonly I{Name}Pool _{name}Pool;
		private readonly IIdentifierService _identifierService;
		private readonly Transform _root;

		public {Name}Factory(
			IInstantiator instantiator,
			I{Name}Pool {name}Pool,
			IIdentifierService identifierService)
		{
			_instantiator = instantiator;
			_{name}Pool = {name}Pool;
			_identifierService = identifierService;
			_root = new GameObject("{Products}").transform;
		}

		public {Product} Create{Product}({Config} config, Vector3 at)
		{
			{Product} product = _{name}Pool.Has(config.Id)
				? _{name}Pool.Get(config.Id)
				: Instantiate(config);

			product.Spawn(at);

			return product;
		}

		private {Product} Instantiate({Config} config)
		{
			{Product} product = _instantiator.InstantiatePrefabForComponent<{Product}>(config.Prefab, _root);
			product.Setup(_identifierService.Next(), config);

			return product;
		}
	}
}
```

**Plain C# product chosen by config type:** a `switch` over the config type with a `default` that throws `ArgumentException`, plus a test that every config subclass is covered.

### 4. Bind it in ZooInstaller

```csharp
Container.BindInterfacesTo<{Name}Factory>().AsSingle();
```

in the feature's `Bind{Feature}Services()` method.

### 5. Verify

Refresh Unity and read the console (`verify-in-unity` skill). If the factory creates prefabs, check in play mode that pooled instances come back reset.

## Rules

- A factory is the **only** place that creates its product, and it returns the product **fully valid in one call** — set up, positioned, with its strategy, ready to tick.
- Prefabs are instantiated only through `IInstantiator` (so Zenject injects into them). Never `Object.Instantiate`, never `DiContainer`.
- Pools keep instances per config id behind `Has` / `Get` / `Put`, and `Put` calls `IPooledHandler.OnPutToPool()` on the product's components.
- A factory may read configs and use `IRandomService` / `IIdentifierService`; it never makes gameplay decisions.
- Never `switch` on the type of a game object. A `switch` on a polymorphic config type is allowed only with a test that covers every subclass.
- Namespace: `Code.Gameplay.Features.{Feature}.Services`.
- Follow `.claude/rules/CodingGuidelines.md`.
