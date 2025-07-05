# Domain Layer

This directory contains the **pure business logic** of the game, with **zero external dependencies**.

## Structure

```
Domain/
├── Entities/          # Core business objects (BuildingEntity, EnemyEntity, etc.)
├── ValueObjects/      # Data containers (BuildingStats, EnemyStats, etc.)
└── Services/          # Pure business rules (BuildingZoneValidator, PathingService)
```

## Principles

- **No Godot dependencies** - Must be pure C# that can run anywhere
- **No external libraries** - Only .NET standard library dependencies
- **Pure functions** - Predictable, testable business logic
- **Immutable where possible** - Value objects should be immutable
- **High test coverage** - Aim for 90%+ unit test coverage

## Testing

- Unit tests are located in `tests/Domain/`
- All Domain logic should be testable without mocking
- Use dependency injection interfaces for any external dependencies

## Current Status

**Phase 1.1 Complete** ✅
- [x] Directory structure created
- [x] Testing framework packages added
- [x] Basic test infrastructure setup

**Note**: Testing framework is configured but may need .NET runtime version adjustments for execution.
