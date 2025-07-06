# Infrastructure Layer Test Coverage Improvement Plan

## Overview
This plan focuses on achieving meaningful test coverage for the Infrastructure layer by implementing **integration-critical tests only**. The emphasis is on testing external system integration, Godot-specific implementations, and critical service adapters, NOT exhaustive validation testing or framework behavior.

## ⚠️ **CRITICAL: AVOID OVER-TESTING**
Learn from Domain layer mistakes (577 tests, 4,856 lines):
- ❌ **Every configuration parameter** (file paths, JSON parsing details)
- ❌ **Framework behavior** (Godot engine internals, .NET I/O mechanics)
- ❌ **Trivial edge cases** (file format validation, resource loading)
- ❌ **Implementation details** (singleton patterns, adapter mechanics)

## ✅ **CORRECT TESTING APPROACH**
**Focus on integration value:**
- ✅ **External system integration** (file loading, Godot services)
- ✅ **Critical service adapters** (domain-to-infrastructure translation)
- ✅ **Essential workflows** (game initialization, resource management)
- ✅ **Error scenarios** (missing files, failed initialization)

**Test quantities should be:**
- **Service Adapters**: 2-3 tests max (translation + error handling)
- **External Integration**: 3-4 tests max (success path + failure scenarios)
- **Configuration Loading**: 2-3 tests max (valid config + error cases)
- **Total Target**: ~60-90 meaningful tests, not 150+

## Execution Instructions
**Process**: Execute phases one at a time. When a phase is complete:
1. Update this plan file to mark completed items
2. Run `dotnet clean; dotnet build; dotnet test tests/Infrastructure`
3. If all pass, proceed to next phase
4. If warnings or errors occur, fix them before proceeding
5. Continue until all phases are complete

---

## Current Status
**Phase 1**: ✅ Core Service Integration Testing - COMPLETED
**Phase 2**: ✅ Configuration and Data Loading Testing - COMPLETED  
**Phase 3**: ✅ Spatial and Map Services Testing - COMPLETED
**Phase 4**: ✅ Critical Integration Workflows - COMPLETED

**Test Coverage**: 7 test suites implemented across 4 completed phases
**Test Runner**: `tests/Infrastructure/TestRunner.cs` orchestrates all test execution
**Target Areas**: Focus on external integrations, Godot adapters, and file operations

---

## Phase 1: Core Service Integration Testing ✅ **COMPLETED**
**Focus**: Test essential service adapters and external integrations

- [x] **GameService Tests** (`tests/Infrastructure/Game/Services/GameServiceTests.cs`)
  - Game state initialization success
  - Money management core functionality
  - Critical error scenarios

- [x] **SoundManagerService Tests** (`tests/Infrastructure/Audio/Services/SoundManagerServiceTests.cs`)
  - Sound loading and playback
  - Volume control functionality
  - Error handling for missing sounds

**Success Criteria**: ✅ Core services integrate correctly with external systems

---

## Phase 2: Configuration and Data Loading Testing ✅ **COMPLETED**
**Focus**: Test critical configuration loading and data access

- [x] **StatsManagerService Tests** (`tests/Infrastructure/Stats/Services/StatsManagerServiceTests.cs`)
  - JSON configuration loading success
  - Error handling for malformed data
  - Fallback behavior when files missing

- [x] **WaveConfigurationService Tests** (`tests/Infrastructure/Waves/Services/WaveConfigurationServiceTests.cs`)
  - Wave data loading from JSON
  - Essential wave validation

**Success Criteria**: ✅ Configuration data loads reliably with proper error handling

---

## Phase 3: Spatial and Map Services Testing ✅ **COMPLETED**
**Focus**: Test critical spatial calculations and map integration

- [x] **MapBoundaryService Tests** (`tests/Infrastructure/Map/Services/MapBoundaryServiceTests.cs`)
  - Position validation for building placement
  - Boundary checking functionality
  - Map bounds calculation and validation
  - Abyss buffer zone detection
  - Position clamping enforcement
  - Empty map handling

- [x] **BuildingZoneService Tests** (`tests/Infrastructure/Buildings/Services/BuildingZoneServiceTests.cs`)
  - Building placement validation
  - Zone service adapter functionality
  - Wave path detection
  - Building validation consistency
  - Edge case handling

**Success Criteria**: ✅ Spatial validation prevents invalid placements reliably

---

## Phase 4: Critical Integration Workflows ✅ **COMPLETED**
**Focus**: Test key infrastructure workflows end-to-end

- [x] **Service Integration Tests** (`tests/Infrastructure/ServiceIntegrationTests.cs`)
  - DI container initialization workflow
  - Configuration loading chain with fallbacks
  - Service resolution and dependency injection
  - Error propagation and recovery mechanisms
  - Startup validation workflow integration
  - Critical service dependency chains

**Success Criteria**: ✅ Critical infrastructure workflows function reliably

---

## 🎧 **INFRASTRUCTURE TESTING GUIDELINES: DO THIS, NOT THAT**

### ✅ **DO: Focus on Integration Points**
```csharp
// GOOD: Tests external system integration
[Fact]
public void SoundService_LoadSound_ShouldIntegrateWithGodotAudio()
{
    var result = soundService.LoadSound("test_sound.ogg");
    result.IsSuccess.Should().BeTrue();
    mockAudioPlayer.Verify(x => x.LoadAudioStream(It.IsAny<string>()));
}

// GOOD: Tests configuration loading
[Fact] 
public void StatsManager_LoadConfig_ShouldHandleMissingFiles()
{
    var result = statsManager.LoadConfiguration("missing_file.json");
    result.IsSuccess.Should().BeFalse();
    result.Should().UseFallbackConfiguration();
}
```

### ❌ **DON'T: Test Framework Internals**
```csharp
// BAD: Testing Godot engine behavior
[Fact]
public void GodotNode_OnReady_ShouldCallBaseMethod() { } // DON'T DO THIS

// BAD: Testing every file format detail
[Fact]
public void JsonLoader_WithInvalidJson_ShouldThrow() { } // DON'T DO THIS
[Fact]
public void JsonLoader_WithMissingProperty_ShouldThrow() { } // DON'T DO THIS
```

### 🏆 **TARGET EXAMPLES**

**Service Adapter (2-3 tests max):**
- Domain-to-infrastructure translation
- Error handling for external failures

**External Integration (3-4 tests max):**
- Success path with valid data
- Key failure scenarios
- Recovery mechanisms

**Configuration Loading (2-3 tests max):**
- Valid configuration loading
- Fallback when files missing

---

## Expected Outcomes

1. **Reliable External Integration**: Well-tested integration with Godot and file systems
2. **Configuration Resilience**: Robust configuration loading with proper fallbacks
3. **Service Adapter Reliability**: Consistent domain-to-infrastructure translation
4. **Error Recovery**: Graceful handling of external system failures

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

1. ✅ **Reliable External Integration**: Well-tested integration with Godot engine and file system
2. ✅ **Robust Error Handling**: Comprehensive error handling for all external dependencies
3. ✅ **Configuration Reliability**: Validated configuration loading and parsing
4. ✅ **Audio System Confidence**: Tested audio pipeline with proper performance characteristics
5. ✅ **Spatial Accuracy**: Reliable map and building zone validation
6. ✅ **Integration Workflows**: End-to-end workflow testing completed

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
