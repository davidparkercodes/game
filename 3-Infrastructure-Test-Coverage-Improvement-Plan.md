# Infrastructure Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving ~80% test coverage for the Infrastructure layer by implementing meaningful, integration-focused tests. The emphasis is on testing external system integration, Godot-specific implementations, file I/O operations, and service adapters rather than padding statistics with trivial tests.

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Infrastructure`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Existing Tests**: No Infrastructure tests currently exist
**Target Areas**: Focus on external integrations, Godot adapters, and file operations

---

## Phase 1: Core Game Services Testing
**Focus**: Test fundamental game state management and singleton services

- [ ] **GameService Tests** (`tests/Infrastructure/Game/Services/GameServiceTests.cs`)
  - Game state initialization and lifecycle
  - Money management (spending, adding, validation)
  - Lives tracking and game over conditions
  - Score calculation and tracking
  - HUD integration and state synchronization
  - Singleton pattern implementation

- [ ] **GodotTimeManager Tests** (`tests/Infrastructure/Game/GodotTimeManagerTests.cs`)
  - Time scale management
  - Godot engine integration
  - Frame rate and timing accuracy
  - Pause/resume functionality

**Success Criteria**: Core game state properly managed with reliable service integration

---

## Phase 2: Audio Infrastructure Testing
**Focus**: Test sound system integration and Godot audio pipeline

- [ ] **SoundService Tests** (`tests/Infrastructure/Sound/SoundServiceTests.cs`)
  - Sound loading and configuration
  - Audio player management and pooling
  - Volume control and categories (SFX, UI, Music)
  - Positional audio calculations
  - Godot AudioStreamPlayer integration
  - Sound format handling (OGG, WAV, MP3)
  - Error handling for missing sounds

- [ ] **SoundLoader Tests** (`tests/Infrastructure/Sound/SoundLoaderTests.cs`)
  - Sound file discovery and loading
  - Configuration parsing
  - Resource path resolution
  - Error handling for corrupted files

- [ ] **SoundServiceAdapter Tests** (`tests/Infrastructure/Sound/SoundServiceAdapterTests.cs`)
  - Domain-to-infrastructure translation
  - SoundRequest processing
  - Adapter pattern implementation

**Success Criteria**: Audio system reliably plays sounds with proper volume and positioning

---

## Phase 3: Stats and Configuration Management Testing
**Focus**: Test data loading and configuration systems

- [ ] **StatsManagerService Tests** (`tests/Infrastructure/Stats/Services/StatsManagerServiceTests.cs`)
  - JSON configuration loading
  - Enemy and building stats parsing
  - Fallback stats generation
  - File path resolution
  - Godot FileAccess integration
  - Error handling for malformed JSON
  - Singleton pattern and initialization

- [ ] **StatsService Tests** (`tests/Infrastructure/Stats/StatsServiceTests.cs`)
  - Stats provider interface implementation
  - Data access patterns
  - Caching behavior

- [ ] **StatsServiceAdapter Tests** (`tests/Infrastructure/Stats/StatsServiceAdapterTests.cs`)
  - Domain stats conversion
  - Adapter pattern implementation
  - Performance optimization

**Success Criteria**: Configuration data reliably loaded with proper error handling

---

## Phase 4: Wave and Enemy Management Testing
**Focus**: Test wave spawning and enemy lifecycle management

- [ ] **WaveManager Tests** (`tests/Infrastructure/Waves/Services/WaveManagerTests.cs`)
  - Wave progression logic
  - Enemy spawning coordination
  - Wave completion detection
  - Boss wave special handling
  - State synchronization with game services
  - Debug wave jumping functionality
  - UI integration and notifications

- [ ] **WaveConfigurationService Tests** (`tests/Infrastructure/Waves/Services/WaveConfigurationServiceTests.cs`)
  - Wave data loading from JSON
  - Wave validation and parsing
  - Enemy group configuration
  - Timing and delay management

- [ ] **WaveSpawnerService Tests** (`tests/Infrastructure/Enemies/Services/WaveSpawnerServiceTests.cs`)
  - Enemy spawning mechanics
  - Spawn timing and intervals
  - Enemy positioning and path assignment
  - Spawn queue management

- [ ] **WaveModel Tests** (`tests/Infrastructure/Waves/Models/WaveModelTests.cs`)
  - Wave data structure validation
  - Enemy group organization
  - Serialization and deserialization

**Success Criteria**: Wave system spawns enemies correctly with proper timing and progression

---

## Phase 5: Map and Spatial Services Testing
**Focus**: Test map boundaries, building zones, and spatial calculations

- [ ] **MapBoundaryService Tests** (`tests/Infrastructure/Map/Services/MapBoundaryServiceTests.cs`)
  - Map bounds calculation from tile data
  - Position validation (walking, building)
  - Abyss buffer zone management
  - Tile-based collision detection
  - World-to-tile coordinate conversion
  - Boundary clamping logic

- [ ] **BuildingZoneService Tests** (`tests/Infrastructure/Buildings/Services/BuildingZoneServiceTests.cs`)
  - Building placement validation
  - Path detection and avoidance
  - Zone service adapter implementation
  - Position coordinate conversion

- [ ] **BuildingZoneValidator Tests** (`tests/Infrastructure/Buildings/Validators/BuildingZoneValidatorTests.cs`)
  - Godot-specific validation logic
  - Tile map integration
  - Static validation methods

**Success Criteria**: Spatial validation prevents invalid building placement and ensures proper pathfinding

---

## Phase 6: Level Data and Resource Management Testing
**Focus**: Test level loading and Godot resource integration

- [ ] **LevelDataResource Tests** (`tests/Infrastructure/Levels/LevelDataResourceTests.cs`)
  - Godot Resource serialization
  - Domain-to-Godot data conversion
  - Path point validation and conversion
  - Export attribute handling
  - Resource loading and saving

- [ ] **GodotLevelDataRepository Tests** (`tests/Infrastructure/Levels/GodotLevelDataRepositoryTests.cs`)
  - Level data repository implementation
  - File system integration
  - Resource path resolution
  - Error handling for missing files

**Success Criteria**: Level data loads correctly from Godot resources with proper validation

---

## Phase 7: Geometry and Adapter Testing
**Focus**: Test coordinate conversion and adapter patterns

- [ ] **GodotGeometryConverter Tests** (`tests/Infrastructure/Common/Converters/GodotGeometryConverterTests.cs`)
  - Vector2 and Rect2 conversion accuracy
  - Domain-to-Godot coordinate mapping
  - Precision handling for floating-point values
  - Bidirectional conversion consistency

- [ ] **TileMapLayerAdapter Tests** (`tests/Infrastructure/Map/Adapters/TileMapLayerAdapterTests.cs`)
  - Godot TileMap integration
  - Tile coordinate conversion
  - Tile data extraction
  - Layer management

- [ ] **Service Adapter Pattern Tests** (`tests/Infrastructure/Integration/ServiceAdapterTests.cs`)
  - Adapter pattern consistency
  - Domain-to-infrastructure boundary testing
  - Interface compliance validation

**Success Criteria**: Geometry conversion and adapters provide accurate coordinate mapping

---

## Phase 8: Godot Engine Integration Testing
**Focus**: Test Godot-specific integrations and engine dependencies

- [ ] **Godot Runtime Detection Tests** (`tests/Infrastructure/Common/GodotRuntimeTests.cs`)
  - Engine runtime detection accuracy
  - Editor vs. runtime differentiation
  - Fallback behavior for non-Godot environments

- [ ] **Scene Tree Integration Tests** (`tests/Infrastructure/Integration/SceneTreeTests.cs`)
  - Node management and lifecycle
  - Scene tree traversal
  - Group-based node queries
  - Child node addition and removal

- [ ] **Resource Loading Tests** (`tests/Infrastructure/Integration/ResourceLoadingTests.cs`)
  - Godot resource loading patterns
  - Path resolution strategies
  - Resource caching behavior
  - Error handling for missing resources

**Success Criteria**: Godot engine integration works reliably with proper error handling

---

## Phase 9: Integration and System Testing
**Focus**: Test complex interactions between infrastructure components

- [ ] **Service Integration Tests** (`tests/Infrastructure/Integration/ServiceIntegrationTests.cs`)
  - Cross-service communication
  - Event propagation and handling
  - State synchronization
  - Service lifecycle management

- [ ] **File I/O Integration Tests** (`tests/Infrastructure/Integration/FileIOTests.cs`)
  - Configuration file loading
  - Resource path resolution
  - File system error handling
  - Platform-specific file access

- [ ] **Performance Tests** (`tests/Infrastructure/Performance/InfrastructurePerformanceTests.cs`)
  - Audio system performance
  - Resource loading benchmarks
  - Memory usage patterns
  - Garbage collection impact

**Success Criteria**: Infrastructure components work together reliably under various conditions

---

## Phase 10: Coverage Analysis and Optimization
**Focus**: Measure and optimize test coverage

- [ ] **Coverage Assessment**
  - Run coverage analysis: `dotnet test --collect:"XPlat Code Coverage"`
  - Generate coverage reports
  - Identify gaps in critical infrastructure logic

- [ ] **Targeted Coverage Improvements**
  - Add tests for uncovered integration paths
  - Focus on error handling and edge cases
  - Test platform-specific code paths

- [ ] **Test Quality Review**
  - Ensure tests validate infrastructure behavior, not implementation details
  - Verify meaningful test names and descriptions
  - Remove any padding tests that don't add value

**Success Criteria**: ~80% coverage with meaningful, maintainable tests

---

## Testing Principles Applied

### Integration Focus
- Test actual Godot engine integration, not mocked behavior
- Verify file I/O operations with real file system
- Test external system boundaries and adapters

### Error Handling Emphasis
- Test all failure scenarios (missing files, corrupted data, etc.)
- Verify graceful degradation and fallback behavior
- Test exception handling and error propagation

### Platform Awareness
- Test Godot-specific features and limitations
- Verify cross-platform compatibility where applicable
- Test editor vs. runtime behavior differences

### Performance Considerations
- Test resource loading performance
- Verify memory usage patterns
- Test audio system latency and quality

---

## Expected Outcomes

1. **Reliable External Integration**: Well-tested integration with Godot engine and file system
2. **Robust Error Handling**: Comprehensive error handling for all external dependencies
3. **Configuration Reliability**: Validated configuration loading and parsing
4. **Audio System Confidence**: Tested audio pipeline with proper performance characteristics
5. **Spatial Accuracy**: Reliable map and building zone validation

---

## Notes
- Focus on testing actual Godot integration, not mocked behavior where possible
- Use TestContainers or similar for file system testing
- Mock only external dependencies that cannot be controlled (e.g., network)
- Test both success and failure paths for all file operations
- Verify Godot-specific behavior like scene tree management
- Test singleton patterns and static service access
- Each phase should be completable independently
- Consider platform-specific testing for file path handling
