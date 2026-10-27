---
name: verify-in-unity
description: Verify changes in the Unity editor through Unity MCP — pick this project's editor instance, make sure the user is not playing, force a refresh, read the console, run EditMode tests and optionally check behaviour in play mode.
---

Verify the current changes in Unity.

What to check: $ARGUMENTS

## Steps

### 1. Pick the editor

Read `mcpforunity://instances`. If more than one editor is connected, call `set_active_instance` with the instance named after this project's folder. Never run tools against another project's editor.

### 2. Make sure the user is not playing

Read `mcpforunity://editor/state` and check `data.editor.play_mode.is_playing`. If it is `true`, stop and wait — forcing a recompile kills their play session. Also wait while `data.compilation.is_compiling` is `true`.

### 3. Refresh and compile

`refresh_unity` with `mode: force`, `scope: all`, `compile: request`, `wait_for_ready: true`. A scripts-only refresh misses asset changes and can report errors that belong to a previously checked-out branch.

### 4. Read the console

`read_console` with `types: ["error", "warning"]`.
- Any error → the change is not done. Fix it and go back to step 3.
- Warnings from `Assets/Plugins/Zenject` about `FindObjectsOfType` are expected; anything else from our code is not.

### 5. Run the tests

`run_tests` in EditMode, then poll `get_test_job` until it finishes. Report the real numbers — passed, failed, and the failure messages.

### 6. Check behaviour in play mode (when the change is visible in game)

- Enter play mode with `manage_editor`, wait a few seconds, read the console again.
- Inspect what matters for the change: `find_gameobjects` for spawned objects, `manage_camera` / screenshots for visuals.
- Exit play mode when done.

### 7. Check the diff

`git diff --stat` — scenes and prefabs should change by a few lines. Thousands of changed lines in a `.unity` or `.prefab` mean Unity reserialized the file; revert it and find another way.

## Report

One short paragraph: compile result, console state, test results, what was seen in play mode. If something was skipped (for example the user was playing), say so.
