# Zoo World

Top-down 3D simulation in Unity 6000.3.6f1 (URP). An animal spawns every 1–2 seconds and wanders around on its own: frogs jump, snakes crawl. Everyone turns back at the screen edge. Two frogs bounce off each other, a snake eats a frog, and of two snakes only one survives.

## Running

Open `Assets/Scenes/Zoo.unity` and press Play. Tests are in Test Runner → EditMode.

Configs come from Addressables, so build the Addressables content (Groups → Build → New Build) before making a player build.

## Adding an animal

1. Make a prefab variant of `Assets/AddressableResources/Visuals/Animals/AnimalBase.prefab` in the same folder.
2. Create an `AnimalConfig` in `Assets/AddressableResources/Configs/Animals` (Create → Zoo World → Configs → Animal Config) and fill in the prefab, role and movement.
3. Add the config to the `AnimalSpawner` in the scene with a spawn weight.

No code needed. Both folders are addressable as a whole, so the new prefab and config show up in the reference pickers right away.

A new movement type is a `MovementConfig` subclass with data, an `IMovementStrategy` and one `case` in `MovementStrategyFactory`. If the `case` is missing, `MovementStrategyFactoryTests` fails.

A new food chain role is a `FoodChainRoleConfig` asset with the list of roles it eats, plus one more `DeathCounterWidget` in the scene if its deaths should be on screen. A new thing that happens on contact is an `ICollisionRule` and one line in `GameplayInstaller`.

## Structure

Zenject with two installers: `BootstrapInstaller` (ProjectContext) for infrastructure and `GameplayInstaller` (scene) for gameplay. Code is split by feature in `Assets/Code/Gameplay/Features`.

Every species uses the same `Animal` component, and what makes them different is data in `AnimalConfig`. Movement is a separate `RigidbodyMovement` component with its own `IMovementStrategy` instance. Animals are created only in `AnimalFactory`, removed only in `AnimalDeathService`, and reused through a pool. Spawning is an `AnimalSpawner` object in the scene with the list of animals it spawns, so the hierarchy shows what runs the game. `Time` and `Random` are behind services, so strategies and the screen-bounds math are tested without a scene.

The HUD in `Assets/Code/Ui` only reads: the death counters use `IDeathStatisticsQuery`, and the "Tasty!" texts listen to `IPredationQuery` and `IAnimalDeathQuery`. Writing interfaces never reach the UI.

## Decisions

- Movement is a component, not a service. A cleaner split would be a plain view, per-animal movement data in a registry and one service moving everyone. The layering is nicer, but that's ECS without a framework, and ECS was ruled out for this task, so I stayed with regular components.
- Velocity is set directly each physics step rather than through forces. That keeps the jump length and crawl speed exact no matter the mass or timestep.
- Movement settings sit inside `AnimalConfig` via `[SerializeReference]`, so one species is one asset and the movement type is a dropdown. The downside is an extra package and `[MovedFrom]` when a config class gets renamed. Separate ScriptableObjects would pay off if many species shared the same settings.
- `MovementStrategyFactory` switches on the config type. I didn't want logic in configs, and a reflection-based lookup is harder to follow. Strategies get the whole config and are created through `IInstantiator`, so the factory doesn't carry their dependencies. Neither the compiler nor Zenject validation checks that, so the factory test builds every strategy through a real container.
- Configs, the spawner and the death counters point to assets through `AssetReference`, not direct links. A config stays light, because its prefab loads only when the first animal of that kind is created. A direct link from the scene would also put a second copy of each config into the build next to the Addressables one, and a counter holding its own copy of the Prey role would never match the role the animals carry.
- The pool is a small generic one keyed by the prefab's asset GUID, so the "Tasty!" texts can use it too. Zenject's `MemoryPool` didn't fit because it is bound to one prefab in an installer, and here prefabs come from configs.
- There is no shared balance config. Spawn settings are fields on `AnimalSpawner` in the scene, and the screen margin has its own small `WorldConfig`. A single "GameBalance" asset was mixing unrelated features and would only keep growing.
- Roles are data: each role lists what it eats. The three rules from the task come out of two assets, Prey eating nothing and Predator eating both, so a new role or a different food web needs no code. An enum with a switch per pair would grow with every role.
- A contact goes through a chain of rules, predation first and bounce last, until one of them handles it. The alternative was a table of outcomes per role pair, but a new mechanic like shooting on contact would still need code there, while here it is one more rule.
- Both bodies get `OnCollisionEnter`, so only the animal with the smaller `EntityId` handles the pair, and an animal eaten earlier in the same physics step is skipped.
- Bounced prey are left to physics for half a second with strong damping, so they visibly fly apart and come to a stop, then stand still for another half second before moving on in the direction they flew. The timings live on the movement component of the prefab, so a heavier animal can be tuned in its variant.
- No MV* in the UI. Widgets read the domain only through query interfaces and never get the writing ones, which is the separation MVP, MVVM and the like are there for. A presenter or view model per widget would add a class and a binding for two counters and a list of texts without changing who can do what.
- Configs load synchronously. It's one scene, so there is nothing to wait for. Async loading is the first thing I'd add if content grew.
