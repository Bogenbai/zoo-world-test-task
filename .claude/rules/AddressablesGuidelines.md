# Addressables Guidelines

## Folders

All assets that should be built into Addressables bundles must be located in `Assets/AddressableResources`. Keep in mind that it's just a regular folder: putting something in it doesn't make it addressable.

Inside `AddressableResources` the main rule is to sort folders by feature and usage — assets that are often used together should ideally share one addressable group. A feature folder can contain sub-feature folders.

## Groups and members

- Every feature is a **separate** addressable group, named in PascalCase (`Configs`, `Visuals`).
- Group members are named in snake_case.
- **Every** feature folder is added to its group as a member with the `_folder` suffix (`animals_folder`). As the folder itself is addressable, everything inside it is addressable too — a new asset added to the folder can't be forgotten in the Addressables setup.
- If an asset must be available by key, enable the `Addressable` checkbox on it. By default its path is used as the key; rename it to a readable one. For example, `Assets/AddressableResources/Visuals/PickUps/PickUp.prefab` becomes `pick_up_prefab`.
- A newly added addressable lands in `Default Local Group` — move it into the group it belongs to.
- If some assets are shared between several features, extract them into a separate `Shared` group.

## Naming

Name every addressable with its type as a suffix. It keeps the Addressables window readable:

| Asset type | Suffix |
|---|---|
| Folder | `_folder` |
| Prefab | `_prefab` |
| Texture | `_texture` |
| Material | `_material` |
| ScriptableObject | `_config` |
| ParticleSystem | `_vfx` |

And so on for other types.
