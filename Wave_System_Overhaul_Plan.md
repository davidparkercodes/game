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

### [ ] 5.1 Connect Wave System to Simulation
- [ ] Ensure `GameSimRunner` can access wave configuration data
- [ ] Implement wave simulation **completely independent of Godot** (Domain/Application only)
- [ ] Add wave progression testing capabilities
- [ ] Create mock wave services for testing
- [ ] Ensure no Infrastructure layer dependencies in simulation

### [ ] 5.2 Add Wave Metrics and Analytics
- [ ] Track wave completion rates
- [ ] Monitor enemy spawn timing accuracy
- [ ] Measure wave difficulty progression
- [ ] Export wave performance data

### [ ] 5.3 Create Wave Testing Tools
- [ ] Add wave validation commands to simulation runner
- [ ] Implement wave balance testing scenarios
- [ ] Create automated wave progression tests
- [ ] Add wave data integrity checks

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
- [ ] GameSimRunner can run wave simulations correctly
- [ ] Wave system works both in Godot and simulation environments
- [ ] Comprehensive testing capabilities

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

