# Coding Guidelines

## General

We use a set of code style rules that are nearly identical to the default C# coding conventions but slightly modified for the Unity "world".

- Use Allman style braces, where each brace begins on a new line. A single line statement block can go without braces, but the block must be properly indented on its own line and must not be nested in other statement blocks that use braces.
- Encapsulation order MUST always follow the pattern where public members are declared first, followed by protected and then private.

## Code lines and length

- A single line of code MUST NOT exceed 120 characters.

## Tabs vs Spaces

- MUST use TABS over SPACES.
- TAB size MUST be set to 4.

## Naming convention

- Class, Interface, Record, Struct, Enum names MUST be declared in PascalCase.
- Interface names start with a capital `I`.
- Attribute types end with the word `Attribute`.
- Use PascalCase for constant names, both fields and local constants.
- Method names MUST be declared in PascalCase.
- Method parameters MUST be declared in camelCase.
- Private instance fields start with an underscore (`_`) and the remaining text is camelCased.
- Avoid using abbreviations or acronyms in names, except for widely known and accepted abbreviations.
- Use meaningful and descriptive namespaces that follow the reverse domain name notation.
- Avoid using single-letter names, except for simple loop counters.
- When naming public members of types, such as fields, properties, events, use PascalCase. Also use PascalCase for all methods and local functions.

## Variable declaration

Declaration MUST be explicit and the keyword `var` MUST be avoided. As the type is already declared explicitly, skip the type after the `new` keyword:

```csharp
Item item = new();
```

Naming MUST be understandable, readable and self-explanatory. Any form of abbreviation or shortening SHOULD be avoided.

```csharp
// Acceptable
public Vector3 Position { get; private set; }
public Quaternion Rotation { get; private set; }
public Rigidbody Rigidbody { get; private set; }

// Forbidden
public Vector3 Pos { get; private set; }
public Quaternion Rot { get; private set; }
public Rigidbody Rb { get; private set; }

// Such example can be considered as an exception
public string Id { get; private set; }
```

A name itself SHOULD NOT be longer than 32 characters.

Private variables MUST include `_` as a prefix:

```csharp
private Item _item;
```

Unity serialized fields are considered an exception, and `_` MUST be removed:

```csharp
[SerializeField] private Sprite icon;
```

You can use the `SF` shortcut in the directives section:

```csharp
using SF = UnityEngine.SerializeField;
```

So a serialized field declaration can be shortened like this, but it's not required:

```csharp
[SF] private Sprite icon;
```

## Property and field declaration

Properties SHOULD NOT contain logic, as it reduces readability. A method should be created instead.

Attributes that are assigned to a field or property MUST always be declared above the field or property:

```csharp
[ShowInInspector, UsedImplicitly]
private readonly HashSet<string> _names;
```

A single `SerializeField` attribute is considered an exception, so it MUST always be declared on the same line as the field or property:

```csharp
[SerializeField] private Button buyButton;
```

The default value of a field or property MUST be declared within the line of declaration:

```csharp
[ShowInInspector, UsedImplicitly]
private readonly HashSet<string> _names = new();
```

## Scripting define declaration

Scripting defines, including custom definitions, MUST be declared in ALL UPPER case with underscore separators. There is NO indentation used for scripting defines:

```csharp
#if UNITY_EDITOR
	public void Example()
	{
		// code
	}
#endif
```

When using `#if UNITY_EDITOR`, the `using` directives that are used in this block should be covered in `#if UNITY_EDITOR` too, because they can be accidentally removed as "unused".

## Method declaration and arguments

- There SHOULD NOT be more than 3 arguments per method.
- An argument count over 3 SHOULD BE extracted into a struct.
- The extracted struct SHOULD have one of the following suffixes: `Request`, `Response`, `Dto`.
- Extracted structs (and other DTOs) MUST live in a `Data` subfolder of their feature, with the namespace ending in `.Data`.
- The exception is `Construct` or a C# class constructor, which CAN contain more than 3 arguments.
- `Construct` or a C# constructor with MORE than 3 arguments MUST be indented downwards.
- `Construct` or a C# constructor with 3 or fewer arguments CAN be indented downwards, but it is not required.

Example of the 3 argument exception case:

```csharp
[Inject]
public void Construct(IConfigService configs, IHealthService healthService, IPathfindingService pathfinding)
{
	_configs = configs;
	_healthService = healthService;
	_pathfinding = pathfinding;
}
```

Example of the 3 argument exception case, ALTERNATIVE:

```csharp
[Inject]
public void Construct(
	IConfigService configs,
	IHealthService healthService,
	IPathfindingService pathfinding)
{
	_configs = configs;
	_healthService = healthService;
	_pathfinding = pathfinding;
}
```

When indented downwards, the opening `(` MUST always stay on the declaration line and NOT move to a new line.

A single method SHOULD be small and have a single responsibility. A good rule is to keep methods around 50–80 lines of code.

The service name can be shortened if it doesn't reduce readability, but this is not required:

```csharp
private IConfigService _configs;
```

## Events

- Events should be named with the `On` prefix, like `OnDeath`.
- An event handler method should have the `Handle` prefix, like `HandleDeath`.
- EVERY subscription handler method should be unsubscribed.

```csharp
private void OnEnable()
{
	harvestToolUpdate.OnToolLevelUpdated += HandleToolLevelUpdated;
}

private void OnDisable()
{
	harvestToolUpdate.OnToolLevelUpdated -= HandleToolLevelUpdated;
}
```

Lambda expressions SHOULD NEVER be used as event handlers:

```csharp
// Never use a lambda expression in a case like this.
// Use a normal method handler instead.
toolService.OnToolLevelUpdated += (int level) => { Debug.Log("new level: " + level); };
```

## Namespaces

Project code files SHOULD be placed inside the `Code/` folder. Namespaces MUST contain each subdirectory that the file is under:

```csharp
namespace Code.Gameplay.Features.Hero.Services
```

The declarations MUST be placed inside `{ }`:

```csharp
namespace Code.Gameplay.Features.Hero.Services
{
	// declaration
}
```

## Class Structure

```csharp
public class ExampleClass : MonoBehaviour, IInitializable
{
	// Serialized fields go first
	[SF] private Image icon;
	[SF] private Button button;
	[SF] private Slider slider;

	// Internal class variables go after serialized fields
	public int Amount;
	protected StatusTypeId StatusTypeId;
	private float _cooldown;

	// Injected fields go after internal variables
	private ITimeService _timeService;
	private IConfigsService _configsService;
	private IStatusFactory _statusFactory;

	// Constants go after injected fields
	public const int MaxPageCount = 5;
	protected const int MinPageCount = 1;
	private const float AbilityDuration = 5f;

	// Properties go after constants
	public bool CanBeOpened { get; private set; }
	protected int PageIndex { get; private set; }
	private BalanceConfig BalanceConfig => _configsService.GetBalanceConfig();

	// Events go after properties
	public event Action OnPageSwitched;
	protected event Action OnPageClosed;
	private event Action OnPageCreated;

	// Then ALWAYS goes the class constructor
	[Inject]
	private void Construct(ITimeService timeService)
	{
		_timeService = timeService;
	}

	// Initialize methods
	public void Initialize() { }

	private void Awake() { }

	private void Start() { }

	// Subscribe/Unsubscribe methods
	// In a plain class they should be moved to the constructor or IInitializable.Initialize()
	private void OnEnable()
	{
		// Subscription handler method name should always start with the "Handle" prefix
		_timeService.OnTimePassed += HandleTimePassed;
	}

	// In a plain class it should be moved to IDisposable.Dispose()
	private void OnDisable()
	{
		// EVERY subscription handler method should be unsubscribed
		_timeService.OnTimePassed -= HandleTimePassed;
	}

	// If the class has some kind of Setup method, it should be placed here
	public void Setup(int timeDuration) { }

	// If the class has a Cleanup method, it should be placed here
	private void Cleanup() { }

	// If the class has Update/Tick/Process methods, they should be placed here
	// Tick() or Process() or Execute()
	private void Update() { }

	// Internal methods
	// Should be sorted by access modifier
	public void StartTimer() { }

	public float GetTimerDuration() { ... }

	protected virtual bool TimerIsDisabled() { ... }

	private void ProcessTimer() { }

	private void HandleTimePassed() { }
}
```

## Sort and split fields

Wrong. Fields are declared in a chaotic way:

```csharp
[SF] private TMP_Text passNameText;
[SF] private Image seasonArt;
[SF] private TMP_Text seasonTimeText;
[SF] private Button challengeButton;
private ClipTransition _closeAnimation;
private int _amount;
private AnimancerComponent _animancer;
private ClipTransition _openAnimation;
```

Correct. Split fields by their type to improve readability:

```csharp
[SF] private TMP_Text passNameText;
[SF] private TMP_Text seasonTimeText;

[SF] private Image seasonArt;

[SF] private Button challengeButton;

private int _amount;

private AnimancerComponent _animancer;

private ClipTransition _closeAnimation;
private ClipTransition _openAnimation;
```

You can use the `[Space]` attribute to make it even more readable in Unity's inspector.

## Condition checks

Wrong. Sometimes it's really easy to miss `!`:

```csharp
if (!someBool)
{
	DoStuff();
}
```

Correct. Use `== false` instead:

```csharp
if (someBool == false)
{
	DoStuff();
}
```

## File separation

Each class, enum, interface etc. should be in a separate file. The exception is derived interfaces/classes with the same name:

```csharp
public interface IDataProvider
{
	void Clear();
}

public interface IDataProvider<T> : IDataProvider where T : class
{
	void Register(string id, T data);
	void Unregister(string id);
	T Get(string id);
	bool TryGet(string id, out T item);
	bool Has(string id);
	IEnumerable<DataItem<T>> GetAll();
}
```
