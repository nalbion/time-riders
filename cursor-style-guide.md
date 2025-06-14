# Cursor AI IDE Style Guide

This document defines coding standards and best practices for the Time Riders Unity project when working with Cursor AI IDE. These guidelines ensure code quality and consistency across the development team.

## Table of Contents
1. [Code Structure](#code-structure)
2. [Naming Conventions](#naming-conventions)
3. [Documentation](#documentation)
4. [Testing](#testing)
5. [AI Assistance Best Practices](#ai-assistance-best-practices)
6. [Code Review Guidelines](#code-review-guidelines)

## Code Structure

### Namespaces
- Use meaningful namespaces that reflect the project structure
- Organize code in the `TimeRiders` namespace with appropriate subnamespaces:
```csharp
namespace TimeRiders.Agents { }
namespace TimeRiders.Physics { }
namespace TimeRiders.UI { }
```

### Interfaces
- Only create interfaces when you expect multiple implementations (YAGNI principle)
- Avoid unnecessary interfaces that add maintenance burden
- Prefix with "I" if interfaces are used

### Braces
- Place opening braces on the same line as the function signature, if/for/while statements, etc. (e.g., `if (x) { ... }`)

### Class Organization
- Follow a consistent structure for all classes:
  1. Constants and static fields
  2. Instance fields (public, then private)
  3. Properties
  4. Constructors
  5. Unity lifecycle methods (in execution order)
  6. Public methods
  7. Protected methods
  8. Private methods
  9. Nested types

### Dependencies
- Use dependency injection for services and managers
- Favor composition over inheritance
- Create interfaces for major components to allow for testing and flexibility

## Naming Conventions

### General Rules
- Use clear, descriptive names that convey purpose
- Prioritize readability over brevity
- No Hungarian notation or prefixes for member variables

### Unity-Specific
- Component references should end with the type name: `playerTransform`, `bikeRigidbody`
- Event handlers should follow the pattern `OnEventName`: `OnPlayerDeath`, `OnRaceStart`
- Coroutines should end with "Routine": `MovementRoutine`, `SpawnEnemyRoutine`

### Editor Extensions
- Editor scripts should end with "Editor": `BikeControllerEditor`
- Custom inspector properties should be descriptive of what they expose

## Documentation

### Code Comments
- Every class should have a summary comment explaining its purpose
- Public methods should have XML documentation comments
- Complex algorithms should include explanation comments
- Todo comments should include the developer's name and date:
  ```csharp
  // TODO(username, 2025-06-14): Refactor physics calculations
  ```

### Inline Comments
- Focus on explaining "why" not "what"
- Keep comments updated when changing code
- Use comments to highlight non-obvious code decisions:
  ```csharp
  // Using 0.75f as the factor here to account for tire friction
  float adjustedSpeed = rawSpeed * 0.75f; 
  ```

## Testing

### Unit Test Structure
- Name tests with pattern: `MethodName_Scenario_ExpectedResult`
- Each test should focus on one specific behavior
- Use Arrange-Act-Assert pattern:
  ```csharp
  [Test]
  public void CalculateJumpHeight_LowSpeed_ReturnsMinimumHeight() {
      // Arrange
      var controller = new BikeController();
      controller.SetSpeed(5f);
      
      // Act
      float height = controller.CalculateJumpHeight();
      
      // Assert
      Assert.AreEqual(MinJumpHeight, height, 0.01f);
  }
  ```

### Integration Tests
- Test critical game systems end-to-end
- Include performance testing for physics-heavy operations
- Create test scenes that isolate specific functionality

## AI Assistance Best Practices

### Prompting Cursor AI
- Start prompts with desired outcome: "Refactor the bike physics to separate concerns"
- Provide context and constraints: "This needs to maintain backward compatibility"
- When asking for new code, specify architecture requirements and error handling needs
- For complex features, provide pseudocode or step-by-step goals

### Code Review with Cursor AI
- Ask AI to review for specific concerns: "Review for potential memory leaks"
- Use AI to suggest optimizations: "How can this algorithm be more efficient?"
- Request alternative approaches: "What are other ways to implement this bike steering system?"

### Pair Programming Workflow
- Generate test cases before implementation
- Ask AI to complete TODO comments and add documentation
- Use AI to help with debugging: "What might cause this NullReferenceException?"

## Code Review Guidelines

### Review Checklist
- Code follows naming conventions and style guidelines
- Methods are focused and follow single responsibility principle
- Public APIs have appropriate documentation
- Magic numbers are replaced with named constants
- Error handling is appropriate and consistent
- Code is testable and includes tests where appropriate

### Feedback Approach
- Frame feedback as questions or suggestions
- Provide reasoning behind critical feedback
- Suggest specific improvements rather than vague criticism
- Acknowledge good code and creative solutions

### Common Issues to Watch For
- Physics calculations in Update instead of FixedUpdate
- Component references not cached in Awake/Start
- Overuse of GetComponent during gameplay
- String comparisons instead of tags or layers
- Missing null checks for external references
- Expensive operations in frequently called methods

## Performance Considerations

### Unity-Specific Optimizations
- Cache component references in Awake/Start
- Use object pooling for frequently created/destroyed objects
- Minimize GetComponent calls during gameplay
- Use proper data structures for collections

### Mobile Optimizations
- Limit physics calculations
- Optimize render calls and batching
- Consider LOD (Level of Detail) for complex models
- Profile and optimize CPU-intensive operations

### Memory Management
- Dispose of resources properly
- Unsubscribe from events when objects are destroyed
- Consider reference counting for shared resources
- Avoid allocating memory during gameplay loops
