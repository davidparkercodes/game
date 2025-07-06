# Wave System Overhaul Plan

## Overview
This plan addresses critical issues in the Wave system spanning Domain, Application, Infrastructure, and Presentation layers. The main error indicates that WaveConfigService.cs cannot be instantiated due to class/filename mismatch issues. Additionally, we need to establish consistent naming conventions and ensure proper data-driven configuration.

## Key Issues Identified
1. **Critical Error**: `WaveConfigService.cs` cannot be instantiated - class/filename mismatch
2. **Inconsistent Naming**: Mixed use of "Config" and "Configuration" throughout the codebase
3. **Hardcoded Wave Data**: Wave stats are generated in code rather than loaded from data files
4. **GameSimRunner Integration**: Wave system needs proper integration with simulation tools
5. **Service Dependencies**: Complex circular dependencies between wave services
6. **Data Layer Misalignment**: Existing JSON data not properly integrated with services

---

## Phase 1: Fix Critical Runtime Error
**Priority: CRITICAL** 

### [✅] 1.1 Investigate and Fix WaveConfigService Instantiation
- [✅] Verify class name in `WaveConfigService.cs` matches filename exactly
- [✅] Check namespace declarations and ensure proper casing
- [✅] Verify the class is public and properly accessible
- [✅] Test instantiation in Godot environment
- [✅] Remove from Main.tscn if necessary and re-add after fixing

### [✅] 1.2 Validate All Wave-Related Class Names
- [✅] Check `WaveSpawnerService.cs` class definition
- [✅] Verify `WaveConfigurationInternal.cs` class definition (to be renamed)
- [✅] Ensure `EnemySpawnGroup.cs` class definition is correct
- [✅] Test all classes can be instantiated properly
- [✅] Plan for upcoming restructure to Models/ subdirectory

### [✅] 1.3 Fix DI Registration Issues
- [✅] Review `DiConfiguration.cs` for proper Wave service registration
- [✅] Ensure all interfaces are properly bound to implementations
- [✅] Verify singleton/transient lifetimes are appropriate

---

## Phase 2: Establish Naming Consistency 
**Priority: HIGH**

### [✅] 2.1 Standardize on "Configuration" (Drop "Config")
- [✅] Rename `IWaveConfigService` → `IWaveConfigurationService`
- [✅] Rename `WaveConfigService` → `WaveConfigurationService`  
- [✅] Update all references in commands, handlers, and DI
- [✅] Update file names and ensure class names match

### [✅] 2.2 Restructure Infrastructure Files and Classes
- [✅] Create `src/Infrastructure/Waves/Models/` directory
- [✅] Move `WaveConfigurationInternal.cs` → `src/Infrastructure/Waves/Models/WaveModel.cs`
- [✅] Move `WaveSetConfigurationInternal.cs` → `src/Infrastructure/Waves/Models/WaveSetModel.cs`
- [✅] Move `EnemySpawnGroup.cs` → `src/Infrastructure/Waves/Models/EnemySpawnGroup.cs`
- [✅] Rename class `WaveConfigurationInternal` → `WaveModel`
- [✅] Rename class `WaveSetConfigurationInternal` → `WaveSetModel`
- [✅] Keep `WaveConfiguration` (Domain value object) unchanged
- [✅] Update all references throughout the codebase

### [✅] 2.3 Organize Wave Infrastructure Services
- [✅] Move `WaveConfigService.cs` → `src/Infrastructure/Waves/Services/WaveConfigurationService.cs`
- [✅] Keep `WaveSpawnerService.cs` in `src/Infrastructure/Enemies/Services/` (enemy-focused)
- [✅] Ensure no files remain directly in `src/Infrastructure/Waves/`
- [✅] Update all namespace references

### [✅] 2.4 Update Method and Property Names
- [✅] Review all method signatures for consistency
- [✅] Update variable names and parameters
- [✅] Ensure XML documentation uses consistent terminology
- [✅] Update any remaining "Config" references to "Configuration"

---

## Phase 3: Implement Data-Driven Wave System
**Priority: HIGH**

### [✅] 3.1 Enhance Data Loading Infrastructure  
- [✅] Verify `default_waves.json` structure matches `WaveModel` and `WaveSetModel`
- [✅] Add validation for JSON schema compliance
- [✅] Implement fallback mechanisms for missing data
- [✅] Add logging for data loading success/failure
- [✅] Update JSON parsing to use new model classes

### [✅] 3.2 Remove Hardcoded Wave Generation
- [✅] Remove `CreateWaveConfiguration()` method from `WaveSpawnerService`
- [✅] Ensure all wave data comes from `data/waves/` directory
- [✅] Update `WaveSpawnerService` to use `IWaveConfigurationService`
- [✅] Remove hardcoded enemy counts, spawn intervals, etc.

### [✅] 3.3 Extend Data Files for Comprehensive Coverage
- [✅] Add more wave configurations beyond the current 5 waves
- [✅] Create wave sets for different difficulty levels
- [✅] Add metadata for wave progression rules
- [✅] Create validation schemas for wave JSON files

### [✅] 3.4 Implement Dynamic Wave Loading
- [✅] Add support for loading different wave sets
- [✅] Implement wave set switching at runtime
- [✅] Add support for custom/modded wave configurations
- [✅] Cache loaded wave data for performance

---

## Phase 4: Fix Service Architecture
**Priority: MEDIUM**

### [✅] 4.1 Resolve Service Dependencies
- [✅] Review circular dependencies between wave services
- [✅] Implement proper dependency injection hierarchy
- [✅] Extract common interfaces where needed
- [✅] Ensure clean separation of concerns

### [✅] 4.2 Unify Wave Management
- [✅] Consolidate `WaveSpawnerService` and `IWaveService` responsibilities
- [✅] Ensure `WaveSpawnerService` implements proper interface
- [✅] Remove static singleton patterns where appropriate
- [✅] Implement proper lifecycle management

### [✅] 4.3 Improve Error Handling
- [✅] Add comprehensive error handling for wave loading
- [✅] Implement graceful degradation when data is missing
- [✅] Add detailed logging for debugging
- [✅] Ensure exceptions don't crash the game

---

## Phase 5: GameSimRunner Integration
**Priority: MEDIUM**

### Overview
Integrate the wave system with the existing GameSimRunner simulation framework to ensure unified wave configuration and testing capabilities across both in-game (Godot) and simulation environments.

### [✅] 5.1 Define Interfaces and Service Alignment
- [✅] Ensure wave system interfaces align with `GameSimRunner` architecture
- [✅] Review and adapt wave service interfaces to work with mock services
- [✅] Verify `IWaveService` compatibility with simulation framework
- [✅] Define clear boundaries between Domain/Application layers for simulation use

### [✅] 5.2 Mock Services Adaptation
- [✅] Create or adapt `MockWaveService.cs` to align with current wave system
- [✅] Ensure wave configuration loading uses `data/simulation/wave-configs.json`
- [✅] Remove any hardcoded wave values from mock services
- [✅] Implement config-driven wave spawning in simulation environment
- [✅] Verify wave progression logic works in both Godot and simulation contexts

### [✅] 5.3 Configuration Alignment
- [✅] Ensure all wave-related configurations are present in simulation JSON files
- [✅] Align `default_waves.json` structure with simulation requirements
- [✅] Add wave-specific parameters to `SimulationConfig.cs` if needed
- [✅] Enable easy modification and testing without recompilation
- [✅] Create wave balance multipliers and testing scenarios

### [✅] 5.4 Unify Wave Logic Between Systems
- [✅] Integrate current in-game wave logic with `GameSimRunner` simulation code
- [✅] Ensure `SpawnEnemiesForWave` method works with unified wave configuration
- [✅] Adapt wave progression and difficulty scaling for simulation use
- [✅] Maintain consistency between Godot wave spawning and simulation wave spawning

### [✅] 5.5 Testing and Validation
- [✅] Write integration tests in `SimulationIntegrationTests.cs` for wave system
- [✅] Verify no circular dependencies introduced during integration
- [✅] Test wave system works correctly in both environments
- [✅] Validate dependency injection setup for wave services
- [✅] Ensure clean architecture principles maintained

### [✅] 5.6 Add Wave Metrics and Analytics
- [✅] Track wave completion rates in simulation with `WaveMetrics` and `SimulationMetrics` classes
- [✅] Monitor enemy spawn timing accuracy with `EnemySpawnTiming` tracking
- [✅] Measure wave difficulty progression with automated difficulty rating calculation
- [✅] Export wave performance data for analysis via `WaveMetricsCollector` service
- [✅] Create wave balance testing scenarios with comprehensive metrics collection
- [✅] Integrate metrics collection into `GameSimRunner` for real-time tracking

### [✅] 5.7 Create Wave Testing Tools
- [✅] Add wave validation commands to simulation runner via `WaveTestingCommands`
- [✅] Implement automated wave progression tests with balance testing scenarios
- [✅] Add wave data integrity checks and configuration validation tools
- [✅] Create visual progress bars for wave simulation testing with real-time updates
- [✅] Enable quick balance testing for wave configurations with comprehensive reporting
- [✅] Provide CLI interface for wave testing with detailed analysis and export capabilities
- [✅] Fixed build compilation errors in type conversions and property references

---

## Phase 6: Performance and Quality
**Priority: LOW**

### [ ] 6.1 Optimize Wave Data Loading
- [ ] Implement lazy loading for large wave sets
- [ ] Add caching mechanisms for frequently accessed data
- [ ] Optimize JSON parsing performance
- [ ] Memory usage optimization for wave configurations

### [ ] 6.2 Add Comprehensive Testing
- [ ] Unit tests for all wave services
- [ ] Integration tests for wave spawning
- [ ] Performance tests for large wave sets
- [ ] Validation tests for JSON schema compliance

### [ ] 6.3 Documentation and Examples
- [ ] Create wave configuration documentation
- [ ] Add examples of custom wave sets
- [ ] Document the wave system architecture
- [ ] Provide troubleshooting guides

---

## Validation Criteria

### Phase 1 Success Criteria:
- [ ] No more "cannot instantiate" errors in Godot
- [ ] All wave services load without exceptions
- [ ] Main.tscn can reference WaveConfigService without errors

### Phase 2 Success Criteria:
- [✅] Consistent naming throughout all layers
- [✅] No remaining "Config" vs "Configuration" inconsistencies  
- [✅] Clean compilation with updated names
- [✅] All Infrastructure files organized in proper subdirectories
- [✅] No files directly in `src/Infrastructure/Waves/`
- [✅] All references to `WaveModel` and `WaveSetModel` updated

### Phase 3 Success Criteria:
- [✅] All wave data loaded from JSON files in `data/waves/`
- [✅] No hardcoded wave generation in services
- [✅] Flexible wave set loading from different files

### Phase 4 Success Criteria:
- [✅] Clean service architecture without circular dependencies
- [✅] Proper separation of concerns between Domain/Application/Infrastructure
- [✅] Robust error handling and logging

### Phase 5 Success Criteria:
- [✅] Wave system interfaces properly aligned with `GameSimRunner` architecture
- [✅] `MockWaveService.cs` successfully loads configuration from `data/simulation/wave-configs.json`
- [✅] All wave configurations present in simulation JSON files with no hardcoded values
- [✅] Wave logic unified between Godot and simulation environments
- [✅] Integration tests validate wave system works in both contexts
- [✅] No circular dependencies introduced during integration
- [✅] Visual wave simulation testing with progress indicators and real-time metrics
- [✅] Quick balance testing capabilities for wave configurations with comprehensive CLI tools
- [✅] Complete wave metrics and analytics system with performance tracking
- [✅] Wave testing tools with validation, reporting, and export capabilities
- [✅] All compilation errors resolved and system builds successfully

### Phase 6 Success Criteria:
- [ ] Optimal performance for wave loading and spawning
- [ ] Full test coverage for wave system
- [ ] Complete documentation and examples

---

## Notes
- This plan prioritizes fixing the critical runtime error first
- Naming consistency is important for maintainability 
- Data-driven approach will improve flexibility and modding support
- GameSimRunner integration ensures the system works for both gameplay and testing
- Each phase has clear deliverables and success criteria
- The plan maintains clean architecture principles throughout

