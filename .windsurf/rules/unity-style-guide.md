---
trigger: always_on
---

**Coding style should be consistent with industry norms, defaults in Unity's C# templates and Microsoft and .NET style guides.**

### File Structure
- One class per file (with exceptions for small, related helper classes)
- Filename should match the primary class name
- Group related scripts in meaningful directories
- Keep directory structures flat where possible (max 3-4 levels of nesting)

### Script Organization
- Constants and static fields
- Serialized fields (grouped with [Header]s, if applicable)
- Public fields and properties
- Private fields
- Unity lifecycle methods (Awake, Start, Update, etc.)
- Public methods
- Private methods
- Nested types

## Naming Conventions

### General
- Use meaningful, descriptive names
- Avoid abbreviations unless universally understood
- Don't use Hungarian notation

### Classes and Types
- PascalCase for class names: `PlayerController`, `BikePhysics`
- Descriptive, noun-based names: `TerrainManager` not `ManageTerrain`

### Methods
- PascalCase for methods: `CalculateSpeed()`, `ApplyBraking()`
- Verb-based names that describe the action: `ResetPosition()` not `Position()`

### Fields and Properties
- PascalCase for public/protected properties: `MaxSpeed`
- camelCase for private/protected fields: `currentSpeed`
- Use descriptive names: `playerHealth` not `ph`
- Prefix boolean variables with is/has/can: `isGrounded`, `hasJumped`

### Parameters
- camelCase for parameters: `float maxSpeed`, `bool isActive`
- Use descriptive names: `targetPosition` not `pos`

### Interfaces
- Prefix with "I": `IInteractable`, `ILogger`

### Enums
- PascalCase for enum name and values
```csharp
public enum TerrainType {
    Bitumen,
    Gravel,
    Dirt,
    OffRoad
}
```

## Formatting

### Indentation and Spacing
- Use 4 spaces for indentation (not tabs)
- Where possible, place braces on the same line as the function signature, `if`, `for` statements etc. 
- Use a space after keywords: `if (condition)`, not `if(condition)`
- Use a space after commas in argument lists
- Use blank lines to separate logical sections

### Line Length
- Keep lines under 120 characters when possible
- Break long statements into multiple lines for readability

### Braces
- Always use braces for control structures, even for single statements
```csharp
if (isGrounded) {
    Jump();
}
```

## Comments and Documentation

### Documentation Comments
- Use XML comments (///) for public methods and classes
- Document parameters, return values, and exceptions

```csharp
/// <summary>
/// Applies a force to make the bike jump
/// </summary>
/// <param name="force">The amount of upward force to apply</param>
/// <returns>True if the jump was successful, false otherwise</returns>
public bool Jump(float force) {
    // Implementation
}
```

### Regular Comments
- Use comments to explain "why", not "what"
- Keep comments current with code changes
- Use TODO comments for pending implementation: `// TODO: Implement collision handling`

## SOLID Principles

### Single Responsibility Principle
- Each class should have only one reason to change
- Split large classes into smaller, focused ones
- Example: Separate `BikePhysics` from `BikeInput`

### Open/Closed Principle
- Classes should be open for extension but closed for modification
- Use inheritance and interfaces to allow behavior extension - but only when needed. YAGNI - If it's likely that only a single implementation will ever exist unnecessary interfaces just create extra maintenance and cognitive work.

### Liskov Substitution Principle
- Derived classes must be substitutable for their base classes
- Ensure override methods maintain the base class's behavior contract

### Interface Segregation Principle
- Keep interfaces focused and minimal
- Clients should not depend on methods they do not use

### Dependency Inversion Principle
- Depend on abstractions, not concrete implementations
- Use dependency injection for better testability

## Unity-Specific Guidelines

### Inspector Variables
- Use `[Header]` and `[Tooltip]` attributes to organize the Inspector
- Use `[SerializeField]` for private fields that need Inspector exposure
- Group related fields under meaningful headers

```csharp
[Header("Movement Settings")]
[Tooltip("Maximum speed in km/h")]
public float maxSpeed = 30f;

[Tooltip("Acceleration rate in m/s²")]
public float acceleration = 5f;
```

### MonoBehaviour Usage
- Avoid empty MonoBehaviour methods (Start, Update, etc.)
- Prefer `Awake()` for component references and one-time initialization
- Use `Start()` for initialization that depends on other components
- Consider using `FixedUpdate()` for physics operations

### Prefabs
- Favor prefabs over scene instances
- Keep prefab variants to manage variations
- Use nested prefabs for complex objects

### Scene Organization
- Use empty GameObjects as organizational containers
- Apply consistent naming for containers: UI, Managers, Environment, etc.
- Ensure objects have meaningful, descriptive names

## Testing

### Unit Testing
- Write tests for core gameplay logic
- Test each class in isolation with mocked dependencies
- Focus on testing behavior, not implementation details

### Play Mode Testing
- Create end-to-end tests for critical gameplay scenarios
- Automate testing of key player interactions
- Test performance under various conditions

## Logging

### Log Levels
- Use appropriate log levels for different messages
- Debug: Detailed development information
- Info: General runtime information
- Warning: Potential issues that don't break functionality
- Error: Critical issues that affect functionality

### Log Content
- Include context in log messages: `[BikeController] Failed to apply physics: {reason}`
- Log important state changes
- Avoid excessive logging in performance-critical code

### Unity Debug
```csharp
Debug.Log("Player started.");
Debug.LogWarning("Low performance detected.");
Debug.LogError("Critical physics calculation failed.");
```

### Custom Logger
- Consider implementing a custom logger for advanced features:
  - Categorization
  - Configurable verbosity
  - Output to file or server