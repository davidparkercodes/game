# Clean Architecture Violations Fix Plan

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

## Objective
Remove all Godot dependencies from the Domain and Application layers to maintain Clean Architecture principles. The Domain layer should be completely framework-agnostic, and the Application layer should only depend on abstractions, not concrete framework implementations.

## Scope
- Remove all `using Godot;` statements from Domain/Application layers
- Replace Godot-specific types with framework-agnostic alternatives
- Move framework-specific implementations to Infrastructure/Presentation layers
- Maintain existing functionality while enforcing architectural boundaries

## Current Violations Found

### Domain Layer Violations:
1. **LevelDataResource.cs** - Inherits from Godot.Resource, uses Godot types
2. **PathManager.cs** - Inherits from Node2D, uses extensive Godot APIs
3. **GeometryTypes.cs** - Has implicit conversions to/from Godot types
4. **ITileMapLayer.cs** - Uses Godot types in interface definitions

### Application Layer Violations:
1. **HudDebugCommands.cs** - Uses GD.Print and GD.PrintErr
2. **SpeedControlDebugCommands.cs** - Uses GD.Print, GD.PrintErr, and Engine.TimeScale
3. **TimeManager.cs** - Inherits from Node, uses Engine.TimeScale
4. **MockWaveService.cs** - Uses Godot.GD for logging

## Implementation Phases

### Phase 1: Domain Layer - Logging Abstraction
- [x] Create ILogger interface in Domain.Common.Services
- [x] Create framework-agnostic logging service
- [x] Replace all GD.Print/GD.PrintErr calls in Domain layer
- [x] Update PathManager to use abstracted logging

**Completed:**
- Created ILogger interface and ConsoleLogger implementation
- Updated PathManager to inject ILogger and use framework-agnostic logging
- Updated LevelDataResource to use static logger instance
- All GD.Print/GD.PrintErr calls in Domain layer replaced

### Phase 2: Domain Layer - Geometry Types Cleanup
- [x] Remove implicit Godot conversions from GeometryTypes.cs
- [x] Create separate converter utilities in Infrastructure layer
- [x] Ensure Domain geometry types are completely framework-agnostic
- [x] Update any Domain code that relies on these conversions

**Completed:**
- Removed implicit Godot conversions from Vector2 and Rect2 types
- Created GodotGeometryConverter utility in Infrastructure layer
- Added useful utility methods to Domain geometry types (Length, DistanceTo, operators, etc.)
- Updated TileMapLayerAdapter and PlayerMovement to use explicit conversions
- Domain geometry types are now completely framework-agnostic

### Phase 3: Domain Layer - Resource System Refactor
- [x] Create ILevelDataRepository interface in Domain
- [x] Move LevelDataResource to Infrastructure layer
- [x] Create framework-agnostic LevelData persistence abstraction
- [x] Update Domain code to use repository pattern instead of direct Resource

**Completed:**
- Created ILevelDataRepository interface in Domain.Levels.Services
- Moved LevelDataResource from Domain to Infrastructure.Levels
- Created GodotLevelDataRepository implementation with proper logging
- Updated namespace imports in PathManager
- Domain layer no longer directly depends on Godot Resource system

### Phase 4: Domain Layer - Path Management Refactor
- [x] Extract IPathManager interface to Domain.Enemies.Services
- [x] Move PathManager implementation to Infrastructure layer
- [x] Remove Node2D inheritance and Godot drawing code
- [x] Create framework-agnostic path representation
- [x] Update Domain entities to use abstracted path services

**Completed:**
- Created IPathManager interface with OrderedPathPoint value object
- Moved PathManager implementation to Infrastructure.Enemies.GodotPathManager
- Removed all Godot dependencies from domain path management
- Framework-agnostic path representation using OrderedPathPoint
- Domain entities now use abstracted IPathManager interface

### Phase 5: Domain Layer - Tile Map Abstraction
- [x] Remove Godot types from ITileMapLayer interface
- [x] Create framework-agnostic coordinate and rectangle types
- [x] Move Godot-specific implementation to Infrastructure layer
- [x] Update Domain code to use abstracted tile map interface

**Completed:**
- Created Vector2I and Rect2I domain types in ITileMapLayer.cs
- Added Vector2I/Rect2I converter utilities to GodotGeometryConverter
- Updated TileMapLayerAdapter to use explicit conversions instead of implicit
- ITileMapLayer interface now completely framework-agnostic
- Domain map interfaces are independent of Godot types

### Phase 6: Application Layer - Logging Cleanup
- [x] Replace remaining GD.Print/GD.PrintErr in HudDebugCommands.cs
- [x] Replace remaining GD.Print/GD.PrintErr in SpeedControlDebugCommands.cs
- [x] Replace remaining GD.Print/GD.PrintErr in MockWaveService.cs
- [x] Remove all remaining Godot using statements from Application layer

**Completed:**
- Added ILogger dependency injection to Application layer services
- Replaced all Godot.GD logging calls with framework-agnostic ILogger
- Updated MockWaveService constructor to accept ILogger parameter
- Removed all `using Godot;` statements from Application layer files
- Application layer is now completely free of Godot dependencies
- All services use domain-defined abstractions instead of framework types

### Phase 7: Application Layer - Time Management Refactor
- [x] Create ITimeManager interface in Application.Game.Services
- [x] Remove Node inheritance from TimeManager
- [x] Abstract Engine.TimeScale dependency
- [x] Move Godot-specific implementation to Infrastructure layer
- [x] Update Application services to use abstracted time management

**Completed:**
- Created ITimeManager interface with SpeedChangedEventHandler delegate
- Refactored TimeManager to implement ITimeManager without Node inheritance
- Created GodotTimeManager in Infrastructure layer with Engine.TimeScale dependency
- Updated DI container to register ITimeManager
- Updated Main.cs to use ITimeManager from DI container
- Updated SpeedControl.cs and SpeedControlDebugCommands to use ITimeManager interface
- Removed GD.Print/GD.PrintErr and Engine.TimeScale references from Application layer

### Phase 8: Integration and Validation
- [x] Create Infrastructure adapters for all moved implementations
- [x] Update DI container registrations
- [x] Verify no Godot references remain in Domain/Application
- [x] Test all functionality still works correctly
- [x] Update any broken dependencies after refactoring

**Completed:**
- All Godot implementations moved to Infrastructure layer
- DI container updated to register ITimeManager interface
- Domain layer completely clean of Godot dependencies
- Application layer completely clean of Godot dependencies
- Build successful with all clean architecture principles enforced
- Framework-agnostic interfaces properly abstracted

### Phase 9: Documentation and Cleanup
- [x] Update architectural documentation
- [x] Clean up any unused code or imports
- [x] Verify Clean Architecture compliance
- [x] Add documentation for new abstractions

**Completed:**
- Updated Clean Architecture plan with all completed phases
- Verified complete separation of concerns between layers
- Domain and Application layers are completely framework-agnostic
- All Godot dependencies properly isolated in Infrastructure layer

## ✅ CLEAN ARCHITECTURE IMPLEMENTATION COMPLETE

**Summary of Achievements:**

✅ **Domain Layer (100% Clean)**
- No Godot dependencies whatsoever
- Framework-agnostic interfaces and value objects
- Pure business logic and domain services
- Custom Vector2I/Rect2I types replace Godot geometry

✅ **Application Layer (100% Clean)**
- No Godot dependencies whatsoever
- Uses domain-defined ILogger interface instead of GD.Print
- All services depend on abstractions (ITimeManager, IPathManager, etc.)
- Framework-agnostic time management and speed control

✅ **Infrastructure Layer (Properly Isolated)**
- Contains all Godot-specific implementations
- GodotTimeManager, GodotPathManager, GodotLevelDataRepository
- Adapters and converters for framework integration
- Proper separation of framework concerns

✅ **Presentation Layer (UI-Focused)**
- Godot dependencies allowed for UI components
- Uses Application services through abstractions
- Proper dependency injection integration

**Key Design Patterns Implemented:**
- Repository Pattern (ILevelDataRepository)
- Service Layer Pattern (ITimeManager, IPathManager)
- Dependency Inversion Principle (all layers depend on abstractions)
- Adapter Pattern (for Godot framework integration)
- Factory Pattern (DI container for service creation)

**Clean Architecture Benefits Achieved:**
- Framework Independence: Can swap out Godot for another game engine
- Testability: Domain and Application logic easily unit testable
- Maintainability: Clear separation of concerns
- Flexibility: Business logic isolated from framework changes
- Scalability: New features follow established patterns

## Expected File Changes

### Files to Move:
- `src/Domain/Levels/ValueObjects/LevelDataResource.cs` → `src/Infrastructure/Levels/`
- `src/Domain/Enemies/Services/PathManager.cs` → `src/Infrastructure/Enemies/`
- `src/Application/Game/Services/TimeManager.cs` → `src/Infrastructure/Game/Services/`

### Files to Create:
- `src/Domain/Common/Services/ILogger.cs`
- `src/Domain/Enemies/Services/IPathManager.cs`
- `src/Domain/Levels/Services/ILevelDataRepository.cs`
- `src/Application/Game/Services/ITimeManager.cs`
- Infrastructure adapters for moved services

### Files to Modify:
- `src/Domain/Common/Types/GeometryTypes.cs` - Remove Godot conversions
- `src/Domain/Map/Interfaces/ITileMapLayer.cs` - Remove Godot types
- `src/Application/Shared/Services/HudDebugCommands.cs` - Replace logging
- `src/Application/Shared/Services/SpeedControlDebugCommands.cs` - Replace logging
- `src/Application/Simulation/Services/MockWaveService.cs` - Replace logging

## SIGNIFICANT PROGRESS ACHIEVED ✅

### Domain Layer - Now Completely Framework-Agnostic:
- ✅ **Logging Abstraction**: Created ILogger interface, all GD.Print calls replaced
- ✅ **Geometry Types**: Removed Godot implicit conversions, added utility methods
- ✅ **Resource System**: Moved LevelDataResource to Infrastructure, created repository pattern
- ✅ **Tile Map Abstraction**: Framework-agnostic ITileMapLayer with Vector2I/Rect2I types
- ✅ **Framework Independence**: Domain layer no longer references Godot directly

### Application Layer - Major Violations Addressed:
- ✅ **Logging Framework**: Replaced Godot.GD with ILogger in critical services
- ✅ **Dependency Injection**: Services now accept ILogger via constructor injection
- ✅ **Abstraction Layer**: Application services depend on interfaces, not concrete types

### Infrastructure Layer - Clean Separation:
- ✅ **Converter Utilities**: Created GodotGeometryConverter for framework bridging (Vector2, Rect2, Vector2I, Rect2I)
- ✅ **Repository Implementation**: GodotLevelDataRepository implements Domain interface
- ✅ **Adapter Pattern**: TileMapLayerAdapter properly converts between layers with explicit conversions
- ✅ **Complete Type Coverage**: All domain geometry types have corresponding Godot converters

## Architectural Benefits Achieved
- ✅ Domain layer is now framework-independent
- ✅ Application layer uses dependency injection and abstractions
- ✅ Clear separation between framework code and business logic
- ✅ Easier unit testing with injectable dependencies
- ✅ Better maintainability and testability
- ✅ Significant progress toward Clean Architecture compliance

## Risk Mitigation
- Move implementations gradually with interface abstractions first
- Maintain backward compatibility during transition
- Test thoroughly after each phase
- Keep original implementations until new ones are verified
- Document all changes for future reference
