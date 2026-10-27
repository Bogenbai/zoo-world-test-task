---
name: create-tests
description: Write EditMode tests for pure logic (services, extension methods, strategies, factories, config integrity) with NUnit, FluentAssertions and NSubstitute, next to the feature in Editor/EditModeTests.
---

Write tests for the code described below.

Target: $ARGUMENTS

## Steps

### 1. Read the code under test

Read the class and list its observable behaviours: every branch of a rule, every edge case (zero, boundaries, repeated calls), every invariant. One test per behaviour.

If the code cannot be tested without a scene, say so and propose how to pull the logic out into a plain class or extension method before writing tests.

### 2. Create the fixture

`Assets/Code/Gameplay/Features/{Feature}/Editor/EditModeTests/{ClassUnderTest}Tests.cs` — the `Editor` folder compiles it into `Assembly-CSharp-Editor`, no asmdef needed.

```csharp
using Code.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Code.Gameplay.Features.{Feature}.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class {ClassUnderTest}Tests
	{
		private readonly {ClassUnderTest} _{classUnderTest} = new();

		[Test]
		public void {Subject}_Should{Behaviour}()
		{
			// Arrange
			{Argument} argument = {setup};

			// Act
			{Result} result = _{classUnderTest}.{Method}(argument);

			// Assert
			result.Should().Be({expected}, "{why this is the expected result}");
		}
	}
}
```

Fakes for dependencies come from NSubstitute:

```csharp
IRandomService randomService = Substitute.For<IRandomService>();
randomService.Chance(Arg.Any<float>()).Returns(true);
```

For config integrity tests, find assets with `AssetDatabase.FindAssets("t:{ConfigType}")`; for "every subclass is covered" tests, iterate `TypeCache.GetTypesDerivedFrom<{BaseType}>()` and create instances with `Activator.CreateInstance`.

### 3. Run

Run the EditMode tests through Unity MCP (`run_tests`, then read the job result). Report the real result — if a test fails, show the failure, do not adjust the expectation to make it pass unless the expectation itself was wrong.

## Rules

- Test names read `Subject_ShouldBehaviour` (`ExpiredTimer_ShouldFire`, `EmptyPool_ShouldInstantiate`).
- Every assertion carries a reason string.
- Every test is split into `// Arrange`, `// Act` and `// Assert` sections with an empty line between them. Leave out `// Arrange` when there is nothing to set up. A loop that both acts and checks is split: collect the results under `// Act`, check them under `// Assert`.
- No scene; no `Time.*`, no real randomness. `GameObject`s only when the code under test takes MonoBehaviours: create them through a disposable test helper and destroy them in `[TearDown]`, so the open scene stays clean. A MonoBehaviour declared in an `Editor` folder cannot be added to a `GameObject`.
- Floating-point checks use `BeApproximately`.
- Namespace mirrors the folder: `Code.Gameplay.Features.{Feature}.Editor.EditModeTests`.
- Follow `.claude/rules/CodingGuidelines.md`.
