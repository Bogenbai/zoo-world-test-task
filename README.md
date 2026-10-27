# Zoo World

Top-down 3D simulation in Unity 6000.3.6f1 (URP). An animal spawns every 1–2 seconds and wanders around on its own: frogs jump, snakes crawl. Everyone turns back at the screen edge.

## Running

Open `Assets/Scenes/Zoo.unity` and press Play. Tests are in Test Runner → EditMode.

Configs come from Addressables, so build the Addressables content (Groups → Build → New Build) before making a player build.

## Adding an animal

1. Make a prefab variant of `Assets/Prefabs/Animals/AnimalBase.prefab`.
2. Create an `AnimalConfig` in `Assets/AddressableResources/Configs/Animals` (Create → Zoo World → Configs → Animal Config) and fill in the prefab, role, spawn weight and movement.

No code needed. The folder is labeled in Addressables, so the config is picked up on its own.

A new movement type is a `MovementConfig` subclass with data, an `IMovementStrategy` and one `case` in `MovementStrategyFactory`. If the `case` is missing, `MovementStrategyFactoryTests` fails.

## Structure

Zenject with two installers: `BootstrapInstaller` (ProjectContext) for infrastructure and `GameplayInstaller` (scene) for gameplay. Code is split by feature in `Assets/Code/Gameplay/Features`.

Every species uses the same `Animal` component, and what makes them different is data in `AnimalConfig`. Movement is a separate `RigidbodyMovement` component with its own `IMovementStrategy` instance. Animals are created only in `AnimalFactory` and reused through a pool. Spawning is an `AnimalSpawner` object in the scene, so the hierarchy shows what runs the game. `Time` and `Random` are behind services, so strategies and the screen-bounds math are tested without a scene.

## Decisions

- Movement is a component, not a service. A cleaner split would be a plain view, per-animal movement data in a registry and one service moving everyone. The layering is nicer, but that's ECS without a framework, and ECS was ruled out for this task, so I stayed with regular components.
- Velocity is set directly each physics step rather than through forces. That keeps the jump length and crawl speed exact no matter the mass or timestep.
- Movement settings sit inside `AnimalConfig` via `[SerializeReference]`, so one species is one asset and the movement type is a dropdown. The downside is an extra package and `[MovedFrom]` when a config class gets renamed. Separate ScriptableObjects would pay off if many species shared the same settings.
- `MovementStrategyFactory` switches on the config type. I didn't want logic in configs, and a reflection-based lookup is harder to follow. Strategies get the whole config and are created through `IInstantiator`, so the factory doesn't carry their dependencies. Neither the compiler nor Zenject validation checks that, so the factory test builds every strategy through a real container.
- The pool is a small generic one keyed by prefab, so the "Tasty!" texts can use it too. Zenject's `MemoryPool` didn't fit because it is bound to one prefab in an installer, and here prefabs come from configs.
- There is no shared balance config. Spawn settings are fields on `AnimalSpawner` in the scene, and the screen margin has its own small `WorldConfig`. A single "GameBalance" asset was mixing unrelated features and would only keep growing.
- Configs load synchronously. It's one scene, so there is nothing to wait for. Async loading is the first thing I'd add if content grew.
