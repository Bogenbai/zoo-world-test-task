---
name: create-factory
description: Create a factory (interface + class + Zenject binding) that returns a fully valid object in one call — instantiating prefabs through IInstantiator and reusing pooled instances when a pool exists.
---

Create a factory for the object described below.

Object / feature: $ARGUMENTS

## Steps

### 1. Find the feature folder

Factories go in the feature's `Services/` folder.

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

**Prefab-based product, reused through the shared pool:**

```csharp
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.{Feature}.Services
{
	public class {Name}Factory : I{Name}Factory
	{
		private readonly Transform _root = new GameObject("{Products}").transform;

		private readonly IInstantiator _instantiator;
		private readonly IPrefabPool _prefabPool;

		public {Name}Factory(IInstantiator instantiator, IPrefabPool prefabPool)
		{
			_instantiator = instantiator;
			_prefabPool = prefabPool;
		}

		public {Product} Create{Product}({Config} config, Vector3 at)
		{
			if (_prefabPool.TryGet(config.Prefab, out {Product} product) == false)
				product = _instantiator.InstantiatePrefabForComponent<{Product}>(config.Prefab, _root);

			product.Setup(config);
			product.gameObject.SetActive(true);
			product.transform.position = at;

			return product;
		}
	}
}
```

**Plain C# product chosen by config type:** a `switch` over the config type where each case is `_instantiator.Instantiate<{Product}>(new object[] { config })`, so Zenject injects the product's services, and a `default` that throws `ArgumentException`. Cover it with a test that builds every config subclass through a real `DiContainer`: it catches a missing `case`, a constructor mismatch and a missing binding.

### 4. Bind it in GameplayInstaller

```csharp
Container.BindInterfacesTo<{Name}Factory>().AsSingle();
```

in the feature's `Bind{Feature}Services()` method.

### 5. Verify

Refresh Unity and read the console (`verify-in-unity` skill). If the factory creates prefabs, check in play mode that pooled instances come back reset.

## Rules

- A factory is the **only** place that creates its product, and it returns the product **fully valid in one call** — set up, positioned, with its strategy, ready to tick.
- Prefabs are instantiated only through `IInstantiator` (so Zenject injects into them). Never `Object.Instantiate`, never `DiContainer`.
- Pooled products go through the shared `IPrefabPool`, keyed by prefab: `TryGet` before instantiating. Whoever removes the product calls `Put(prefab, product)`, which calls `IPooledHandler.OnPutToPool()` on its components and deactivates it.
- A factory may read configs and use `IRandomService`; it never makes gameplay decisions.
- Never `switch` on the type of a game object. A `switch` on a polymorphic config type is allowed only with a test that covers every subclass.
- Namespace: `Code.Gameplay.Features.{Feature}.Services`.
- Follow `.claude/rules/CodingGuidelines.md`.
