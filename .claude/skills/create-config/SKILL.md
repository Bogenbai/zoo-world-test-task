---
name: create-config
description: Create a ScriptableObject config (data only — fields and getters, no methods or logic), place its asset under AddressableResources and expose it through IConfigsService.
---

Create a config for the feature described below.

Config description: $ARGUMENTS

## Steps

### 1. Find the feature folder

Look in `Assets/Code/Gameplay/Features/` for the feature. Configs go in its `Configs/` subfolder; game-wide configs go in `Assets/Code/Gameplay/Configs/`.

If the data is a variant of an existing polymorphic config (for example a new movement type), do not create a ScriptableObject — add a `[Serializable]` data-only subclass of that config's base class instead, plus whatever maps the config type to its behaviour, with a test that covers every subclass.

### 2. Generate the config class

Create `{ConfigName}.cs`:

```csharp
using Code.Infrastructure;
using UnityEngine;

namespace Code.Gameplay.Features.{Feature}.Configs
{
	[CreateAssetMenu(menuName = Constants.GameName + "/Configs/{Readable Name} Config", fileName = "{ConfigName}")]
	public class {ConfigName} : ScriptableObject
	{
		[SerializeField] private {type} {fieldName} = {defaultValue};

		[SerializeField, Tooltip("{Why this value is what it is.}")]
		private {type} {otherFieldName} = {defaultValue};

		public {Type} {PropertyName} => {fieldName};
		public {Type} {OtherPropertyName} => {otherFieldName};
	}
}
```

If the `[CreateAssetMenu]` line goes over 120 characters, put each argument on its own line.

A config that is one of many assets of its type (a collection) also gets a unique id:

```csharp
[SerializeField, ID]
private string id;

public string Id => id;
```

`[ID]` is `Code.Infrastructure.Identifiers.IDAttribute`; its drawer assigns the GUID from the inspector. Through Unity MCP, set the field to `System.Guid.NewGuid().ToString()` when creating the asset. Never regenerate an existing id.

### 3. Expose it through IConfigsService

Read `Assets/Code/Infrastructure/ConfigsManagement/IConfigsService.cs` and `ConfigsService.cs`, then:

- **A single config** (one asset of its type): add a key to `Constants.Addressables.Keys` (snake_case with a `_config` suffix), load it in `ConfigsService.Initialize()` with `_assetsService.Load<T>(key)`, add `Get{ConfigName}()`.
- **A collection** (many assets of one type): add a label to `Constants.Addressables.Labels` (snake_case, plural, `_configs`), load with `_assetsService.LoadAllByLabel<T>(label)` into a dictionary keyed by the config's `Id`, add `Get{Name}Configs()` / `Get{Name}Config(id)`.

### 4. Create the asset

Path: `Assets/AddressableResources/Configs/{Feature}/{ConfigName}.asset`. Ask the user to create it via the Create menu, or do it through Unity MCP:

- **The asset:** `manage_scriptable_object` with `action: create`, the namespace-qualified `type_name`, `folder_path`, `asset_name` and `patches` (SerializedObject property paths; object references as `{"ref": {"guid": "..."}}`).
- **`[SerializeReference]` fields:** `manage_scriptable_object` cannot set them. Use `execute_code`: load the asset, `new SerializedObject(asset)`, `FindProperty("{field}").managedReferenceValue = System.Activator.CreateInstance(type)`, fill the nested properties (`"{field}.{nestedField}"`), `ApplyModifiedProperties()`, `AssetDatabase.SaveAssets()`.
- **Addressables:** there is no dedicated MCP tool — use `execute_code` with `UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings`. Find or create the feature group (`FindGroup` / `CreateGroup`, PascalCase), add the asset with `CreateOrMoveEntry(guid, group)`, then give a single config a snake_case address with `entry.SetAddress(...)`. For a collection, add the folder instead as `{feature}_folder` and label it: `settings.AddLabel(label)`, `entry.SetLabel(label, true)`. Follow `.claude/rules/AddressablesGuidelines.md`.

`execute_code` compiles with CodeDom (C# 6): fully qualify every type, no `using`. If a game type is not visible to the compiler, resolve it by name: `System.Type.GetType("Code.Gameplay.Features.{Feature}.Configs.{Type}, Assembly-CSharp")`.

### 5. Verify

Refresh Unity, read the console, run the EditMode tests (`verify-in-unity` skill).

## Rules

- **Data only.** No methods, no logic, no lookups, no computed properties. Derived values go into a service or an extension method in the feature's `Extensions/` folder.
- All fields are `[SerializeField] private` with public getter properties.
- `Tooltip`s explain *why* a value is what it is.
- Read-only at runtime. Runtime state never lives in a config.
- Namespace mirrors the folder: `Code.Gameplay.Features.{Feature}.Configs`.
- Follow `.claude/rules/CodingGuidelines.md`.
