# Zoo World

Top-down 3D simulation in Unity 6000.3.6f1 (URP): frogs jump, snakes crawl, and what happens on contact is decided by the food chain.

https://github.com/user-attachments/assets/a062fd99-d5a9-4b37-b56d-353b6d0a7441

## Running

Open `Assets/Scenes/Zoo.unity` and press Play.

## Adding an animal

1. Make a prefab variant of `AnimalBase` in `Assets/AddressableResources/Visuals/Animals`.
2. Create an `AnimalConfig` in `Assets/AddressableResources/Configs/Animals` with the prefab, role and movement.
3. Add it to the `AnimalSpawner` in the scene with a spawn weight.

No code needed. New behaviour is a new class: a movement type is a `MovementConfig`, an `IMovementStrategy` and one `case` in `MovementStrategyFactory`; a contact mechanic is an `ICollisionRule` bound in `GameplayInstaller`. A new food chain role is just a `FoodChainRoleConfig` asset.

## Structure

Zenject with `BootstrapInstaller` (project) and `GameplayInstaller` (scene). Code is split by feature in `Assets/Code/Gameplay/Features`, UI lives in `Assets/Code/Ui`. One `Animal` component serves every species, the differences live in `AnimalConfig`, and movement is a `RigidbodyMovement` component with its own `IMovementStrategy`. Animals are created by `AnimalFactory`, removed by `AnimalDeathService` and reused through a pool. The UI reads the domain only through query interfaces.

## Decisions

- Composition over inheritance. With a thousand species coming, a subclass per animal would mean a class per species and copied code whenever traits mix, like a jumping predator or a crawling prey. Here a species is a set of parts picked in data: one `Animal` component, a prefab, a food chain role and a movement strategy. A new species needs no code, and a new behaviour is one class that every species can use.


- Movement is a component, not a service. A cleaner split would be a plain view, per-animal movement data in a registry and one service moving everyone. The layering would be much cleaner, but that goes into a DOD/ECS direction, and as ECS was ruled out for this task - I stayed with regular components. 


- Movement settings sit inside `AnimalConfig` via `[SerializeReference]`, so one species is one asset and the movement type is a dropdown. The downside is an extra package and `[MovedFrom]` when a config class gets renamed. Separate ScriptableObjects would pay off if many species shared the same settings.


- `MovementStrategyFactory` switches on the config type. I didn't want logic in configs, and a reflection-based lookup is harder to follow. Strategies get the whole config and are created through `IInstantiator`, so the factory doesn't carry their dependencies. Neither the compiler nor Zenject validation checks that, so the factory test builds every strategy through a real container.


- Roles are data: each role lists what it eats. The three rules from the task come out of two assets, Prey eating nothing and Predator eating both, so a new role or a different food web needs no code. An enum with a switch per pair would grow with every role.


- No MV* in the UI. Widgets read the domain only through query interfaces and never get the writing ones, which is the separation MVP, MVVM and the like are there for. A presenter or view model per widget would add a class and a binding for two counters and a list of texts without changing who can do what.


- Configs load synchronously. It's one scene, so there is nothing to wait for. Async loading is the first thing I'd add if content grew.

## AI

The task didn't rule out AI, so I used it. AI speeds development up, but my firm position is that there still has to be a responsible person who maintains it, steers it and reviews the code it generates. That is how this project was made: the rules it works by are in `CLAUDE.md` and `.claude/`, and every change went through my review before it was committed. There is **zero vibe coding** in this repository.
