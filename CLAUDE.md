# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Zoo World** — a Unity 6 (6000.3.6f1, URP) top-down 3D simulation. Animals spawn over time, move on their own and collide through physics; what happens on contact is decided by the food chain. A uGUI HUD shows how many prey and predators have died. There is no player input.

Detailed rules live in `.claude/rules/` — read the relevant one before making changes:
- `CodingGuidelines.md` — C# style, naming, class structure (read before writing any code)
- `AddressablesGuidelines.md` — asset organization and naming in Addressables
- `Gitflow.md` — branches, commits, approvals

## Repository Structure

- The Unity project is the repository root.
- All game code is under `Assets/Code/`, organized by feature: `Gameplay/Features/<Feature>/{Behaviours, Configs, Data, Extensions, Services, Strategies, Editor/EditModeTests}`.
- `Assets/Code/Gameplay/Common/` — wrappers over Unity statics (`Random/`, `Time/`); `Assets/Code/Gameplay/Configs/` — game-wide configs.
- `Assets/Code/Infrastructure/` — installers, configs and assets management, identifiers, pooling interfaces, `Constants`, `TestCategories`.
- `Assets/Code/Ui/<Feature>/` — uGUI widgets.
- `Assets/AddressableResources/Configs/` — config assets loaded through Addressables.
- `Assets/Plugins/` — vendored third-party code: Zenject (Extenject 9.3.1) and `TestingDependencies` (FluentAssertions 6.4, NSubstitute, Castle.Core; editor-only). **Never edit anything in `Assets/Plugins/`.**

## Architecture

**View → Domain → Configs.** Views (MonoBehaviours in `Behaviours/`, uGUI widgets) read the domain through queries and call services; the domain never references views or UI.

- **Composition over inheritance.** A single `Animal` component serves every animal type; everything that makes a type different lives in its `AnimalConfig` asset. No subclass per animal type and no `switch` on animal type anywhere.
- **New content is data, new behaviour is a new class.** Adding an animal type means a prefab variant and a config asset. Adding a behaviour (a movement type, an interaction rule) means adding classes, not editing existing ones.
- **Movement** is a strategy per animal instance (`IMovementStrategy`); the strategy owns per-instance state such as timers. Its data is a `[Serializable]` `MovementConfig` subclass stored inside `AnimalConfig` via `[SerializeReference, SubclassSelector]`. A factory maps each config type to its strategy — the only accepted `switch` on type, covered by a test that checks every `MovementConfig` subclass. Renaming or moving a `MovementConfig` class needs `[MovedFrom]`, otherwise the serialized data is lost.
- **Interaction rules** are pure functions in stateless services. Collision callbacks fire on both bodies, so every pair is resolved once, and an object removed earlier in the same physics step is skipped.
- **State** lives in as few places as possible: the registry of alive animals and the statistics. Every other domain service is stateless.
- **Lifecycle:** objects are created only by factories (pooled; prefabs instantiated through `IInstantiator`) and removed through one place that also updates statistics and notifies listeners.
- **UI** reads the domain through read-only query interfaces with `On{X}Changed` events and never gets the writing interface.
- **Unity statics** are wrapped: use `ITimeService` and `IRandomService`, never `Time.*` or `UnityEngine.Random` in game code.
- **No ECS.** Game objects are MonoBehaviours driven by plain C# services.

## DI (Zenject)

- Composition roots: `BootstrapInstaller` on `Assets/Resources/ProjectContext.prefab` binds project-wide infrastructure (time, random, assets, configs); `GameplayInstaller` on the scene's `SceneContext` binds gameplay features. Both are split into private `Bind{Area}Services()` methods.
- Every service, factory and query is an interface + class, bound with `Container.BindInterfacesTo<X>().AsSingle()`.
- MonoBehaviours receive dependencies through `[Inject] private void Construct(...)`; scene objects are injected by `SceneContext` automatically.
- Prefabs are instantiated through `IInstantiator`, and only inside factories.
- Never inject `DiContainer` into game code. No SignalBus, no `PlaceholderFactory`, no `MemoryPool`, no singletons, no `FindObjectOfType`, no `GameManager`.

## Configs

- ScriptableObjects (or `[Serializable]` classes nested in them) hold **data only**: `[SerializeField] private` fields with getter properties. No methods, no logic, no lookups — derived values go into services or extension methods.
- `[CreateAssetMenu(menuName = Constants.GameName + "/Configs/<Name> Config", fileName = "<Name>Config")]`.
- `Tooltip`s explain *why* a value is what it is, not what the field is.
- Configs are loaded through `IConfigsService`, using the Addressables labels and keys in `Constants.Addressables`.

## Code Conventions

Read `.claude/rules/CodingGuidelines.md` before writing code. The rules that are easiest to miss:
- Tabs (size 4), Allman braces, lines up to 120 characters, block-scoped namespaces that mirror the folder path (`Code.Gameplay.Features.Animals.Services`).
- Explicit types, never `var`; `Item item = new();`.
- `== false` instead of `!`.
- Private fields `_camelCase`; serialized fields without the underscore, `[SerializeField]` on the same line when it is the only attribute.
- Events `On{X}`, handlers `Handle{X}`, never a lambda as a handler, every subscription has a matching unsubscription.
- At most 3 method arguments; more go into a `*Request` / `*Dto` struct in the feature's `Data/` folder. Constructors and `Construct` are the exception, one argument per line.
- One type per file, class members in the order from the guidelines.
- An empty line before `return`, except in single-line methods and when `return` is the only statement in the body.
- No explanatory comments in C#. The only exception is a one-line *why* comment where the reason cannot be read from the code itself (an engine quirk, an ordering constraint).

## Tests

- EditMode tests live next to their feature in `<Feature>/Editor/EditModeTests/` and compile into `Assembly-CSharp-Editor`; game code has no asmdefs.
- NUnit + FluentAssertions with a reason in every assertion (`.Should().Be(expected, "...")`); NSubstitute for fakes.
- `[Category(TestCategories.Gameplay)]` on every fixture; test names read `Subject_ShouldBehaviour`.
- Pure logic (rules, math, strategies, factories) must be testable without a scene.

## Unity MCP

The Unity editor is reachable through the `UnityMCP` server (CoplayDev MCP for Unity).

- Several editors can be connected at once. Read `mcpforunity://instances` and call `set_active_instance` with the instance named after this project's folder before using any other tool.
- **Before any refresh or compile, check that the user is not in play mode** (`mcpforunity://editor/state` → `data.editor.play_mode.is_playing`). If they are playing, stop and wait.
- After changing scripts or assets: `refresh_unity` with `mode: force`, `scope: all`, `compile: request`, then `read_console` for errors. A file that was written is not a file that compiles.
- `execute_code` compiles with CodeDom (C# 6): no `using` directives (fully qualify every type), no `out var`, no pattern matching, no tuples. Deleting assets requires `safety_checks: false` — look at the target first.
- `manage_scriptable_object` creates config assets and sets plain fields and object references, but not `[SerializeReference]` fields — set those through `execute_code` with `SerializedProperty.managedReferenceValue`. Addressables have no dedicated tool: use `execute_code` with `AddressableAssetSettingsDefaultObject.Settings`.
- Edit a prefab in a single `execute_code` call: `PrefabUtility.LoadPrefabContents`, build the hierarchy, wire serialized fields through `SerializedObject`, `SaveAsPrefabAsset`, `UnloadPrefabContents` in a `finally`. Check the git diff afterwards — thousands of deleted lines mean Unity reserialized the whole file; revert and find another way.
- Run tests with `run_tests` (EditMode) and read the results; do not report work as done while tests are red.

## Git

Follow `.claude/rules/Gitflow.md`. In short: work on `feature/*` branches from `dev`, **never commit without explicit approval of that specific commit**, never push or merge unless asked. **`CLAUDE.md` is never modified** — if something seems to belong here, tell the user instead.
