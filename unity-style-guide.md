# Windsurf Unity Style Guide

This style guide defines coding standards and best practices for the Time Riders Unity project. Following these guidelines will ensure code consistency, maintainability, and quality across the project.

## Table of Contents
1. [Code Organization](#code-organization)
2. [Naming Conventions](#naming-conventions)
3. [Formatting](#formatting)
4. [Comments and Documentation](#comments-and-documentation)
5. [SOLID Principles](#solid-principles)
6. [Unity-Specific Guidelines](#unity-specific-guidelines)
7. [Testing](#testing)
8. [Logging](#logging)

## Code Organization
- One class per file (with exceptions for small, related helper classes)
- Filename should match the primary class name
- Group related scripts in meaningful directories by feature/domain
- Keep directory structures flat where possible (max 3-4 levels)

Example:
```csharp
using UnityEngine;

namespace TimeRiders {
    public class BikeController : MonoBehaviour {
        // Public fields and properties
        [Header("Movement Settings")]
        public float maxSpeed = 25f;
        
        // Serialized private fields
        [SerializeField] 
        private Transform wheelModel;
        
        // Private fields
        private float currentSpeed;
        private Rigidbody rb;
        
        // Unity lifecycle methods
        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }
        
        private void Start() {
            Initialize();
        }
        
        private void Update() {
            HandleInput();
        }
        
        private void FixedUpdate() {
            ApplyPhysics();
        }
        
        // Public methods
        public void Reset() {
            currentSpeed = 0;
        }
        
        // Private methods
        private void Initialize() {
            // Implementation
        }
        
        private void HandleInput() {
            // Implementation
        }
        
        private void ApplyPhysics() {
            // Implementation
        }
    }
}
```

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
- Only create interfaces when you expect multiple implementations (YAGNI principle)
- Avoid unnecessary interfaces that add maintenance burden
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
- Place opening braces on the same line as the function signature, if/for/while statements, etc. (e.g., `if (x) { ... }`)
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
- Use inheritance and interfaces to allow behavior extension - but only when needed (YAGNI)

### Liskov Substitution Principle
- Derived classes must be substitutable for their base classes
- Ensure override methods maintain the base class's behavior contract

### Interface Segregation Principle
- Keep interfaces focused and minimal
- Only create interfaces when you expect multiple implementations (YAGNI principle)
- Avoid unnecessary interfaces that add maintenance burden
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
