---
name: create-widget
description: Create a uGUI widget — a MonoBehaviour that reads the domain through a read-only Query interface, subscribes to its On{X}Changed event and refreshes its texts. No presenters.
---

Create a UI widget for the feature described below.

Widget description: $ARGUMENTS

## Steps

### 1. Find or create the Query it reads

The widget reads data only through a read-only Query interface (`I{Name}Query` with `Get{Data}()` methods and an `On{What}Changed` event). If none exists, create it with the `create-service` skill (stateful service + Query). The widget never receives the writing interface.

### 2. Generate the widget

Create `Assets/Code/Ui/{Feature}/{Name}Widget.cs`:

```csharp
using Code.Gameplay.Features.{Feature}.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Ui.{Feature}
{
	public class {Name}Widget : MonoBehaviour
	{
		[SerializeField] private TMP_Text {valueName}Text;

		private I{Name}Query _{name}Query;

		[Inject]
		private void Construct(I{Name}Query {name}Query)
		{
			_{name}Query = {name}Query;
		}

		private void OnEnable()
		{
			RefreshTexts();
			_{name}Query.On{What}Changed += Handle{What}Changed;
		}

		private void OnDisable()
		{
			_{name}Query.On{What}Changed -= Handle{What}Changed;
		}

		private void RefreshTexts()
		{
			{valueName}Text.text = ...;
		}

		private void Handle{What}Changed()
		{
			RefreshTexts();
		}
	}
}
```

### 3. Put it in the scene

Through Unity MCP or by hand: add the widget under the `Canvas` (Screen Space – Overlay, Canvas Scaler 1920×1080, match 0.5), anchor it, wire the serialized `TMP_Text` fields, turn off Raycast Target on every text. The widget is a scene object, so `SceneContext` injects it — no binding in `ZooInstaller` is needed.

For a widget built from a prefab at runtime (floating texts, labels that follow objects), instantiate through `IInstantiator` so it gets injected.

### 4. Verify

Refresh Unity, read the console, then enter play mode and check that the widget updates (`verify-in-unity` skill).

## Rules

- The class name ends with `Widget`, unless it names a concrete UI element (`FloatingText`).
- Text formatting lives in the widget; no game logic does.
- Subscribe in `OnEnable`, unsubscribe in `OnDisable`, handler named `Handle{X}`, never a lambda.
- Serialized fields without an underscore; injected fields with `_`.
- Per-frame data (a position that follows an object) is polled in `Update`; infrequent changes come through events.
- Namespace: `Code.Ui.{Feature}`.
- Follow `.claude/rules/CodingGuidelines.md`.
