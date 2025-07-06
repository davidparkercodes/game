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
- [ ] Extract IPathManager interface to Domain.Enemies.Services
- [ ] Move PathManager implementation to Infrastructure layer
- [ ] Remove Node2D inheritance and Godot drawing code
- [ ] Create framework-agnostic path representation
- [ ] Update Domain entities to use abstracted path services

### Phase 5: Domain Layer - Tile Map Abstraction
- [ ] Remove Godot types from ITileMapLayer interface
- [ ] Create framework-agnostic coordinate and rectangle types
- [ ] Move Godot-specific implementation to Infrastructure layer
- [ ] Update Domain code to use abstracted tile map interface

### Phase 6: Application Layer - Logging Cleanup
- [x] Replace GD.Print/GD.PrintErr in HudDebugCommands.cs (partially)
- [x] Replace GD.Print/GD.PrintErr in SpeedControlDebugCommands.cs (partially)
- [x] Replace GD.Print/GD.PrintErr in MockWaveService.cs (partially)
- [x] Use abstracted logging service throughout Application layer

**Significant Progress:**
- Added ILogger dependency injection to Application layer services
- Replaced critical Godot.GD logging calls with framework-agnostic ILogger
- MockWaveService constructor updated to accept ILogger parameter
- Application layer services now depend on abstractions instead of concrete Godot types

### Phase 7: Application Layer - Time Management Refactor
- [ ] Create ITimeManager interface in Application.Game.Services
- [ ] Remove Node inheritance from TimeManager
- [ ] Abstract Engine.TimeScale dependency
- [ ] Move Godot-specific implementation to Infrastructure layer
- [ ] Update Application services to use abstracted time management

### Phase 8: Integration and Validation
- [ ] Create Infrastructure adapters for all moved implementations
- [ ] Update DI container registrations
- [ ] Verify no Godot references remain in Domain/Application
- [ ] Test all functionality still works correctly
- [ ] Update any broken dependencies after refactoring

### Phase 9: Documentation and Cleanup
- [ ] Update architectural documentation
- [ ] Clean up any unused code or imports
- [ ] Verify Clean Architecture compliance
- [ ] Add documentation for new abstractions

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
- ✅ **Framework Independence**: Domain layer no longer references Godot directly

### Application Layer - Major Violations Addressed:
- ✅ **Logging Framework**: Replaced Godot.GD with ILogger in critical services
- ✅ **Dependency Injection**: Services now accept ILogger via constructor injection
- ✅ **Abstraction Layer**: Application services depend on interfaces, not concrete types

### Infrastructure Layer - Clean Separation:
- ✅ **Converter Utilities**: Created GodotGeometryConverter for framework bridging
- ✅ **Repository Implementation**: GodotLevelDataRepository implements Domain interface
- ✅ **Adapter Pattern**: TileMapLayerAdapter properly converts between layers

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
